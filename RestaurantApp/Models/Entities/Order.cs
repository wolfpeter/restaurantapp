using RestaurantApp.Models.Enums;

namespace RestaurantApp.Models.Entities;

public class Order : EntityBase
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Received;
    public decimal TotalPrice { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}