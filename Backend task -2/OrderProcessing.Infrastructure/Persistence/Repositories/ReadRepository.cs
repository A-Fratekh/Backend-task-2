using Microsoft.EntityFrameworkCore;
using OrderProcessing.Infrastructure.Data;
using System.Linq.Expressions;
using OrderProcessing.Domain.SeedWork;

namespace OrderProcessing.Infrastructure.Persistence.Repositories;

public class ReadRepository<T> : IReadRepository<T> where T : class
{
    private readonly AppDbContext _context;

    public ReadRepository(AppDbContext context)
    {
        _context = context;
        _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public IEnumerable<T> GetAll()
    {
        IQueryable<T> query = _context.Set<T>();

        return query;
    }

    public T GetById(int id, string? include = null)
    {
        var query = _context.Set<T>().AsQueryable();

        if (!string.IsNullOrEmpty(include))
            query = query.Include(include);

        var entity = query.FirstOrDefault(e => EF.Property<int>(e, "Id") == id);

        if (entity == null)
            throw new KeyNotFoundException($"Entity with Number {id} not found");

        return entity;
    }
    public T GetById(int id)
    {
        var entity = _context.Set<T>().Find(id);

        if (entity == null)
            throw new KeyNotFoundException($"Entity with Number {id} not found");

        return entity;
    }
    public T GetById(object[] id)
    {
        var entity = _context.Set<T>().Find(id);

        if (entity == null)
            throw new KeyNotFoundException($"Entity with Number {id} not found");

        return entity;
    }
    public IQueryable<T> Query()
    {
        return _context.Set<T>().AsQueryable();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().Where(predicate).ToListAsync();
    }

}