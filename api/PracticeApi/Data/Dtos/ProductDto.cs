using System.ComponentModel.DataAnnotations;

namespace PracticeApi.Data.Dtos;

public class ProductDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required")]
    [MaxLength(300, ErrorMessage = "Product name cannot exceed 300 characters")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Category ID is required")]
    public int CategoryId { get; set; }

    public CategoryDto? Category { get; set; }

    [Required(ErrorMessage = "Stock quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater")]
    public int StockQuantity { get; set; }

    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}
