using Microsoft.EntityFrameworkCore;
using OrderProcessing.Domain;
using OrderProcessing.Domain.SeedWork;
using OrderProcessing.Infrastructure.Data;

namespace OrderProcessing.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : Entity, IAggregateRoot
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

            _context.Add(entity);
    }
    public void Update(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _context.Update(entity);

    }
    public void Delete(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _context.Remove(entity);
        
    }
    public T GetById(int id)
    {
        var entity = _context.Set<T>().Find(id);
        if (entity == null)
            throw new KeyNotFoundException($"Entity of type {typeof(T).Name} with ID {id} not found");
        return entity;
    }

    public int GetNextId()
    {
        var dbSet = _context.Set<T>();
        var maxId = dbSet.Any() ? dbSet.Max(e => e.Id) : 0;
        return maxId + 1;
    }
}