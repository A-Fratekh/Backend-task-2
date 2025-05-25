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
        var order = new Order(request.Id, request.Date, request.CustomerName,
             request.State, request.OrderItems);
        if (order.State == OrderState.Draft)
        {
            foreach (var item in request.OrderItems)
            {
                var product = _productReadRepostiory.GetById(item.ProductId);
                if (item.Price.Amount != product.CurrentPrice.Amount)
                {
                    throw new ArgumentException($"Item {product.Name} with Id : {item.OrderItemId} price {item.Price.Amount} doesn't equal product {product.Name} current price {product.CurrentPrice.Amount}");
                }
            }
        }
        _orderRepostiory.Add(order);

        return Task.FromResult(order.Id);
    }
}
