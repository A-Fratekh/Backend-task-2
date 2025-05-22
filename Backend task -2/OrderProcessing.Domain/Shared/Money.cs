using OrderProcessing.Domain.SeedWork;

namespace OrderProcessing.Domain.Shared;

public class Money : ValueObject
{
    public decimal Amount { get; set; } = 0;
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
    }
}