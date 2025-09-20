using System.Data.Common;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantApp.DataAccess.DbContexts;
using RestaurantApp.Hubs;
using RestaurantApp.Models.DTOs;
using RestaurantApp.Models.Entities;
using RestaurantApp.Models.Enums;
using RestaurantApp.Services;
using RestaurantApp.Utils;

namespace RestaurantApp.Tests;

public class OrderServiceTests : IDisposable
{
    private readonly IMapper _mapper;
    private readonly DbContextOptions<RestaurantAppDbContext> _dbContextOptions;
    private readonly DbConnection _connection;
    private readonly Mock<IHubContext<OrderHub>> _hubContextMock;

    public OrderServiceTests()
    {
        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
        _mapper = mappingConfig.CreateMapper();

        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _dbContextOptions = new DbContextOptionsBuilder<RestaurantAppDbContext>()
            .UseSqlite(_connection)
            .Options;
            
        _hubContextMock = new Mock<IHubContext<OrderHub>>(); 
        _hubContextMock.Setup(x => x.Clients.All.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
            .Returns(Task.CompletedTask);
        _hubContextMock.Setup(x => x.Clients.Group(It.IsAny<string>()).SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
            .Returns(Task.CompletedTask);
        
        using var context = new RestaurantAppDbContext(_dbContextOptions);
        context.Database.EnsureCreated();
    }
    
    private async Task SeedDatabase(RestaurantAppDbContext context)
    {
        var user = new User 
        { 
            Id = 1, 
            Address = "Fake Address", 
            Email="test@test.com", 
            FirstName ="Test", 
            LastName="User", 
            PasswordHash = "..." 
        };
        
        var restaurant = new Restaurant { Id = 1, Name = "Test Restaurant" };
        var menuItems = new List<MenuItem>
        {
            new() { Id = 1, Name = "Pizza", Price = 10, RestaurantId = 1 },
            new() { Id = 2, Name = "Pasta", Price = 8, RestaurantId = 1 }
        };

        await context.Users.AddAsync(user);
        await context.Restaurants.AddAsync(restaurant);
        await context.MenuItems.AddRangeAsync(menuItems);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task PlaceOrderAsync_WithValidData_ShouldCreateOrderSuccessfully()
    {
        // ARRANGE
        await using var context = new RestaurantAppDbContext(_dbContextOptions);
        await SeedDatabase(context);
        
        var orderService = new OrderService(context, _mapper, _hubContextMock.Object);
        
        var createOrderRequest = new CreateOrderRequest
        {
            RestaurantId = 1,
            DeliveryAddress = "Fake Address",
            Items = new List<OrderItemRequest>
            {
                new() { MenuItemId = 1, Quantity = 2 },
                new() { MenuItemId = 2, Quantity = 1 }
            }
        };

        // ACT
        var result = await orderService.PlaceOrderAsync(1, createOrderRequest);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(28, result.TotalPrice);
        Assert.Equal(OrderStatus.Received, result.Status);
        Assert.Equal(2, result.Items.Count);

        var orderInDb = await context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync();
        Assert.NotNull(orderInDb);
        Assert.Equal(28, orderInDb.TotalPrice);
    }
    
    [Fact]
    public async Task PlaceOrderAsync_WithInvalidRestaurantId_ShouldThrowArgumentException()
    {
        // ARRANGE
        await using var context = new RestaurantAppDbContext(_dbContextOptions);
        await SeedDatabase(context);
        
        var orderService = new OrderService(context, _mapper, _hubContextMock.Object);
        
        var createOrderRequest = new CreateOrderRequest
        {
            RestaurantId = 99,
            Items = new List<OrderItemRequest>
            {
                new() { MenuItemId = 1, Quantity = 1 }
            }
        };

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            orderService.PlaceOrderAsync(1, createOrderRequest));
            
        Assert.Equal("Restaurant with ID 99 not found.", exception.Message);
    }

    [Fact]
    public async Task PlaceOrderAsync_WithInvalidMenuItemId_ShouldThrowArgumentException()
    {
        // ARRANGE
        await using var context = new RestaurantAppDbContext(_dbContextOptions);
        await SeedDatabase(context);
        
        var orderService = new OrderService(context, _mapper, _hubContextMock.Object);
        
        var createOrderRequest = new CreateOrderRequest
        {
            RestaurantId = 1,
            Items = new List<OrderItemRequest>
            {
                new() { MenuItemId = 99, Quantity = 1 }
            }
        };

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            orderService.PlaceOrderAsync(1, createOrderRequest));
            
        Assert.Equal("One or more menu items are invalid or do not belong to this restaurant.", exception.Message);
    }
    
    public void Dispose()
    {
        _connection.Dispose();
    }
}