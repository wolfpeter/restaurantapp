using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Models.DTOs;

public class CreateOrderRequest
{
    [Required]
    public int RestaurantId { get; set; }

    [Required]
    public string DeliveryAddress { get; set; }
    
    [Required]
    [MinLength(1)]
    public List<OrderItemRequest> Items { get; set; }
}