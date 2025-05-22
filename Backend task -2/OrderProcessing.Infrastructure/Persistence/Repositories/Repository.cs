using OrderProcessing.Domain;
using OrderProcessing.Domain.SeedWork;
using OrderProcessing.Infrastructure.Data;

namespace OrderProcessing.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T :Entity, IAggregateRoot
{
    private readonly AppDbContext _context;
    public Repository(AppDbContext context)
    {
        _context = context;
    }
    public void Add(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

         _context.Set<T>().Add(entity);
       

    }
    public void Update(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

         _context.Set<T>().Update(entity);
        
    }
    public void Delete(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _context.Set<T>().Remove(entity);
        
    }
}