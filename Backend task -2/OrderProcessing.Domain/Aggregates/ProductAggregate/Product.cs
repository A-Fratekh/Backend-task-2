using OrderProcessing.Domain.Shared;

namespace OrderProcessing.Domain.Aggregates.ProductAggregate;

public class Product :Entity, IAggregateRoot
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public byte[] Photo { get; private set; }
    public Money CurrentPrice { get; private set; }
   
    protected Product() { }
    public Product(int productId, string name, string description, byte[] photo, Money currentPrice)
    {
        Id = productId;
        Name = name;
        Description = description;
        Photo = photo;
        CurrentPrice = currentPrice;
    }
    public void UpdatePrice(Money newPrice)
    {
        if (CurrentPrice.Amount != newPrice.Amount)
        {
            var oldPrice = CurrentPrice;
            CurrentPrice = newPrice;
            AddDomainEvent(new ProductPriceChangedEvent(Id, oldPrice, newPrice));
        }
    }

    public void Update(string name, string description, byte[] photo)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Photo = photo ?? throw new ArgumentNullException(nameof(photo));
    }

}
