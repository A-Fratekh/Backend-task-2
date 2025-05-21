
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Domain.Aggregates.OrderAggregate;
using OrderProcessing.Domain.Aggregates.ProductAggregate;

namespace OrderProcessing.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)
    {
    }

    public DbSet<Order> Departments { get; set; }
    public DbSet<OrderItem> Occupations { get; set; }
    public DbSet<Product> Grades { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o=>o.State).IsRequired();
            entity.Property(o=>o.IsSubmitted).IsRequired();
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi=>oi.OrderItemId);
            entity.Property(oi=>oi.ProductId).IsRequired();
            entity.Property(oi => oi.Quantity).IsRequired();

        });
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p=>p.Id);
            entity.Property(p=>p.Name).IsRequired();
            entity.Property(p=>p.Photo).IsRequired();
            entity.HasMany<Order>().WithOne();
        });
    }
}