using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using OrderProcessing.Domain.Aggregates.OrderAggregate;
using OrderProcessing.Domain.Aggregates.ProductAggregate;
using OrderProcessing.Domain.SeedWork;

namespace OrderProcessing.Application.DomainEventHandlers;

public class ProductPriceChangedEventHandler : INotificationHandler<ProductPriceChangedEvent>
{
    private readonly IReadRepository<Order> _orderReadRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly IReadRepository<OrderItem> _orderItemsReadRepository;

    public ProductPriceChangedEventHandler(
        IReadRepository<Order> orderReadRepository,
        IRepository<Order> orderRepository,
        IReadRepository<OrderItem> orderItemsReadRepository)
    {
        _orderReadRepository = orderReadRepository;
        _orderRepository = orderRepository;
        _orderItemsReadRepository = orderItemsReadRepository;
    }

    public async Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
    {
        var ordersWithProduct = await GetOrdersContainingProductAsync(notification.ProductId, OrderState.Draft);

        foreach (var order in ordersWithProduct)
        {
            order.UpdateOrderItemPricesForProduct(notification.ProductId, notification.NewPrice);
            _orderRepository.Update(order);
        }

    }

    private async Task<IEnumerable<Order>> GetOrdersContainingProductAsync(int productId, OrderState? state = null)
    { 
        return await Task.FromResult(_orderReadRepository.GetAll().ToList()
            .Where(o => o.OrderItems.Any(oi => oi.ProductId == productId) &&
                       (!state.HasValue || o.State == state.Value))
            .AsEnumerable());
    }
}
