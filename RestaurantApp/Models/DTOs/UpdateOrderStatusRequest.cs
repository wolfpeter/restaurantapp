using System.ComponentModel.DataAnnotations;
using RestaurantApp.Models.Enums;

namespace RestaurantApp.Models.DTOs;

public class UpdateOrderStatusRequest
{
    [Required]
    public OrderStatus Status { get; set; }
}