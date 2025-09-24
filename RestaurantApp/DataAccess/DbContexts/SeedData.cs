using Microsoft.EntityFrameworkCore;
using RestaurantApp.Models.Entities;
using RestaurantApp.Models.Enums;

namespace RestaurantApp.DataAccess.DbContexts;

public static class SeedData
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        var restaurant = new Restaurant
        {
            Id = 1,
            Created = DateTime.Parse("2025-01-01"),
            Address = "6722 Szeged, Indóház tér 1",
            Name = "Vasút étterem",
            Email = "info@vasutetterem.hu",
            PhoneNumber = "0662123456",
            Description = "Teszt étterem"
        };
        
        modelBuilder.Entity<Restaurant>().HasData(restaurant);

        var menuItem1 = new MenuItem
        {
            Id = 1,
            Created = DateTime.Parse("2025-01-01"),
            Category = "Levesek",
            Name = "Gyümölcsleves",
            Description = "",
            IsAvailable = true,
            Price = 100,
            RestaurantId = restaurant.Id
        };

        var menuItem2 = new MenuItem
        {
            Id = 2,
            Created = DateTime.Parse("2025-01-01"),
            Category = "Előételek",
            Name = "Rántott sajt",
            IsAvailable = true,
            Price = 100,
            RestaurantId = restaurant.Id
        };

        var menuItem3 = new MenuItem
        {
            Id = 3,
            Created = DateTime.Parse("2025-01-01"),
            Category = "Köretek",
            Name = "Rizs",
            IsAvailable = true,
            Price = 100,
            RestaurantId = restaurant.Id
        };
        
        modelBuilder.Entity<MenuItem>().HasData(menuItem1, menuItem2, menuItem3);

        var user = new User
        {
            Id = 1,
            Created = DateTime.Parse("2025-01-01"),
            FirstName = "John",
            LastName = "Doe",
            Email = "manager@vasutetterem.hu",
            PhoneNumber = "0662123456",
            PasswordHash = "$2a$11$VoJtOj.3CALx84THrqdpruADp9ldqh16.RjH/pkbbOtdHOCk1N4gG", // Almafa01
            RestaurantId = 1,
            Role = Role.Restaurant
        };
        
        modelBuilder.Entity<User>().HasData(user);
    }
}