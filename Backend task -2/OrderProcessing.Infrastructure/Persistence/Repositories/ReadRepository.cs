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
        IQueryable<T> query = _context.Set<T>().AsNoTracking();

        return query;
    }

    public T GetById(int id)
    {
        var entity = _context.Set<T>().Find(id);

        if (entity == null)
            throw new KeyNotFoundException($"Entity with Number {id} not found");

        return entity;
    }


}