using Microsoft.EntityFrameworkCore;
using PracticeApi.Data.Models;

namespace PracticeApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            // INDEX: Fast lookups by IsActive (common filter)
            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_Categories_IsActive");

            // INDEX: Fast lookups by Name (common for searches and uniqueness)
            entity.HasIndex(e => e.Name)
                .HasDatabaseName("IX_Categories_Name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(300);

            entity.Property(e => e.Description)
                .HasMaxLength(2000);

            entity.Property(e => e.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Configure FK relationship
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // INDEX 1: CategoryId (FK) - CRITICAL for JOIN performance
            // This is the most important index for millions of products
            entity.HasIndex(e => e.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId");

            // INDEX 2: IsActive - Common filter for "active products only" queries
            entity.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_Products_IsActive");

            // INDEX 3: Composite index (CategoryId, IsActive, Price)
            // Optimized for: "Get active products by category ordered by price"
            // Covers the most common query pattern efficiently
            entity.HasIndex(e => new { e.CategoryId, e.IsActive, e.Price })
                .HasDatabaseName("IX_Products_CategoryId_IsActive_Price");

            // INDEX 4: CreatedDate - For "newest products" queries and time-based filtering
            entity.HasIndex(e => e.CreatedDate)
                .HasDatabaseName("IX_Products_CreatedDate")
                .IsDescending(); // Descending for "newest first" queries

            // INDEX 5: Name - For text search and autocomplete
            entity.HasIndex(e => e.Name)
                .HasDatabaseName("IX_Products_Name");

            // INDEX 6: Composite (IsActive, CreatedDate)
            // Optimized for: "Get all active products sorted by date"
            entity.HasIndex(e => new { e.IsActive, e.CreatedDate })
                .HasDatabaseName("IX_Products_IsActive_CreatedDate")
                .IsDescending(false, true); // IsActive ASC, CreatedDate DESC

            // INDEX 7: StockQuantity - For inventory management queries
            // Useful for "low stock alerts" and availability checks
            entity.HasIndex(e => e.StockQuantity)
                .HasDatabaseName("IX_Products_StockQuantity");
        });
    }
}
