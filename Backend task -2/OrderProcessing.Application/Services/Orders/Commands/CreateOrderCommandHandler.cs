using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using OrderProcessing.Domain.Aggregates.OrderAggregate;
using OrderProcessing.Domain.Aggregates.ProductAggregate;
using OrderProcessing.Domain.SeedWork;
using OrderProcessing.Domain.Shared;

namespace OrderProcessing.Application.Services.Orders.Commands;

public class CreateOrderCommand : IRequest<int>
{
    public int Id { get; set; }
    public DateOnly Date {  get; set; }
    public string CustomerName { get; set; }
    public OrderState State { get; set; }
    public bool IsSubmitted { get; set; } = false;
    public List<OrderItem> OrderItems { get; set; }
}
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IRepository<Order> _orderRepostiory;
    private readonly IReadRepository<Product> _productReadRepostiory;
    private readonly IReadRepository<Order> _orderReadRepostiory;



    public CreateOrderCommandHandler(IRepository<Order> orderRepostiory,
        IReadRepository<Product> productReadRepostiory,
        IReadRepository<Order> orderReadRepostiory)
    {
        _orderRepostiory = orderRepostiory;
        _productReadRepostiory = productReadRepostiory;
        _orderReadRepostiory = orderReadRepostiory;
    }

    public Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order(request.Id, request.Date, request.CustomerName,
             request.State, request.OrderItems, request.IsSubmitted);
        if (request.OrderItems == null)
        {
            throw new ArgumentNullException(nameof(request.OrderItems));
        }
        foreach (var item in request.OrderItems.ToList())
        {
            var product = _productReadRepostiory.GetById(item.ProductId);
            order.AddOrderItem(item.OrderItemId ,request.Id, product.ProductId, item.Quantity, product.CurrentPrice, item.Comments);
        }
        _orderRepostiory.Add(order);

        return Task.FromResult(order.OrderId);
    }
}
