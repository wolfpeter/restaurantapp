using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.DataAccess.DbContexts;
using RestaurantApp.Models.DTOs;

namespace RestaurantApp.Services;

public class RestaurantService : IRestaurantService
{
    private readonly RestaurantAppDbContext _dbContext;
    private readonly IMapper _mapper;

    public RestaurantService(RestaurantAppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RestaurantShortDto>> GetAllRestaurantsAsync()
    {
        var restaurants = await _dbContext.Restaurants
            .Where(r => r.Deleted == null)
            .ToListAsync();
            
        return _mapper.Map<IEnumerable<RestaurantShortDto>>(restaurants);
    }

    public async Task<RestaurantDetailsDto> GetRestaurantByIdAsync(int id)
    {
        var restaurant = await _dbContext.Restaurants
            .Include(r => r.MenuItems)
            .FirstOrDefaultAsync(r => r.Id == id && r.Deleted == null);

        if (restaurant == null)
        {
            throw new KeyNotFoundException($"Restaurant with ID {id} not found.");
        }

        return _mapper.Map<RestaurantDetailsDto>(restaurant);
    }
    
    public async Task<IEnumerable<OrderDetailsDto>> GetOrdersForRestaurantAsync(int restaurantId)
    {
        var orders = await _dbContext.Orders
            .Where(o => o.RestaurantId == restaurantId)
            .Include(o => o.User)
            .Include(o => o.Restaurant)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.MenuItem)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
            
        return _mapper.Map<IEnumerable<OrderDetailsDto>>(orders);
    }
}