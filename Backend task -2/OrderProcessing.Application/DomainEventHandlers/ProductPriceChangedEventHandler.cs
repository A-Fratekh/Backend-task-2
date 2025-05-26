
using MediatR;
using OrderProcessing.Domain.Aggregates.OrderAggregate;
using OrderProcessing.Domain.Aggregates.ProductAggregate;
using OrderProcessing.Domain.SeedWork;

namespace OrderProcessing.Application.DomainEventHandlers;

public class ProductPriceChangedEventHandler : INotificationHandler<ProductPriceChangedEvent>
{
    private readonly IReadRepository<Order> _orderReadRepository;
    private readonly IRepository<Order> _orderRepository;

    public ProductPriceChangedEventHandler(
        IReadRepository<Order> orderReadRepository,
        IRepository<Order> orderRepository)
    {
        _orderReadRepository = orderReadRepository;
        _orderRepository = orderRepository;
    }
    public async Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
    {
        var ordersWithProduct = await GetOrdersContainingProductAsync(notification.ProductId);

        foreach (var order in ordersWithProduct)
        {
            order.UpdateOrderItemPricesForProduct(notification.ProductId, notification.NewPrice);
            _orderRepository.Update(order);
        }
    }
    private async Task<IEnumerable<Order>> GetOrdersContainingProductAsync(int productId)
    { 
        return await Task.FromResult(_orderReadRepository.GetAll().ToList()
            .Where(o => o.OrderItems.Any(oi => oi.ProductId == productId) &&
                       (o.State.StateName == "Draft")).AsEnumerable());
    }
}
