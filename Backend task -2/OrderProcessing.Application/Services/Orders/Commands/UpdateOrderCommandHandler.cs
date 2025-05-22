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

public class UpdateOrderCommand : IRequest
{
    public int OrderId { get; set; }
    public Money Total { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}
public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
{
    private readonly IRepository<Order> _orderRepostiory;
    private readonly IReadRepository<Order> _orderReadRepository;
    private readonly IReadRepository<Product> _productReadRepository;

    public UpdateOrderCommandHandler(IRepository<Order> orderRepostiory,
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
        foreach (var item in request.OrderItems)
        {
            var product = _productReadRepository.GetById(item.ProductId);
            order.AddOrderItem(item.OrderItemId,request.OrderId,item.ProductId, item.Quantity, product.CurrentPrice , item.Comments);
        }
        _orderRepostiory.Update(order);
        return Task.FromResult(order.OrderId);
    }
}
