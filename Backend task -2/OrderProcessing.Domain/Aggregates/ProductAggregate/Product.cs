using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderProcessing.Domain.Aggregates.OrderAggregate;
using OrderProcessing.Domain.Shared;

namespace OrderProcessing.Domain.Aggregates.ProductAggregate;

public class Product :Entity, IAggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public byte[] Photo { get; private set; }
    public Money CurrentPrice { get; private set; }

    private readonly List<Order> _orders = [];
    public virtual IReadOnlyList<Order> Orders => _orders;

    private Product() { }
    public Product(string name, string description, byte[] photo, Money currentPrice)
    {
        Name = name;
        Description = description;
        Photo = photo;
        CurrentPrice = currentPrice;
    }
}
