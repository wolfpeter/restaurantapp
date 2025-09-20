using RestaurantApp.Models.Enums;

namespace RestaurantApp.Models.DTOs;

public class OrderDetailsDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public int RestaurantId { get; set; }
    public string RestaurantName { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItemDetailsDto> Items { get; set; }
}

public class OrderItemDetailsDto
{
    public string MenuItemName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? SpecialInstructions { get; set; }
}