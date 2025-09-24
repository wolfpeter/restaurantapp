using System.Data.Common;
using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RestaurantApp.DataAccess.DbContexts;
using RestaurantApp.Models.DTOs;
using RestaurantApp.Models.Entities;
using RestaurantApp.Services;
using RestaurantApp.Utils.Mapping;

namespace RestaurantApp.Tests;

public class AuthServiceTests : IDisposable
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<RestaurantAppDbContext> _dbContextOptions;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthServiceTests()
    {
        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
        _mapper = mappingConfig.CreateMapper();

        var inMemorySettings = new Dictionary<string, string> {
            {"Jwt:Key", "ThisIsAValidSuperSecretKeyForTesting123456789"},
            {"Jwt:Issuer", "TestIssuer"},
            {"Jwt:Audience", "TestAudience"},
            {"Jwt:ExpiryMinutes", "60"},
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        _dbContextOptions = new DbContextOptionsBuilder<RestaurantAppDbContext>()
            .UseSqlite(_connection)
            .Options;
        using var context = new RestaurantAppDbContext(_dbContextOptions);
        context.Database.EnsureCreated();
    }

    [Fact]
    public async Task RegisterAsync_WithNewEmail_ShouldSucceedAndReturnToken()
    {
        // ARRANGE
        await using var context = new RestaurantAppDbContext(_dbContextOptions);
        var authService = new AuthService(context, _configuration, _mapper);
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123",
            FirstName = "Test",
            LastName = "User"
        };

        // ACT
        var response = await authService.RegisterAsync(request);

        // ASSERT
        Assert.NotNull(response);
        Assert.False(string.IsNullOrEmpty(response.Token));
        Assert.Equal("Registration successful!", response.Message);

        var userInDb = await context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
        Assert.NotNull(userInDb);
        Assert.Equal("Test", userInDb.FirstName);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldThrowArgumentException()
    {
        // ARRANGE
        await using var context = new RestaurantAppDbContext(_dbContextOptions);
        context.Users.Add(new User { Email = "test@example.com", PasswordHash = "..." });
        await context.SaveChangesAsync();
        
        var authService = new AuthService(context, _configuration, _mapper);
        var request = new RegisterRequest { Email = "test@example.com", Password = "..." };

        // ACT & ASSERT
        await Assert.ThrowsAsync<ArgumentException>(() => authService.RegisterAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        // ARRANGE
        await using var context = new RestaurantAppDbContext(_dbContextOptions);
        var password = "Password123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        context.Users.Add(new User { Email = "test@example.com", PasswordHash = hashedPassword });
        await context.SaveChangesAsync();
        
        var authService = new AuthService(context, _configuration, _mapper);
        var request = new LoginRequest { Email = "test@example.com", Password = password };

        // ACT
        var response = await authService.LoginAsync(request);

        // ASSERT
        Assert.NotNull(response);
        Assert.False(string.IsNullOrEmpty(response.Token));
        Assert.Equal("Login successful!", response.Message);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowUnauthorizedAccessException()
    {
        // ARRANGE
        await using var context = new RestaurantAppDbContext(_dbContextOptions);
        context.Users.Add(new User { Email = "test@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword") });
        await context.SaveChangesAsync();
        
        var authService = new AuthService(context, _configuration, _mapper);
        var request = new LoginRequest { Email = "test@example.com", Password = "WrongPassword" };

        // ACT & ASSERT
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => authService.LoginAsync(request));
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}