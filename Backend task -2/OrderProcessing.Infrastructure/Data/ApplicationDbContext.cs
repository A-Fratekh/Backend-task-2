
using Microsoft.EntityFrameworkCore;
using OrderProcessing.Domain.Aggregates.OrderAggregate;
using OrderProcessing.Domain.Aggregates.ProductAggregate;
using OrderProcessing.Domain.Shared;

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
            entity.Property(o => o.OrderId).ValueGeneratedNever();
            entity.Property(o=>o.State).IsRequired();
            entity.Property(o=>o.IsSubmitted).IsRequired();
            entity.OwnsOne(o => o.Total);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi=>new { oi.OrderId, oi.OrderItemId });
            entity.Property(oi => oi.OrderId).ValueGeneratedNever();
            entity.Property(oi => oi.OrderItemId).ValueGeneratedNever();
            entity.HasOne<Product>().WithOne();
            entity.Property(oi=>oi.ProductId).IsRequired();
            entity.Property(oi => oi.Quantity).IsRequired();
            entity.OwnsOne(oi => oi.Price);

        });
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p=>p.ProductId);
            entity.Property(p => p.ProductId).ValueGeneratedNever();
            entity.Property(p=>p.Name).IsRequired();
            entity.Property(p=>p.Photo).IsRequired();
            entity.OwnsOne(p => p.CurrentPrice).HasData(
                new {ProductId =1, Amount = 2.6m},
                new { ProductId = 2, Amount = 1.5m },
                new { ProductId = 3, Amount = 2.8m },
                new { ProductId = 4, Amount = 3.5m }
                );
            entity.HasData(
                new {ProductId = 1,  Name = "Sahwerma Meal", Description = "6 Shawerma Pcs", Photo = new byte[] { } },
                new { ProductId = 2, Name = "Zinger Sandwich", Description = "Zinger wrap", Photo = new byte[] { } },
                new { ProductId = 3, Name = "Super Sahwerma Meal", Description = "9 Shawerma Pcs", Photo = new byte[] { } },
                new { ProductId = 4, Name = "Italian Sahwerma Meal", Description = "Italian Shawerma", Photo = new byte[] { } }
                 );

    });
    }
}