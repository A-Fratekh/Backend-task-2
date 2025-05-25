
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies(); 
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Id).ValueGeneratedNever();
            entity.Property(o=>o.State).IsRequired();
            entity.Property(o => o.Date).IsRequired();
            entity.Property(o => o.CustomerName).IsRequired().HasMaxLength(100);
            entity.OwnsOne(o => o.Total);
            entity.HasMany(o => o.OrderItems)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi=>new { oi.OrderId, oi.OrderItemId });
            entity.HasOne<Product>().WithOne();
            entity.Property(oi=>oi.ProductId).IsRequired().ValueGeneratedNever();
            entity.Property(oi => oi.OrderId).IsRequired().ValueGeneratedNever();
            entity.Property(oi => oi.OrderItemId).IsRequired().ValueGeneratedNever();
            entity.Property(oi => oi.Quantity).IsRequired();
            entity.OwnsOne(oi => oi.Price);

        });
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p=>p.Id);
            entity.Property(p => p.Id).ValueGeneratedNever();
            entity.Property(p=>p.Name).IsRequired();
            entity.Property(p=>p.Photo).IsRequired();
            entity.OwnsOne(p => p.CurrentPrice).HasData(
                new { ProductId = 1, Amount = 2.6m},
                new { ProductId = 2, Amount = 1.5m },
                new { ProductId = 3, Amount = 2.8m },
                new { ProductId = 4, Amount = 3.5m }
                );
            entity.HasData(
                new { Id = 1,  Name = "Sahwerma Meal", Description = "6 Shawerma Pcs", Photo = new byte[] { } },
                new { Id = 2, Name = "Zinger Sandwich", Description = "Zinger wrap", Photo = new byte[] { } },
                new { Id = 3, Name = "Super Sahwerma Meal", Description = "9 Shawerma Pcs", Photo = new byte[] { } },
                new { Id = 4, Name = "Italian Sahwerma Meal", Description = "Italian Shawerma", Photo = new byte[] { } }
                 );

    });
    }
}