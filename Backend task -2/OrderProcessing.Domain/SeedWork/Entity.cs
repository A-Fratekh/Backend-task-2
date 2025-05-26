using MediatR;

namespace OrderProcessing.Domain;

public class Entity
{
    private List<INotification> _domainEvents=new();
    public List<INotification> DomainEvents => _domainEvents;

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents = _domainEvents ?? new List<INotification>();
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    public override bool Equals(object? obj)
    {
        if (obj is null || GetType() != obj.GetType())
            return false;

        return ReferenceEquals(this, obj);
    }
   
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
