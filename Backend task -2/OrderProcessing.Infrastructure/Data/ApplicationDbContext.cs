
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

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.OrderId);
            entity.Property(o=>o.State).IsRequired();
            entity.Property(o=>o.IsSubmitted).IsRequired();
            entity.OwnsOne(o => o.Total);



        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi=>new { oi.OrderId, oi.OrderItemId });
            entity.HasOne<Product>().WithOne();
            entity.Property(oi=>oi.ProductId).IsRequired();
            entity.Property(oi => oi.Quantity).IsRequired();
            entity.OwnsOne(oi => oi.Price);

        });
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p=>p.ProductId);
            entity.Property(p=>p.Name).IsRequired();
            entity.Property(p=>p.Photo).IsRequired();
            entity.OwnsOne(p => p.CurrentPrice);
        });
    }
}