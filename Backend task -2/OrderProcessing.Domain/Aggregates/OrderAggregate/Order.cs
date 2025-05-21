using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderProcessing.Domain.Shared;

namespace OrderProcessing.Domain.Aggregates.OrderAggregate;

public class Order : Entity, IAggregateRoot
{
    public DateOnly Date { get; private set; }
    public string CustomerName { get; private set; }
    public Money Total { get; private set; }
    public OrderState State { get; private set; } = OrderState.Draft;
    public bool IsSubmitted { get; private set; } = false;
    private readonly List<OrderItem> _orderItems = [];
    public virtual IReadOnlyList<OrderItem> OrderItems => _orderItems;

    private Order() { }

    public Order(DateOnly date, string customerName, Money total, OrderState state, bool isSubmitted)
    {
        Date = date;
        CustomerName = customerName;
        Total = total;
        State = state;
        IsSubmitted = isSubmitted;

    }
    //public void AddOrderItem(OrderItem item)
    //{
    //    if(!_orderItems.Contains(item)) 
    //    _orderItems.Add(item);
    //    else
    //    {
    //        item.UpdateQuantity();
    //    }
    //    CalculateOrderTotalAmount();
    //}
    //public void RemoveOrderItem(OrderItem item)
    //{
    //    if (!_orderItems.Remove(item))
    //    {
    //        throw new NullReferenceException($"Item with id {item.OrderItemId} does not exist");
    //    }
    //    CalculateOrderTotalAmount();
    //}
    //public Money CalculateOrderTotalAmount()
    //{
    //    foreach (var item in _orderItems)
    //    {
    //        Total.Amount += (item.Price.Amount * item.Quantity);
    //    }
    //    return Total;
    //}
    //public void SubmitOrder()
    //{
    //    IsSubmitted = true;
    //    State = OrderState.Submitted;
    //}
   

}
