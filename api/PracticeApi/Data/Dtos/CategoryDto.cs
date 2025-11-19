using System.ComponentModel.DataAnnotations;

namespace PracticeApi.Data.Dtos;

public class CategoryDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Category name is required")]
    [MaxLength(200, ErrorMessage = "Category name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}
