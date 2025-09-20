﻿namespace RestaurantApp.Models.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = default!;

    public int MenuItemId { get; set; }
    public MenuItem MenuItem { get; set; } = default!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}