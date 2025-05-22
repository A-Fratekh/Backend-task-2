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
    public GetOrderByIdQueryHandler(IReadRepository<Order> orderReadRepository)
    {
        _orderReadRepository = orderReadRepository;
    }

    public Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = _orderReadRepository.GetById(request.OrderId);
        var orderItems = new List<OrderItemDto>();

            foreach (var orderItem in order.OrderItems)
            {
                orderItems.Add(new OrderItemDto
                {
                    OrderItemId = orderItem.OrderItemId,
                    Price = orderItem.Price.Amount,
                    Quantity = orderItem.Quantity,
                    Comments = orderItem.Comments,
                });
            }
            var orderDto =new OrderDto
            {
                Id = order.OrderId,
                CustomerName = order.CustomerName,
                Date = order.Date,
                Total = order.Total.Amount,
                State = order.State,
                Items = orderItems,
            };

        return Task.FromResult(orderDto);
    }
}
