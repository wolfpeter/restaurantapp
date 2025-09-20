using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Models.DTOs;
using RestaurantApp.Services;

namespace RestaurantApp.Controllers;

[ApiController]
[Route("orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Allows a customer to place an order.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "User")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OrderDetailsDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var customerId))
        {
            return Unauthorized("Invalid user identifier.");
        }
        
        var newOrder = await _orderService.PlaceOrderAsync(customerId, request);
        return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.Id }, newOrder);
    }
    
    /// <summary>
    /// Returns the details of the order with the specified ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDetailsDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        return Ok(order);
    }
    
    /// <summary>
    /// Updates the status of the order with the specified ID. (For restaurant owners)
    /// </summary>
    [HttpPatch("{id}")]
    [Authorize(Roles = "Restaurant")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        var success = await _orderService.UpdateOrderStatusAsync(id, request.Status);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }
}