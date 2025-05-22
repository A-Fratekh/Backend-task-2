using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderProcessing.Domain.Shared;

namespace OrderProcessing.Domain.Aggregates.ProductAggregate;

public class Product :Entity, IAggregateRoot
{
    public int ProductId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public byte[] Photo { get; private set; }
    public Money CurrentPrice { get; private set; }
   
    private Product() { }
    public Product(int productId, string name, string description, byte[] photo, Money currentPrice)
    {
        ProductId = productId;
        Name = name;
        Description = description;
        Photo = photo;
        CurrentPrice = currentPrice;
    }
}
