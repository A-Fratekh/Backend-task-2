using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderProcessing.Domain.Shared;

namespace OrderProcessing.Domain.Aggregates.OrderAggregate;

public class Order : Entity, IAggregateRoot
{
    public int OrderId { get; private set; }
    public DateOnly Date { get; private set; }
    public string CustomerName { get; private set; }
    public Money Total { get; private set; }
    public OrderState State { get; private set; } = OrderState.Draft;
    public bool IsSubmitted { get; private set; } = false;
    private readonly List<OrderItem> _orderItems = []!;
    public IReadOnlyList<OrderItem> OrderItems => _orderItems;

    private Order() { }

    public Order(DateOnly date, string customerName, Money total, OrderState state, bool isSubmitted)
    {
        Date = date;
        CustomerName = customerName;
        Total = total;
        State = state;
        IsSubmitted = isSubmitted;
        _orderItems = new List<OrderItem>();
    }
    public void AddOrderItem(int productId, int quantity, Money price, string comments)
    {

        var item = _orderItems.FirstOrDefault(oi => oi.ProductId == productId);

        if (item == null) { 
            item = new OrderItem(OrderId, productId, quantity, price, comments);
            _orderItems.Add(item); 
        }
        else
        {
            item.UpdateQuantity();
        }
        CalculateOrderTotalAmount();
    }
    //TODO: 
    public void RemoveOrderItem(int itemId)
    {
        //OrderItem ? item = _orderItems.Find(oi=>oi.OrderItemId==itemId);
        //if (item==null)
        //{
        //    throw new NullReferenceException($"Item with id {itemId} does not exist");
        //}
        //_orderItems.Remove(item);
        //CalculateOrderTotalAmount();
    }
    public Money CalculateOrderTotalAmount()
    {
        decimal total = 0;
        foreach (var item in _orderItems)
        {
            total += (item.Price.Amount * item.Quantity);
        }
        Total.Amount = total;
        return Total;
    }
    public void SubmitOrder()
    {
        IsSubmitted = true;
        State = OrderState.Submitted;
    }

}
