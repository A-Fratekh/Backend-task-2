﻿using OrderProcessing.Domain.Shared;

namespace OrderProcessing.Domain.Aggregates.OrderAggregate;

public class OrderItem : Entity
{
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money Price { get; private set; }
    public string? Comments { get; private set; }
    protected OrderItem() { }
    public OrderItem( int orderId, int orderItemId, int productId, int quantity,Money price, string? comments)
    {
        OrderId = orderId;
        Id = orderItemId;
        ProductId = productId;
        Quantity = quantity;
        Price = price;
        Comments = comments;
        
    }
    internal void UpdatePrice(Money newPrice)
    {
        Price = newPrice;
    }

    internal void UpdateQuantity( int quantity)
    {
        Quantity = quantity;
    }

}
