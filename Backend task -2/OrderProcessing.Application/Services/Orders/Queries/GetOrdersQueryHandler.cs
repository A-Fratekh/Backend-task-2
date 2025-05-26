using MediatR;
using OrderProcessing.Application.DTOs;
using OrderProcessing.Domain.Aggregates.OrderAggregate;
using OrderProcessing.Domain.Aggregates.ProductAggregate;
using OrderProcessing.Domain.SeedWork;

namespace OrderProcessing.Application.Services.Orders.Queries;

public class GetOrdersQuery : IRequest<List<OrderDto>>
{
}
public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<OrderDto>>
{
    private readonly IReadRepository<Order> _orderReadRepository;
    private readonly IReadRepository<Product> _productReadRepository;

    public GetOrdersQueryHandler(
        IReadRepository<Order> orderReadRepository,
        IReadRepository<Product> productReadRepository)
    {
        _orderReadRepository = orderReadRepository;
        _productReadRepository = productReadRepository;
    }

    public Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = _orderReadRepository.GetAll().ToList();

        var productIds = orders
            .SelectMany(o => o.OrderItems)
            .Select(oi => oi.ProductId)
            .Distinct()
            .ToList();

        var products = _productReadRepository.GetAll()
            .Where(p => productIds.Contains(p.Id))
            .ToList();

        var productNameMap = products.ToDictionary(p => p.Id, p => p.Name);

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
                ProductName = productNameMap.GetValueOrDefault(oi.ProductId) ,
                Price = oi.Price.Amount,
                Quantity = oi.Quantity,
                Comments = oi.Comments
            }).ToList()
        }).ToList();

        return Task.FromResult(result);
    }
}