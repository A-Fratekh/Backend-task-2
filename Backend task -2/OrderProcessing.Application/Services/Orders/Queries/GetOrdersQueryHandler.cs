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
        var orders = _orderReadRepository.GetAll().ToList();

        var result = orders.Select(order => new OrderDto
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
        }).ToList();

        return Task.FromResult(result);
    }
}