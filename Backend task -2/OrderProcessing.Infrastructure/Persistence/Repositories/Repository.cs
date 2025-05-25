using Microsoft.EntityFrameworkCore;
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
        _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

    }
    public void Add(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var isTracked = _context.ChangeTracker.Entries<T>()
        .Any(e => e.Entity == entity);

        if (!isTracked)
        {
            _context.Set<T>().Add(entity);
        }


    }
    public void Update(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _context.Update(entity);

        foreach (var entry in _context.ChangeTracker.Entries())
        {
            Console.WriteLine($"{entry.Entity.GetType().Name}: {entry.State}");
        }
    }


    public void Delete(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _context.Set<T>().Remove(entity);
        
    }
}