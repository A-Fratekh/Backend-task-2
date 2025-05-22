using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using OrderProcessing.Domain.Aggregates.OrderAggregate;
using OrderProcessing.Domain.SeedWork;

namespace OrderProcessing.Application.Services.Orders.Commands;

public class DeleteOrderCommand : IRequest
{
    public int OrderId { get; set; }
}
public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
{
    private readonly IReadRepository<Order> _orderReadRepository;
    private readonly IRepository<Order> _orderRepository;


    public DeleteOrderCommandHandler(IReadRepository<Order> orderReadRepository, IRepository<Order> orderRepository)
    {
        _orderReadRepository = orderReadRepository;
        _orderRepository = orderRepository;
    }

    public Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order =  _orderReadRepository.GetById(request.OrderId) ?? 
            throw new NullReferenceException($"Order with id {request.OrderId} not found");

        _orderRepository.Delete(order);
        return Task.CompletedTask;
        

    }
}