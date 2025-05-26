using MediatR;
using OrderProcessing.Application.DTOs;
using OrderProcessing.Domain.Aggregates.OrderAggregate;
using OrderProcessing.Domain.Aggregates.ProductAggregate;
using OrderProcessing.Domain.SeedWork;

namespace OrderProcessing.Application.Services.Orders.Queries;

public class GetOrderByIdQuery : IRequest<OrderDto>
{
    public int OrderId { get; set; }
}

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IReadRepository<Order> _orderReadRepository;
    private readonly IReadRepository<Product> _productReadRepository;

    public GetOrderByIdQueryHandler(
        IReadRepository<Order> orderReadRepository,
        IReadRepository<Product> productReadRepository)
    {
        _orderReadRepository = orderReadRepository;
        _productReadRepository = productReadRepository;
    }

    public Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = _orderReadRepository.GetById(request.OrderId);

        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");
        }

        var productIds = order.OrderItems.Select(oi => oi.ProductId).Distinct().ToList();

        var products = _productReadRepository
            .GetAll()
            .Where(p => productIds.Contains(p.Id))
            .ToList();

        var productNameMap = products.ToDictionary(p => p.Id, p => p.Name);

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
                ProductName = productNameMap.GetValueOrDefault(oi.ProductId),
                Price = oi.Price.Amount,
                Quantity = oi.Quantity,
                Comments = oi.Comments
            }).ToList()
        };

        return Task.FromResult(orderDto);
    }
}
