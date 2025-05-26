namespace OrderProcessing.Domain.Aggregates.OrderAggregate;

 public class OrderState :Entity
{
    public string StateName { get; private set; }

    public OrderState (int stateId, string stateName)
    {
        Id = stateId;
        StateName = stateName;
    }
    internal void SetState( string name)
    {
        StateName = name;
    }

}
