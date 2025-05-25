namespace OrderProcessing.Domain.SeedWork;

public interface IUnitOfWork 
{
    int SaveChanges();
    Task<int> SaveChangesAsync();
}
