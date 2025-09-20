using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Models.DTOs;

public class OrderItemRequest
{
    [Required]
    public int MenuItemId { get; set; }

    [Required]
    [Range(1, 100)]
    public int Quantity { get; set; }

    public string? SpecialInstructions { get; set; }
}