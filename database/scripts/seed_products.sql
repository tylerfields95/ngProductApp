-- =============================================
-- Seed Products Table (Procedurally Generated)
-- =============================================
-- This script procedurally generates a large number of test products
-- Run this AFTER seed_categories.sql has been executed
--
-- CONFIGURATION:
-- Adjust the generate_series() range to control how many products are generated
-- Current setting: 100,000 products (adjust as needed)
-- For 1 million products, use generate_series(1, 1000000)
-- For 10 million products, use generate_series(1, 10000000)

-- =============================================
-- PART 1: Product Name Components
-- =============================================
-- Create temporary arrays for generating realistic product names

DO $$
DECLARE
    v_category_id INTEGER;
    v_category_name VARCHAR(200);
    v_product_count INTEGER := 0;
    v_batch_size INTEGER := 10000; -- Insert in batches for better performance
    v_total_products INTEGER := 100000; -- CHANGE THIS to generate more/fewer products
BEGIN
    RAISE NOTICE 'Starting product generation...';
    RAISE NOTICE 'Target: % products', v_total_products;

    -- Loop through each active category
    FOR v_category_id, v_category_name IN
        SELECT "Id", "Name" FROM "Categories" WHERE "IsActive" = true
    LOOP
        RAISE NOTICE 'Generating products for category: % (ID: %)', v_category_name, v_category_id;

        -- Generate products for this category
        -- Distribute products across categories (more products per category)
        INSERT INTO "Products" (
            "Name",
            "Description",
            "Price",
            "CategoryId",
            "StockQuantity",
            "CreatedDate",
            "IsActive"
        )
        SELECT
            -- Generate product names with variations
            CASE v_category_name
                WHEN 'Electronics' THEN
                    (ARRAY['Premium', 'Ultra', 'Pro', 'Max', 'Elite', 'Advanced', 'Basic', 'Standard'])[1 + (random() * 7)::INT] || ' ' ||
                    (ARRAY['Electronic', 'Digital', 'Smart', 'Wireless', 'Portable'])[1 + (random() * 4)::INT] || ' ' ||
                    (ARRAY['Device', 'Gadget', 'System', 'Unit'])[1 + (random() * 3)::INT] || ' ' ||
                    'Model-' || LPAD(i::TEXT, 5, '0')

                WHEN 'Computers & Laptops' THEN
                    (ARRAY['Gaming', 'Business', 'Student', 'Professional', 'Home', 'Creator'])[1 + (random() * 5)::INT] || ' ' ||
                    (ARRAY['Laptop', 'Desktop', 'Workstation', 'PC'])[1 + (random() * 3)::INT] || ' ' ||
                    (ARRAY['i5', 'i7', 'i9', 'Ryzen 5', 'Ryzen 7', 'Ryzen 9'])[1 + (random() * 5)::INT] || ' ' ||
                    (8 + (random() * 56)::INT)::TEXT || 'GB RAM'

                WHEN 'Smartphones & Tablets' THEN
                    (ARRAY['Galaxy', 'iPhone', 'Pixel', 'OnePlus', 'Xperia', 'Mi'])[1 + (random() * 5)::INT] || ' ' ||
                    (ARRAY['Pro', 'Max', 'Ultra', 'Plus', 'Lite'])[1 + (random() * 4)::INT] || ' ' ||
                    (32 + (random() * 480)::INT)::TEXT || 'GB ' ||
                    (ARRAY['5G', '4G', 'WiFi'])[1 + (random() * 2)::INT]

                WHEN 'Gaming' THEN
                    (ARRAY['Xbox', 'PlayStation', 'Nintendo', 'Gaming'])[1 + (random() * 3)::INT] || ' ' ||
                    (ARRAY['Controller', 'Console', 'Headset', 'Keyboard', 'Mouse', 'Chair'])[1 + (random() * 5)::INT] || ' ' ||
                    (ARRAY['RGB', 'Wireless', 'Pro', 'Elite', 'Standard'])[1 + (random() * 4)::INT]

                WHEN 'Mens Fashion' THEN
                    (ARRAY['Casual', 'Formal', 'Sport', 'Classic', 'Modern'])[1 + (random() * 4)::INT] || ' ' ||
                    (ARRAY['Shirt', 'Pants', 'Jacket', 'Sweater', 'Jeans', 'Suit'])[1 + (random() * 5)::INT] || ' ' ||
                    (ARRAY['Slim Fit', 'Regular Fit', 'Relaxed Fit'])[1 + (random() * 2)::INT]

                WHEN 'Womens Fashion' THEN
                    (ARRAY['Elegant', 'Casual', 'Formal', 'Trendy', 'Classic', 'Bohemian'])[1 + (random() * 5)::INT] || ' ' ||
                    (ARRAY['Dress', 'Blouse', 'Skirt', 'Pants', 'Jacket', 'Sweater'])[1 + (random() * 5)::INT] || ' ' ||
                    (ARRAY['Size S', 'Size M', 'Size L', 'Size XL'])[1 + (random() * 3)::INT]

                WHEN 'Home & Kitchen' THEN
                    (ARRAY['Premium', 'Deluxe', 'Essential', 'Professional', 'Home'])[1 + (random() * 4)::INT] || ' ' ||
                    (ARRAY['Blender', 'Mixer', 'Coffee Maker', 'Toaster', 'Air Fryer', 'Microwave'])[1 + (random() * 5)::INT] || ' ' ||
                    (ARRAY['Stainless Steel', 'Digital', 'Smart', 'Manual'])[1 + (random() * 3)::INT]

                WHEN 'Furniture' THEN
                    (ARRAY['Modern', 'Classic', 'Contemporary', 'Vintage', 'Minimalist'])[1 + (random() * 4)::INT] || ' ' ||
                    (ARRAY['Sofa', 'Chair', 'Table', 'Desk', 'Bed', 'Cabinet', 'Shelf'])[1 + (random() * 6)::INT] || ' ' ||
                    (ARRAY['Wood', 'Metal', 'Fabric', 'Leather'])[1 + (random() * 3)::INT]

                WHEN 'Sports & Fitness' THEN
                    (ARRAY['Professional', 'Premium', 'Standard', 'Elite', 'Pro'])[1 + (random() * 4)::INT] || ' ' ||
                    (ARRAY['Dumbbell', 'Treadmill', 'Yoga Mat', 'Resistance Band', 'Weight Bench'])[1 + (random() * 4)::INT] || ' ' ||
                    (5 + (random() * 45)::INT)::TEXT || 'kg'

                WHEN 'Books & Media' THEN
                    (ARRAY['The', 'A', 'An'])[1 + (random() * 2)::INT] || ' ' ||
                    (ARRAY['Complete', 'Ultimate', 'Essential', 'Comprehensive', 'Practical'])[1 + (random() * 4)::INT] || ' ' ||
                    (ARRAY['Guide', 'Manual', 'Handbook', 'Reference', 'Collection'])[1 + (random() * 4)::INT] || ' ' ||
                    'to ' || (ARRAY['Programming', 'Design', 'Business', 'Cooking', 'Fitness', 'Photography'])[1 + (random() * 5)::INT]

                ELSE
                    -- Generic product name for other categories
                    v_category_name || ' ' ||
                    (ARRAY['Premium', 'Standard', 'Basic', 'Pro', 'Elite', 'Deluxe'])[1 + (random() * 5)::INT] || ' ' ||
                    (ARRAY['Item', 'Product', 'Article', 'Piece'])[1 + (random() * 3)::INT] || ' ' ||
                    '#' || LPAD(i::TEXT, 6, '0')
            END AS "Name",

            -- Generate description
            'High-quality ' || LOWER(v_category_name) || ' product with excellent features and durability. ' ||
            'Perfect for ' || (ARRAY['everyday use', 'professional use', 'home use', 'outdoor activities', 'special occasions'])[1 + (random() * 4)::INT] || '. ' ||
            'Model: ' || LPAD(i::TEXT, 8, '0') || '. ' ||
            'Manufactured with premium materials and backed by quality assurance.' AS "Description",

            -- Generate realistic prices based on category
            CASE v_category_name
                WHEN 'Computers & Laptops' THEN ROUND((500 + random() * 2500)::NUMERIC, 2)
                WHEN 'Smartphones & Tablets' THEN ROUND((200 + random() * 1300)::NUMERIC, 2)
                WHEN 'Gaming' THEN ROUND((50 + random() * 950)::NUMERIC, 2)
                WHEN 'Furniture' THEN ROUND((100 + random() * 2900)::NUMERIC, 2)
                WHEN 'Jewelry & Watches' THEN ROUND((50 + random() * 5000)::NUMERIC, 2)
                WHEN 'Electronics' THEN ROUND((30 + random() * 1000)::NUMERIC, 2)
                WHEN 'Books & Media' THEN ROUND((5 + random() * 50)::NUMERIC, 2)
                WHEN 'Mens Fashion' THEN ROUND((20 + random() * 300)::NUMERIC, 2)
                WHEN 'Womens Fashion' THEN ROUND((20 + random() * 300)::NUMERIC, 2)
                WHEN 'Home & Kitchen' THEN ROUND((15 + random() * 500)::NUMERIC, 2)
                WHEN 'Sports & Fitness' THEN ROUND((20 + random() * 800)::NUMERIC, 2)
                ELSE ROUND((10 + random() * 500)::NUMERIC, 2)
            END AS "Price",

            -- Category ID
            v_category_id AS "CategoryId",

            -- Generate stock quantity (with some out of stock items)
            CASE
                WHEN random() < 0.05 THEN 0  -- 5% out of stock
                WHEN random() < 0.15 THEN (1 + (random() * 9)::INT)  -- 10% low stock (1-10)
                ELSE (10 + (random() * 990)::INT)  -- Normal stock (10-1000)
            END AS "StockQuantity",

            -- Generate random created dates over the past 3 years
            (CURRENT_TIMESTAMP - (random() * INTERVAL '1095 days')) AS "CreatedDate",

            -- 95% active products, 5% inactive
            (random() > 0.05) AS "IsActive"

        FROM generate_series(1, (v_total_products / 37)::INT) AS i  -- Divide by number of active categories
        WHERE i <= (v_total_products / 37)::INT;

        v_product_count := v_product_count + (v_total_products / 37)::INT;

    END LOOP;

    RAISE NOTICE 'Product generation complete!';
    RAISE NOTICE 'Total products generated: %', v_product_count;

