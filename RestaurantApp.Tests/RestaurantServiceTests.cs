using System.Data.Common;
using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.DataAccess.DbContexts;
using RestaurantApp.Models.Entities;
using RestaurantApp.Services;
using RestaurantApp.Utils;

namespace RestaurantApp.Tests;

public class RestaurantServiceTests : IDisposable
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<RestaurantAppDbContext> _dbContextOptions;
    private readonly IMapper _mapper;

    public RestaurantServiceTests()
    {
        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
        _mapper = mappingConfig.CreateMapper();

        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        _dbContextOptions = new DbContextOptionsBuilder<RestaurantAppDbContext>()
            .UseSqlite(_connection)
            .Options;
        using var context = new RestaurantAppDbContext(_dbContextOptions);
        context.Database.EnsureCreated();
    }

    private async Task SeedDatabase(RestaurantAppDbContext context)
    {
        var restaurants = new List<Restaurant>
        {
            new() { Id = 1, Name = "Restaurant A", Address = "Address A" },
            new() { Id = 2, Name = "Restaurant B", Address = "Address B" }
        };
        await context.Restaurants.AddRangeAsync(restaurants);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllRestaurantsAsync_ShouldReturnAllRestaurants()
    {
        // ARRANGE
        await using var context = new RestaurantAppDbContext(_dbContextOptions);
        await SeedDatabase(context);
        var service = new RestaurantService(context, _mapper);
        
        // ACT
        var result = (await service.GetAllRestaurantsAsync()).ToList();

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Restaurant A", result[0].Name);
    }

    [Fact]
    public async Task GetRestaurantByIdAsync_WithValidId_ShouldReturnDetails()
    {
        // ARRANGE
        await using var context = new RestaurantAppDbContext(_dbContextOptions);
        await SeedDatabase(context);
        context.MenuItems.Add(new MenuItem { Id = 1, Name = "Pizza", Price = 10, RestaurantId = 1 });
        await context.SaveChangesAsync();

        var service = new RestaurantService(context, _mapper);

        // ACT
        var result = await service.GetRestaurantByIdAsync(1);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal("Restaurant A", result.Name);
        Assert.Single(result.Menu);
        Assert.Equal("Pizza", result.Menu[0].Name);
    }

    [Fact]
    public async Task GetRestaurantByIdAsync_WithInvalidId_ShouldThrowKeyNotFoundException()
    {
        // ARRANGE
        await using var context = new RestaurantAppDbContext(_dbContextOptions);
        await SeedDatabase(context);
        var service = new RestaurantService(context, _mapper);
        
        // Act & ASSERT
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetRestaurantByIdAsync(99));
    }
    
    public void Dispose()
    {
        _connection.Dispose();
    }
}