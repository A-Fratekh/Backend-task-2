namespace OrderProcessing.Domain.Shared;

public class Money : ValueObject
{
    public decimal Amount { get; set; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
    }
}