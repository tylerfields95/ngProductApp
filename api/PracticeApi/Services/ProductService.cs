using Microsoft.EntityFrameworkCore;
using PracticeApi.Data;
using PracticeApi.Data.Dtos;
using PracticeApi.Data.Models;

public interface IProductService
{
    Task<PaginatedResult<ProductDto>> GetAllActiveAsync(int page, int pageSize);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(ProductDto productDto);
    Task<ProductDto?> UpdateAsync(ProductDto productDto);
    Task<bool> SoftDeleteAsync(int id);
    Task<PaginatedResult<ProductDto>> SearchAsync(
        string? searchTerm,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStock,
        string? sortBy,
        string? sortOrder,
        int page,
        int pageSize);
}

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<ProductDto>> GetAllActiveAsync(int page = 1, int pageSize = 50)
    {
        // Start with base query - no Include yet for better count performance
        var query = _context.Products.Where(p => p.IsActive);

        // Count without loading related data
        var totalCount = await query.CountAsync();

        // Now include Category only for the actual page of results
        var items = await query
            .OrderByDescending(p => p.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(p => p.Category)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                Category = new CategoryDto
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name,
                    Description = p.Category.Description,
                    IsActive = p.Category.IsActive
                },
                StockQuantity = p.StockQuantity,
                CreatedDate = p.CreatedDate,
                IsActive = p.IsActive
            })
            .ToListAsync();

        return new PaginatedResult<ProductDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Id == id && p.IsActive)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                Category = new CategoryDto
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name,
                    Description = p.Category.Description,
                    IsActive = p.Category.IsActive
                },
                StockQuantity = p.StockQuantity,
                CreatedDate = p.CreatedDate,
                IsActive = p.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProductDto> CreateAsync(ProductDto productDto)
    {
        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            CategoryId = productDto.CategoryId,
            StockQuantity = productDto.StockQuantity,
            IsActive = true
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Load the category to return with the product
        var category = await _context.Categories.FindAsync(product.CategoryId);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            Category = category != null ? new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive
            } : null,
            StockQuantity = product.StockQuantity,
            CreatedDate = product.CreatedDate,
            IsActive = product.IsActive
        };
    }

    public async Task<ProductDto?> UpdateAsync(ProductDto productDto)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productDto.Id && p.IsActive);

        if (product == null)
            return null;

        product.Name = productDto.Name;
        product.Description = productDto.Description;
        product.Price = productDto.Price;
        product.CategoryId = productDto.CategoryId;
        product.StockQuantity = productDto.StockQuantity;

        await _context.SaveChangesAsync();

        // Load the category to return with the product
        var category = await _context.Categories.FindAsync(product.CategoryId);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            Category = category != null ? new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive
            } : null,
            StockQuantity = product.StockQuantity,
            CreatedDate = product.CreatedDate,
            IsActive = product.IsActive
        };
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (product == null)
            return false;

        product.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<PaginatedResult<ProductDto>> SearchAsync(
        string? searchTerm,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStock,
        string? sortBy,
        string? sortOrder,
        int page = 1,
        int pageSize = 50)
    {
        // Start with base query - no Include yet for better count performance
        var query = _context.Products.Where(p => p.IsActive);

        // Search term filter - using EF.Functions.ILike for case-insensitive search with index support
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchWords = searchTerm.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in searchWords)
            {
                var searchPattern = $"%{word}%";
                query = query.Where(p =>
                    EF.Functions.ILike(p.Name, searchPattern) ||
                    (p.Description != null && EF.Functions.ILike(p.Description, searchPattern)));
            }
        }

        // Category filter - uses IX_Products_CategoryId index
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        // Price range filters - can use IX_Products_CategoryId_IsActive_Price composite index
        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        // In stock filter - uses IX_Products_StockQuantity index
        if (inStock.HasValue)
        {
            if (inStock.Value)
            {
                query = query.Where(p => p.StockQuantity > 0);
            }
            else
            {
                query = query.Where(p => p.StockQuantity == 0);
            }
        }

        // Sorting - all sortable fields have indexes
        var sortOrderLower = sortOrder?.ToLower() ?? "asc";
        var isDescending = sortOrderLower == "desc";

        query = (sortBy?.ToLower()) switch
        {
            "name" => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "price" => isDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            "createddate" => isDescending ? query.OrderByDescending(p => p.CreatedDate) : query.OrderBy(p => p.CreatedDate),
            "stockquantity" => isDescending ? query.OrderByDescending(p => p.StockQuantity) : query.OrderBy(p => p.StockQuantity),
            _ => query.OrderByDescending(p => p.CreatedDate) // Default sort - uses IX_Products_IsActive_CreatedDate
        };

        // Count without loading related data
        var totalCount = await query.CountAsync();

        // Now include Category only for the actual page of results
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(p => p.Category)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                Category = new CategoryDto
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name,
                    Description = p.Category.Description,
                    IsActive = p.Category.IsActive
                },
                StockQuantity = p.StockQuantity,
                CreatedDate = p.CreatedDate,
                IsActive = p.IsActive
            })
            .ToListAsync();

        return new PaginatedResult<ProductDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
