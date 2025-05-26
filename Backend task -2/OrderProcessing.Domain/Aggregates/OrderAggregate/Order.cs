using OrderProcessing.Domain.Shared;
namespace OrderProcessing.Domain.Aggregates.OrderAggregate;

public class Order : Entity, IAggregateRoot
{
    public DateOnly Date { get; private set; }
    public string CustomerName { get; private set; }
    public Money Total { get; private set; }
    public int StateId { get; private set; }
    public virtual OrderState State { get; private set; }
    private readonly List<OrderItem> _orderItems = [];
    public virtual IReadOnlyList<OrderItem> OrderItems => _orderItems.ToList();

    protected Order() { }

    public Order(
        int orderId,
        DateOnly date,
        string customerName,
        int stateId,
        List<OrderItem> orderItems
        )
    {
        Id = orderId;
        Date = date;
        CustomerName = customerName;
        StateId = stateId;
        _orderItems = orderItems;
        Total = CalculateOrderTotalAmount();
    }

    public void AddOrderItem(int orderItemId, int orderId, int productId, int quantity, Money price, string? comments)
    {
        if (IsSubmitted()) throw new ArgumentException("Can't add items to a submitted order");
        var item = new OrderItem(orderId, orderItemId, productId, quantity, price, comments);
        _orderItems.Add(item);
    }

    public void RemoveOrderItem(int itemId)
    {
        if (IsSubmitted()) throw new ArgumentException("Can't remove items from a submitted order");
        var item = _orderItems.FirstOrDefault(oi => oi.Id == itemId);
        if (item == null)
        {
            throw new NullReferenceException($"Item with id {itemId} does not exist");
        }
        _orderItems.Remove(item);
    }

    public Money CalculateOrderTotalAmount()
    {
        Total = Total.SetAmount(0);
        decimal total = 0;
        foreach (var item in _orderItems)
        {
            total += item.Price.Amount * item.Quantity;
        }
        Total.SetAmount(total);
        return Total;
    }

    public void SubmitOrder()
    {
        if (IsSubmitted()) throw new ArgumentException("Can't submit an already submitted order");
        StateId = 2;
        State.SetState("Submitted");
    }

    public void UpdateOrderItem(int orderItemId, int quantity)
    {
        if (IsSubmitted()) throw new ArgumentException("Can't update items of a submitted order");
        var item = _orderItems.FirstOrDefault(oi => oi.Id == orderItemId);
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

    public void Update(string customerName, DateOnly date, int stateId)
    {
        CustomerName = customerName ?? throw new ArgumentNullException(nameof(customerName));
        Date = date;
        StateId = stateId;
        Total = CalculateOrderTotalAmount();
    }


    private bool IsSubmitted()
    {
        return StateId == 2;
    }
}