using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Models.Entities;

public abstract class EntityBase
{
    [Key]
    public int Id { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Deleted { get; set; }
}