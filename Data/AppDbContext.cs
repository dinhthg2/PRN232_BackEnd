using Microsoft.EntityFrameworkCore;
using PRN232.Backend.Models;

namespace PRN232.Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderProduct> OrderProducts => Set<OrderProduct>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderProduct>().HasKey(op => new { op.OrderId, op.ProductId });

        // Add unique constraint for User Email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Classic White T-Shirt", Description = "Premium cotton t-shirt with a classic fit.", Price = 29.99m, Image = "", Category = "T-Shirts", Stock = 50 },
            new Product { Id = 2, Name = "Slim Fit Jeans", Description = "Modern slim fit jeans with stretch fabric.", Price = 79.99m, Image = "", Category = "Jeans", Stock = 30 },
            new Product { Id = 3, Name = "Leather Jacket", Description = "Genuine leather jacket with premium finish.", Price = 299.99m, Image = "", Category = "Jackets", Stock = 15 }
        );
    }
}
