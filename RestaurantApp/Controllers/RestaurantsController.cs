using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Models.DTOs;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers;

[ApiController]
[Route("restaurants")]
[Authorize]
public class RestaurantsController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantsController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    /// <summary>
    /// Returns a list of all restaurants.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RestaurantShortDto>))]
    public async Task<IActionResult> GetRestaurants()
    {
        var restaurants = await _restaurantService.GetAllRestaurantsAsync();
        return Ok(restaurants);
    }

    /// <summary>
    /// Returns the details of the restaurant with the specified ID, including its menu.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RestaurantDetailsDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRestaurantById(int id)
    {
        var restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
        return Ok(restaurant);
    }

    /// <summary>
    /// Returns the menu of the restaurant with the specified ID.
    /// </summary>
    [HttpGet("{id}/menu")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MenuItemDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRestaurantMenu(int id)
    {
        var restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
        return Ok(restaurant.Menu);
    }
    
    /// <summary>
    /// Returns a list of all orders placed with a specific restaurant. (For restaurant owners)
    /// </summary>
    /// <param name="restaurantId">The ID of the restaurant.</param>
    [HttpGet("{restaurantId}/orders")]
    [Authorize(Roles = "Restaurant")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrderDetailsDto>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrdersForRestaurant(int restaurantId)
    {
        var orders = await _restaurantService.GetOrdersForRestaurantAsync(restaurantId);
        return Ok(orders);
    }
}