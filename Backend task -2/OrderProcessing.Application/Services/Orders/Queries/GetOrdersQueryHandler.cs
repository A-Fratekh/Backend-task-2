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

public class GetOrdersQuery : IRequest<List<OrderDto>>
{
}
public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<OrderDto>>
{
    private readonly IReadRepository<Order> _orderReadRepository;
    public GetOrdersQueryHandler(IReadRepository<Order> orderReadRepository)
    {
        _orderReadRepository = orderReadRepository;
    }

    public Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = _orderReadRepository.GetAll();
        var orderItems = new List<OrderItemDto>();
        var result = new List<OrderDto>();
        foreach (var order in orders) {
            foreach (var orderItem in order.OrderItems) {
                orderItems.Add(new OrderItemDto
                {
                    OrderItemId = orderItem.OrderItemId,
                    Price= orderItem.Price.Amount,
                    Quantity= orderItem.Quantity,
                    Comments= orderItem.Comments,
                });
            }
            result.Add(new OrderDto
            {
                Id = order.OrderId,
                CustomerName = order.CustomerName,
                Date = order.Date,
                Total=order.Total.Amount,
                State=order.State,
                Items=orderItems,
            });
        }

        return Task.FromResult(result);
    }
}