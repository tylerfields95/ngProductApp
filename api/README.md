# PracticeApi - Database Indexing Strategy

## Schema Overview

### Tables
- **Categories**: Limited number of categories (hundreds to low thousands)
- **Products**: Millions of products, each associated with one category

### Relationships
- **Product → Category**: Many-to-One relationship (Foreign Key: `CategoryId`)
- Delete behavior: `Restrict` (prevents deletion of categories with products)

---

## Indexing Strategy

### Design Principles
This indexing strategy is optimized for **millions of products** with the following assumptions:
1. **Read-heavy workload**: More queries than inserts/updates
2. **Common query patterns**: Category filtering, active products, price sorting, date ranges
3. **PostgreSQL optimization**: Leveraging B-tree indexes for range queries and composite indexes for complex filters

### Trade-offs
- **Pros**: Lightning-fast query performance for common patterns
- **Cons**: Increased storage (~10-15% overhead) and slightly slower writes
- **Decision**: For millions of products, read performance is critical, making the trade-off worthwhile

---

## Category Table Indexes

### 1. Primary Key (Auto-generated)
```sql
PK_Categories (Id)
```
- Default clustered index for primary key lookups

### 2. `IX_Categories_IsActive`
```sql
CREATE INDEX IX_Categories_IsActive ON Categories (IsActive);
```
**Purpose**: Filter active/inactive categories
**Query Pattern**: `WHERE IsActive = true`
**Performance Impact**: O(log n) → O(1) for boolean filtering

### 3. `IX_Categories_Name`
```sql
CREATE INDEX IX_Categories_Name ON Categories (Name);
```
**Purpose**: Category name searches and uniqueness validation
**Query Pattern**: `WHERE Name LIKE 'Electronics%'`
**Performance Impact**: Enables fast text prefix searches

---

## Product Table Indexes

### 1. Primary Key (Auto-generated)
```sql
PK_Products (Id)
```
- Default clustered index for primary key lookups

### 2. `IX_Products_CategoryId` ⚡ **CRITICAL**
```sql
CREATE INDEX IX_Products_CategoryId ON Products (CategoryId);
```
**Purpose**: Foreign key index for JOIN operations
**Query Pattern**: `JOIN Categories ON Products.CategoryId = Categories.Id`
**Performance Impact**: Without this index, JOINs on millions of products would cause full table scans (O(n) → O(log n))
**Cardinality**: Moderate (hundreds to thousands of distinct categories)

### 3. `IX_Products_IsActive`
```sql
CREATE INDEX IX_Products_IsActive ON Products (IsActive);
```
**Purpose**: Filter active products
**Query Pattern**: `WHERE IsActive = true`
**Use Case**: Most applications only show active products on the front end
**Performance Impact**: Dramatically reduces result set for filtering

### 4. `IX_Products_CategoryId_IsActive_Price` ⚡ **COMPOSITE - HIGH IMPACT**
```sql
CREATE INDEX IX_Products_CategoryId_IsActive_Price
ON Products (CategoryId, IsActive, Price);
```
**Purpose**: Covering index for the most common query pattern
**Query Pattern**:
```sql
SELECT * FROM Products
WHERE CategoryId = 5 AND IsActive = true
ORDER BY Price ASC;
```
**Performance Impact**:
- Covers 3 columns in a single index (no table lookup needed)
- Enables "index-only scans" in PostgreSQL
- Perfect for e-commerce product listing pages

**Why this order?**
1. `CategoryId` (highest selectivity) - Filters to ~thousands of products
2. `IsActive` (boolean filter) - Further reduces to active subset
3. `Price` (sort key) - Already ordered, no additional sort needed

### 5. `IX_Products_CreatedDate` (Descending)
```sql
CREATE INDEX IX_Products_CreatedDate ON Products (CreatedDate DESC);
```
**Purpose**: "Newest products" queries and date range filtering
**Query Pattern**:
```sql
SELECT * FROM Products
ORDER BY CreatedDate DESC
LIMIT 50;
```
**Performance Impact**: Descending index means "newest first" queries use the index efficiently
**Use Case**: "New arrivals", "Recently added", date-based pagination

### 6. `IX_Products_Name`
```sql
CREATE INDEX IX_Products_Name ON Products (Name);
```
**Purpose**: Product name searches, autocomplete, and text filtering
**Query Pattern**:
```sql
SELECT * FROM Products
WHERE Name LIKE 'iPhone%';
```
**Performance Impact**: Enables fast prefix searches (B-tree index)
**Note**: For full-text search at scale, consider PostgreSQL's `pg_trgm` or full-text search indexes

