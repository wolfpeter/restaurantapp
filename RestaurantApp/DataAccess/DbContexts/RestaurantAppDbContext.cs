using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantApp.Models.Entities;
using RestaurantApp.Models.Enums;

namespace RestaurantApp.DataAccess.DbContexts;

public class RestaurantAppDbContext : DbContext
{
    public RestaurantAppDbContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurant>()
            .HasIndex(r => r.Name)
            .IsUnique()
            .HasFilter("Deleted IS NULL");
        
        modelBuilder.Entity<User>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId);
        
        modelBuilder.Entity<User>()
            .HasIndex(c => c.Email)
            .IsUnique()
            .HasFilter("Deleted IS NULL");
        
        modelBuilder.Entity<User>()
            .Property(s => s.Role)
            .HasConversion<string>();
        
        modelBuilder.Entity<Restaurant>()
            .HasMany(r => r.Orders)
            .WithOne(o => o.Restaurant)
            .HasForeignKey(o => o.RestaurantId);
        
        modelBuilder.Entity<Restaurant>()
            .HasMany(r => r.MenuItems)
            .WithOne(m => m.Restaurant)
            .HasForeignKey(m => m.RestaurantId);
        
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId);
        
        modelBuilder.Entity<Order>()
            .Property(s => s.Status)
            .HasConversion<string>();
        
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.MenuItem)
            .WithMany()
            .HasForeignKey(oi => oi.MenuItemId);
        
        modelBuilder.Seed();
        
        base.OnModelCreating(modelBuilder);
    }
}