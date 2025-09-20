using RestaurantApp.Models.DTOs;
using RestaurantApp.Models.Enums;

namespace RestaurantApp.Services;

public interface IOrderService
{
    Task<OrderDetailsDto> PlaceOrderAsync(int customerId, CreateOrderRequest request);
    Task<OrderDetailsDto> GetOrderByIdAsync(int orderId);
    Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
}