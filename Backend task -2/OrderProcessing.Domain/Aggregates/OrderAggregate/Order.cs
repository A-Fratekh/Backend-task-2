using OrderProcessing.Domain.Shared;

namespace OrderProcessing.Domain.Aggregates.OrderAggregate;

public class Order : Entity, IAggregateRoot
{
    public int Id { get; private set; }
    public DateOnly Date { get; private set; }
    public string CustomerName { get; private set; }
    public Money Total { get; private set; }
    public OrderState State { get; private set; } = OrderState.Draft;
    private readonly List<OrderItem> _orderItems = [];
    public virtual IReadOnlyList<OrderItem> OrderItems => _orderItems.ToList();

    protected Order() { }

    public Order(
        int orderId,
        DateOnly date,
        string customerName,
        OrderState state,
        List<OrderItem> orderItems
        )
    {
        Id = orderId;
        Date = date;
        CustomerName = customerName;
        State = state;
        _orderItems = orderItems;
        Total = CalculateOrderTotalAmount();
    }
    public void AddOrderItem(int orderItemId, int orderId, int productId, int quantity, Money price, string? comments)
    {
        var item = new OrderItem(orderId, orderItemId, productId, quantity, price, comments);
        _orderItems.Add(item);
    }
    public void RemoveOrderItem(int itemId)
    {
        var item = _orderItems.FirstOrDefault(oi => oi.OrderItemId == itemId);
        if (item == null)
        {
            throw new NullReferenceException($"Item with id {itemId} does not exist");
        }
        _orderItems.Remove(item);
    }
    public Money CalculateOrderTotalAmount()
    {
        Total = new Money(0);
        decimal total = 0;
        foreach (var item in _orderItems)
        {
            total += item.Price.Amount * item.Quantity;
        }
        Total.Amount = total;
        return Total;
    }
    public void SubmitOrder()
    {
        State = OrderState.Submitted;
    }

    public void UpdateOrderItem(int orderItemId, int quantity)
    {

        var item = _orderItems.FirstOrDefault(oi => oi.OrderItemId == orderItemId);
        if (item == null)
        {
            throw new NullReferenceException($"Item with id {orderItemId} does not exist");
        }
        item.UpdateQuantity(quantity);
    }

    public void UpdateOrderItemPricesForProduct(int productId, Money newPrice)
    {
        var itemsToUpdate = _orderItems.Where(oi => oi.ProductId == productId).ToList();

        foreach (var item in itemsToUpdate)
        {
            item.UpdatePrice(newPrice);
        }
        Total = CalculateOrderTotalAmount();
    }

    public void Update(string customerName, DateOnly date, OrderState state)
    {
        CustomerName = customerName ?? throw new ArgumentNullException(nameof(customerName));
        Date = date;
        State = state;
        Total = CalculateOrderTotalAmount();
    }
}
