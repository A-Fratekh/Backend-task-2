using MediatR;
using OrderProcessing.Domain.SeedWork;
using OrderProcessing.Domain.Shared;

namespace OrderProcessing.Domain.Aggregates.ProductAggregate;

public class ProductPriceChangedEvent : IDomainEvent, INotification
{
    public int ProductId { get; }
    public Money OldPrice { get; }
    public Money NewPrice { get; }

    public ProductPriceChangedEvent(int productId, Money oldPrice, Money newPrice)
    {
        ProductId = productId;
        OldPrice = oldPrice;
        NewPrice = newPrice;
    }
}
