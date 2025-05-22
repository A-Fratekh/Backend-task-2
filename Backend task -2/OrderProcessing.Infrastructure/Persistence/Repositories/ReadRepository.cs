using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public int GetNextId()
    {
        var id = typeof(T).GetProperty("OrderId");
        if (id == null || id.PropertyType != typeof(int))
        {
            throw new InvalidOperationException($"Entity type {typeof(T).Name} does not have an integer Id property");
        }

        var maxId = _context.Set<T>()
            .Select(e => EF.Property<int>(e, "OrderId"))
            .Max();
        return maxId + 1;

    }
    public T GetById(int id)
    {
        var entity = _context.Set<T>().Find(id);

        if (entity == null)
            throw new KeyNotFoundException($"Entity with Number {id} not found");

        return entity;
    }
}