using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderProcessing.Domain.Aggregates.ProductAggregate;
using OrderProcessing.Domain.Shared;

namespace OrderProcessing.Domain.Aggregates.OrderAggregate;

public class OrderItem
{
    public int OrderItemId { get; private set; }
    [ForeignKey(nameof(Product))]
    public int ProductId { get; private set; }
    public int Quantity { get; private set; } = 1;
    public Money Price { get; private set; }
    public string Comments { get; private set; }

    private OrderItem() { }
    public OrderItem( int productId, int quantity, Money price, string comments)
    {
        ProductId = productId;
        Quantity = quantity;
        Price = price;
        Comments = comments;
    }

    //public void UpdateQuantity()
    //{
    //    Quantity += 1;
    //}
}