### 7. `IX_Products_IsActive_CreatedDate` ⚡ **COMPOSITE**
```sql
CREATE INDEX IX_Products_IsActive_CreatedDate
ON Products (IsActive ASC, CreatedDate DESC);
```
**Purpose**: "Active products sorted by date" queries
**Query Pattern**:
```sql
SELECT * FROM Products
WHERE IsActive = true
ORDER BY CreatedDate DESC;
```
**Performance Impact**:
- Combines filtering and sorting in one index
- Common for "Latest active products" lists
- Avoids separate filter + sort operations

### 8. `IX_Products_StockQuantity`
```sql
CREATE INDEX IX_Products_StockQuantity ON Products (StockQuantity);
```
**Purpose**: Inventory management and stock alerts
**Query Pattern**:
```sql
SELECT * FROM Products
WHERE StockQuantity < 10;  -- Low stock alert
```
**Use Case**:
- "Out of stock" filtering
- "Low inventory" reports
- Availability checks for bulk operations

---

## Query Performance Examples

### Example 1: E-commerce Product Listing (Category Page)
```sql
SELECT p.Id, p.Name, p.Price, p.StockQuantity, c.Name as CategoryName
FROM Products p
INNER JOIN Categories c ON p.CategoryId = c.Id
WHERE p.CategoryId = 10
  AND p.IsActive = true
  AND c.IsActive = true
ORDER BY p.Price ASC
LIMIT 50;
```
**Indexes Used**:
- `IX_Products_CategoryId_IsActive_Price` (covering index - index-only scan)
- `IX_Categories_IsActive` (filter active categories)

**Performance**: Sub-millisecond for millions of products

---

### Example 2: New Arrivals Page
```sql
SELECT * FROM Products
WHERE IsActive = true
ORDER BY CreatedDate DESC
LIMIT 20;
```
**Indexes Used**:
- `IX_Products_IsActive_CreatedDate` (perfectly optimized)

**Performance**: Index-only scan, O(log n) lookup

---

### Example 3: Low Stock Inventory Report
```sql
SELECT p.*, c.Name as CategoryName
FROM Products p
INNER JOIN Categories c ON p.CategoryId = c.Id
WHERE p.StockQuantity < 10
  AND p.IsActive = true
ORDER BY p.StockQuantity ASC;
```
**Indexes Used**:
- `IX_Products_StockQuantity` (range filter)
- `IX_Products_CategoryId` (JOIN)
- `IX_Products_IsActive` (filter)

**Performance**: Fast for moderate result sets (< 10k rows)

---

### Example 4: Product Search by Name
```sql
SELECT * FROM Products
WHERE Name LIKE 'Gaming%'
  AND IsActive = true
LIMIT 50;
```
**Indexes Used**:
- `IX_Products_Name` (prefix search)
- `IX_Products_IsActive` (filter)

**Performance**: Efficient for prefix searches; for full-text, consider `pg_trgm`

---

## Index Maintenance Considerations

### Write Performance Impact
- Each index adds overhead during `INSERT`, `UPDATE`, and `DELETE` operations
- For bulk imports, consider temporarily disabling indexes and rebuilding afterward
- PostgreSQL handles 7-10 indexes per table reasonably well for OLTP workloads

### Index Size Estimation
For **10 million products**:
- Each index: ~100-300 MB
- Total index overhead: ~1-2 GB
- Storage is cheap; query speed is priceless

### Monitoring & Optimization
Use PostgreSQL's built-in tools to monitor index usage:
```sql
-- Check index usage statistics
SELECT schemaname, tablename, indexname, idx_scan, idx_tup_read, idx_tup_fetch
FROM pg_stat_user_indexes
WHERE tablename = 'Products'
ORDER BY idx_scan DESC;

-- Identify unused indexes
SELECT schemaname, tablename, indexname
FROM pg_stat_user_indexes
WHERE idx_scan = 0 AND tablename = 'Products';
```

### When to Add More Indexes
Consider additional indexes if you observe:
1. Frequent queries on `Price` ranges without `CategoryId`
2. Complex filters combining `Price + StockQuantity + IsActive`
3. Geographic data (if added later, use GiST/SP-GiST indexes)

---

## Future Optimizations

### Partitioning (for 100M+ products)
- **Partition by CategoryId**: If categories have vastly different product counts
- **Partition by CreatedDate**: For time-series queries (monthly/yearly partitions)

### Full-Text Search
- **pg_trgm extension**: For fuzzy matching and typo tolerance
- **GIN index on Name/Description**: For `to_tsvector()` full-text search

### Caching Strategy
- **Redis/Memcached**: Cache hot product data (top sellers, featured products)
- **PostgreSQL Materialized Views**: Pre-compute expensive aggregations

---

## Conclusion

This indexing strategy prioritizes **read performance** for the most common query patterns in a product catalog system handling millions of products. The composite indexes eliminate the need for expensive table scans and sorts, ensuring sub-second response times even at scale.

**Key Takeaway**: The `IX_Products_CategoryId_IsActive_Price` composite index is the MVP—covering the most frequent query pattern used in e-commerce applications.
