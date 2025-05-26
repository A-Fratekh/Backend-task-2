using OrderProcessing.Domain.SeedWork;
namespace OrderProcessing.Domain.Shared;
public class Money : ValueObject
{
    public decimal Amount { get; set; } = 0;
    protected Money(decimal amount)
    {
        Amount = amount;
    }

    public Money SetAmount(decimal amount)
    {
        if (amount < 0) { throw new ArgumentOutOfRangeException(nameof(amount)); }
    
        return new Money(amount);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
    }
}