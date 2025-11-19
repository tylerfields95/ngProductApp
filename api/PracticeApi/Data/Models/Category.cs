using System.ComponentModel.DataAnnotations;

namespace PracticeApi.Data.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation property
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
