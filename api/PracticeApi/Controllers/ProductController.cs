using Microsoft.AspNetCore.Mvc;
using PracticeApi.Data.Dtos;

namespace PracticeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get all active products with pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<ProductDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var products = await _productService.GetAllActiveAsync(page, pageSize);
        return Ok(products);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] ProductDto productDto)
    {
        var createdProduct = await _productService.CreateAsync(productDto);
        return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut]
    public async Task<ActionResult<ProductDto>> Update([FromBody] ProductDto productDto)
    {
        var updatedProduct = await _productService.UpdateAsync(productDto);

        if (updatedProduct == null)
            return NotFound();

        return Ok(updatedProduct);
    }

    /// <summary>
    /// Soft delete a product
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _productService.SoftDeleteAsync(id);

        if (!result)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Search products with optional filters
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<PaginatedResult<ProductDto>>> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? inStock,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var products = await _productService.SearchAsync(
            searchTerm,
            categoryId,
            minPrice,
            maxPrice,
            inStock,
            sortBy,
            sortOrder,
            page,
            pageSize);

        return Ok(products);
    }
}
