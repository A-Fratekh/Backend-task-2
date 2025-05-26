using MediatR;
using OrderProcessing.Domain.Aggregates.OrderAggregate;
using OrderProcessing.Domain.Aggregates.ProductAggregate;
using OrderProcessing.Domain.SeedWork;

namespace OrderProcessing.Application.Services.Orders.Commands;

public class UpdateOrderCommand : IRequest
{
    public int OrderId { get; set; }
    public DateOnly Date { get; set; }
    public string CustomerName { get; set; }
    public int StateId { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
{
    private readonly IRepository<Order> _orderRepostiory;
    private readonly IReadRepository<Product> _productReadRepository;
    private readonly IReadRepository<OrderState> _stateReadRepository;


    public UpdateOrderCommandHandler(
        IRepository<Order> orderRepostiory,
        IReadRepository<Product> productReadRepository,
        IReadRepository<OrderState> stateReadRepository)
    {
        _orderRepostiory = orderRepostiory;
        _productReadRepository = productReadRepository;
        _stateReadRepository = stateReadRepository;
    }

    public Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = _orderRepostiory.GetById(request.OrderId);

        var currentItems = order.OrderItems
            .ToDictionary(oi => new { oi.OrderId, oi.Id });

        var uniqueItems = request.OrderItems
            .GroupBy(oi => oi.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalQuantity = g.Sum(x => x.Quantity),
                FirstItem = g.First(),
                })
            .ToList();

        var products = _productReadRepository.GetAll()
            .ToDictionary(p => p.Id);

        foreach (var uniqueItem in uniqueItems)
        {
            var firstItem = uniqueItem.FirstItem;

            if (uniqueItem.TotalQuantity == 0)
            {
                var itemsToRemove = currentItems.Values
                    .Where(ci => ci.ProductId == uniqueItem.ProductId)
                    .ToList();

                foreach (var itemToRemove in itemsToRemove)
                {
                    order.RemoveOrderItem(itemToRemove.Id);
                }
                continue;
            }

            var product = products[uniqueItem.ProductId];
            if (firstItem.Price.Amount != product.CurrentPrice.Amount)
            {
                throw new InvalidOperationException(
                    $"Item {product.Name} with product id {uniqueItem.ProductId} price {firstItem.Price.Amount} doesn't equal product current price {product.CurrentPrice.Amount}");
            }
            var existingItem = currentItems.Values
                .FirstOrDefault(ci => ci.ProductId == uniqueItem.ProductId);

            if (existingItem != null)
            {
                order.UpdateOrderItem(existingItem.Id, uniqueItem.TotalQuantity);
                var additionalItems = currentItems.Values
                    .Where(ci => ci.ProductId == uniqueItem.ProductId && ci.Id != existingItem.Id)
                    .ToList();

                foreach (var additionalItem in additionalItems)
                {
                    order.RemoveOrderItem(additionalItem.Id);
                }
            }
            else
            {
                order.AddOrderItem(firstItem.Id, request.OrderId, uniqueItem.ProductId,
                    uniqueItem.TotalQuantity, firstItem.Price, firstItem.Comments);
            }
        }
        var newItems = request.OrderItems.ToDictionary(ni => new { ni.OrderId, ni.Id });
        foreach (var currentItem in currentItems.Values)
        {
            var key = new { currentItem.OrderId, currentItem.Id };
            if (!newItems.ContainsKey(key))
            {
                order.RemoveOrderItem(currentItem.Id);
            }
        }
        var state = _stateReadRepository.GetById(request.StateId);
        if (state == null)  throw new ArgumentException($"State with id {request.StateId} could not be found");
        if (state.StateName == "Submitted") order.SubmitOrder();
        order.Update(request.CustomerName, request.Date, request.StateId);
        _orderRepostiory.Update(order);

        if (order.Total.Amount==0) _orderRepostiory.Delete(order);

        return Task.CompletedTask;
    }
}
