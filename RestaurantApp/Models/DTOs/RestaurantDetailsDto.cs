namespace RestaurantApp.Models.DTOs;

public class RestaurantDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<MenuItemDto> Menu { get; set; } = new();
}