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
    private readonly IReadRepository<OrderItem> _orderItemsReadRepository;
    public GetOrdersQueryHandler(IReadRepository<Order> orderReadRepository, IReadRepository<OrderItem> orderItemsReadRepository)
    {
        _orderReadRepository = orderReadRepository;
        _orderItemsReadRepository = orderItemsReadRepository;
    }

    public Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = _orderReadRepository.GetAll();
        var allOrderItems = _orderItemsReadRepository.GetAll();

        var orderItemsLookup = allOrderItems
            .GroupBy(oi => oi.OrderId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var result = orders.Select(order => new OrderDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            Date = order.Date,
            Total = order.Total.Amount,
            State = order.State.ToString(),
            Items = orderItemsLookup.GetValueOrDefault(order.Id, new List<OrderItem>())
                .Select(oi => new OrderItemDto
                {
                    OrderItemId = oi.OrderItemId,
                    Price = oi.Price.Amount,
                    Quantity = oi.Quantity,
                    Comments = oi.Comments
                }).ToList()
        }).ToList();

        return Task.FromResult(result);
    }
}