using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using OrderProcessing.Application.DTOs;
using OrderProcessing.Domain.Aggregates.OrderAggregate;
using OrderProcessing.Domain.SeedWork;

namespace OrderProcessing.Application.Services.Orders.Queries;

public class GetOrderByIdQuery : IRequest<OrderDto>
{
    public int OrderId { get; set; }
}
public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IReadRepository<Order> _orderReadRepository;
    private readonly IReadRepository<OrderItem> _orderItemsReadRepository;
    public GetOrderByIdQueryHandler(IReadRepository<Order> orderReadRepository, IReadRepository<OrderItem> orderItemsReadRepository)
    {
        _orderReadRepository = orderReadRepository;
        _orderItemsReadRepository = orderItemsReadRepository;
    }

    public Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = _orderReadRepository.GetById(request.OrderId, "OrderItems");

        var orderDto = new OrderDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            Date = order.Date,
            Total = order.Total.Amount,
            State = order.State.ToString(),
            Items = order.OrderItems.Select(oi => new OrderItemDto
            {
                OrderItemId = oi.OrderItemId,
                Price = oi.Price.Amount,
                Quantity = oi.Quantity,
                Comments = oi.Comments
            }).ToList()
        };

        return Task.FromResult(orderDto);
    }
}
