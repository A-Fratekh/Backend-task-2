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
    public OrderState State { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
{
    private readonly IRepository<Order> _orderRepostiory;
    private readonly IReadRepository<Order> _orderReadRepository;
    private readonly IReadRepository<Product> _productReadRepository;

    public UpdateOrderCommandHandler(
        IRepository<Order> orderRepostiory,
        IReadRepository<Order> orderReadRepository,
        IReadRepository<Product> productReadRepository)
    {
        _orderRepostiory = orderRepostiory;
        _orderReadRepository = orderReadRepository;
        _productReadRepository = productReadRepository;
    }

    public Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = _orderReadRepository.GetById(request.OrderId);

        var currentItems = order.OrderItems
            .ToDictionary(oi => new { oi.OrderId, oi.OrderItemId });

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
                    order.RemoveOrderItem(itemToRemove.OrderItemId);
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
                order.UpdateOrderItem(existingItem.OrderItemId, uniqueItem.TotalQuantity);
                var additionalItems = currentItems.Values
                    .Where(ci => ci.ProductId == uniqueItem.ProductId && ci.OrderItemId != existingItem.OrderItemId)
                    .ToList();

                foreach (var additionalItem in additionalItems)
                {
                    order.RemoveOrderItem(additionalItem.OrderItemId);
                }
            }
            else
            {
                order.AddOrderItem(firstItem.OrderItemId, request.OrderId, uniqueItem.ProductId,
                    uniqueItem.TotalQuantity, firstItem.Price, firstItem.Comments);
            }
        }
        var newItems = request.OrderItems.ToDictionary(ni => new { ni.OrderId, ni.OrderItemId });
        foreach (var currentItem in currentItems.Values)
        {
            var key = new { currentItem.OrderId, currentItem.OrderItemId };
            if (!newItems.ContainsKey(key))
            {
                order.RemoveOrderItem(currentItem.OrderItemId);
            }
        }

        if (request.State == OrderState.Submitted) order.SubmitOrder();
        order.Update(request.CustomerName, request.Date, request.State);
        _orderRepostiory.Update(order);

        if (order.Total.Amount==0) _orderRepostiory.Delete(order);

        return Task.CompletedTask;
    }
}
