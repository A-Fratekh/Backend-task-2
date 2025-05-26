using MediatR;
using OrderProcessing.Domain.Aggregates.OrderAggregate;
using OrderProcessing.Domain.Aggregates.ProductAggregate;
using OrderProcessing.Domain.SeedWork;

namespace OrderProcessing.Application.Services.Orders.Commands;

public class CreateOrderCommand : IRequest<int>
{
    public int Id { get; set; }
    public DateOnly Date {  get; set; }
    public string CustomerName { get; set; }
    public OrderState State { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IRepository<Order> _orderRepostiory;
    private readonly IReadRepository<Product> _productReadRepostiory;

    public CreateOrderCommandHandler(IRepository<Order> orderRepostiory,
        IReadRepository<Product> productReadRepostiory)
    {
        _orderRepostiory = orderRepostiory;
        _productReadRepostiory = productReadRepostiory;
    }

    public  Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.State == OrderState.Draft)
        {
            var products = _productReadRepostiory.GetAll()
                .ToDictionary(p => p.Id);

            foreach (var item in request.OrderItems)
            {
                var product = products[item.ProductId];

                if (item.Price.Amount != product.CurrentPrice.Amount)
                {
                    throw new ArgumentException($"Item {product.Name} with Id : {item.OrderItemId} price {item.Price.Amount} doesn't equal product {product.Name} current price {product.CurrentPrice.Amount}");
                }
            }
        }
        var order = new Order(request.Id, request.Date, request.CustomerName,
             request.State, request.OrderItems);
        var productGroups = request.OrderItems
            .GroupBy(oi => oi.ProductId)
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var group in productGroups)
        {
            var totalQuantity = group.Sum(x => x.Quantity);
            var firstItem = group.First();

            foreach (var item in group)
            {
                order.RemoveOrderItem(item.OrderItemId);
            }

            if (totalQuantity > 0)
            {
                order.AddOrderItem(firstItem.OrderItemId, request.Id, firstItem.ProductId,
                    totalQuantity, firstItem.Price, firstItem.Comments);
            }
        }

        _orderRepostiory.Add(order);
        return Task.FromResult(order.Id);
    }
}
