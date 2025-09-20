using RestaurantApp.Models.Enums;

namespace RestaurantApp.Models.Entities;

public class User : EntityBase
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Role Role { get; set; }
    public int? RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}