END $$;

-- =============================================
-- PART 2: Verification Queries
-- =============================================

-- Summary statistics
SELECT
    COUNT(*) AS "Total Products",
    COUNT(CASE WHEN "IsActive" = true THEN 1 END) AS "Active Products",
    COUNT(CASE WHEN "IsActive" = false THEN 1 END) AS "Inactive Products",
    COUNT(CASE WHEN "StockQuantity" = 0 THEN 1 END) AS "Out of Stock",
    COUNT(CASE WHEN "StockQuantity" BETWEEN 1 AND 10 THEN 1 END) AS "Low Stock (1-10)",
    ROUND(AVG("Price")::NUMERIC, 2) AS "Average Price",
    ROUND(MIN("Price")::NUMERIC, 2) AS "Min Price",
    ROUND(MAX("Price")::NUMERIC, 2) AS "Max Price"
FROM "Products";

-- Products per category
SELECT
    c."Name" AS "Category",
    COUNT(p."Id") AS "Product Count",
    ROUND(AVG(p."Price")::NUMERIC, 2) AS "Avg Price",
    COUNT(CASE WHEN p."IsActive" = true THEN 1 END) AS "Active",
    COUNT(CASE WHEN p."StockQuantity" = 0 THEN 1 END) AS "Out of Stock"
FROM "Categories" c
LEFT JOIN "Products" p ON c."Id" = p."CategoryId"
WHERE c."IsActive" = true
GROUP BY c."Id", c."Name"
ORDER BY COUNT(p."Id") DESC;

-- Sample products from each category (5 per category)
SELECT
    c."Name" AS "Category",
    p."Name" AS "Product",
    p."Price",
    p."StockQuantity",
    p."IsActive"
FROM "Products" p
INNER JOIN "Categories" c ON p."CategoryId" = c."Id"
WHERE p."Id" IN (
    SELECT "Id" FROM "Products" WHERE "CategoryId" = c."Id" LIMIT 5
)
ORDER BY c."Name", p."Name"
LIMIT 50;

-- Index usage verification
SELECT
    schemaname,
    relname AS tablename,
    indexrelname AS indexname,
    idx_scan AS "Times Used",
    idx_tup_read AS "Tuples Read",
    idx_tup_fetch AS "Tuples Fetched"
FROM pg_stat_user_indexes
WHERE relname = 'Products'
ORDER BY idx_scan DESC;

-- Final success message
DO $$
BEGIN
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Seed script completed successfully!';
    RAISE NOTICE '========================================';
END $$;
