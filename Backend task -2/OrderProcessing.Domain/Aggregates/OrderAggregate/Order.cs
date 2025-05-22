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
    private readonly List<OrderItem> _orderItems = [];
    public IReadOnlyList<OrderItem> OrderItems => _orderItems;

    private Order() { }

    public Order(
        int orderId,
        DateOnly date,
        string customerName,
        OrderState state,
        List<OrderItem> orderItems,
        bool isSubmitted = false
        )
    {
        OrderId = orderId;
        Date = date;
        CustomerName = customerName;
        State = state;
        Total = CalculateOrderTotalAmount();
        IsSubmitted = isSubmitted;
        _orderItems = orderItems;
    }

    public void AddOrderItem(int orderItemId, int orderId, int productId, int quantity, Money price, string comments)
    {
        
          var  item = new OrderItem(orderItemId, orderId, productId, quantity, price, comments);
            _orderItems.Add(item); 
        
       
        Total = CalculateOrderTotalAmount();
    }
    public void RemoveOrderItem(int itemId)
    {
        OrderItem? item = _orderItems.Find(oi => oi.OrderItemId == itemId);
        if (item == null)
        {
            throw new NullReferenceException($"Item with id {itemId} does not exist");
        }
        _orderItems.Remove(item);
        Total = CalculateOrderTotalAmount();
    }
    public Money CalculateOrderTotalAmount()
    {
        Money total = new Money();
        foreach (var item in _orderItems)
        {
                total.Amount += item.Price.Amount * item.Quantity;
        }
       
        return total;
    }
    //public void SubmitOrder()
    //{
    //    IsSubmitted = true;
    //    State = OrderState.Submitted;
    //}

}
