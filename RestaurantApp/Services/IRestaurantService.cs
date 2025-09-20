using RestaurantApp.Models.DTOs;

namespace RestaurantApp.Services;

public interface IRestaurantService
{
    Task<IEnumerable<RestaurantShortDto>> GetAllRestaurantsAsync();
    Task<RestaurantDetailsDto> GetRestaurantByIdAsync(int id);
    Task<IEnumerable<OrderDetailsDto>> GetOrdersForRestaurantAsync(int restaurantId);
}