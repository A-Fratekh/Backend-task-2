using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderProcessing.Domain;
using OrderProcessing.Domain.SeedWork;
using OrderProcessing.Infrastructure.Data;

namespace OrderProcessing.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IPublisher _publisher;

    public UnitOfWork(AppDbContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        var entitiesWithEvents = _context.ChangeTracker.Entries<Entity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();
        foreach (var entity in entitiesWithEvents)
        {
            foreach (var domainEvent in entity.DomainEvents)
            {
                if (domainEvent is INotification notification)
                {
                    await _publisher.Publish(notification);
                }
            }
            entity.ClearDomainEvents();
        }
        var result = await _context.SaveChangesAsync();
        return result;
    }
}

