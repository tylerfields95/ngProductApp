-- =============================================
-- Seed Categories Table
-- =============================================
-- This script inserts test data into the Categories table
-- Run this after the migration has been applied

-- Insert main product categories
INSERT INTO "Categories" ("Name", "Description", "IsActive")
VALUES
    -- Electronics
    ('Electronics', 'Electronic devices and accessories including phones, computers, and gadgets', true),
    ('Computers & Laptops', 'Desktop computers, laptops, and computer accessories', true),
    ('Smartphones & Tablets', 'Mobile phones, tablets, and mobile accessories', true),
    ('Audio & Headphones', 'Speakers, headphones, earbuds, and audio equipment', true),
    ('Gaming', 'Video game consoles, gaming PCs, and gaming accessories', true),
    ('Cameras & Photography', 'Digital cameras, lenses, tripods, and photography equipment', true),
    ('Wearables & Smartwatches', 'Smart watches, fitness trackers, and wearable technology', true),

    -- Home & Living
    ('Home & Kitchen', 'Home appliances, kitchen tools, and household items', true),
    ('Furniture', 'Indoor and outdoor furniture for home and office', true),
    ('Home Decor', 'Decorative items, wall art, and home accents', true),
    ('Bedding & Bath', 'Bedding sets, towels, and bathroom accessories', true),
    ('Lighting', 'Indoor and outdoor lighting fixtures and bulbs', true),

    -- Fashion & Apparel
    ('Mens Fashion', 'Clothing, shoes, and accessories for men', true),
    ('Womens Fashion', 'Clothing, shoes, and accessories for women', true),
    ('Kids Fashion', 'Clothing and accessories for children', true),
    ('Shoes & Footwear', 'Shoes, boots, sandals, and footwear for all ages', true),
    ('Jewelry & Watches', 'Jewelry, watches, and fashion accessories', true),
    ('Bags & Luggage', 'Handbags, backpacks, luggage, and travel accessories', true),

    -- Sports & Outdoors
    ('Sports & Fitness', 'Exercise equipment, sports gear, and fitness accessories', true),
    ('Outdoor Recreation', 'Camping gear, hiking equipment, and outdoor activities', true),
    ('Cycling', 'Bicycles, cycling gear, and bike accessories', true),
    ('Fishing & Hunting', 'Fishing rods, hunting equipment, and outdoor sports gear', true),

    -- Health & Beauty
    ('Health & Personal Care', 'Health products, vitamins, and personal care items', true),
    ('Beauty & Cosmetics', 'Makeup, skincare, haircare, and beauty products', true),
    ('Wellness & Supplements', 'Vitamins, supplements, and wellness products', true),

    -- Toys & Entertainment
    ('Toys & Games', 'Toys, board games, puzzles, and educational games', true),
    ('Books & Media', 'Books, magazines, music, and movies', true),
    ('Musical Instruments', 'Guitars, keyboards, drums, and musical accessories', true),
    ('Crafts & Hobbies', 'Art supplies, craft materials, and hobby equipment', true),

    -- Automotive
    ('Automotive', 'Car parts, accessories, and automotive tools', true),
    ('Motorcycles & ATVs', 'Motorcycles, ATVs, and riding gear', true),

    -- Grocery & Food
    ('Grocery & Gourmet Food', 'Food, beverages, and gourmet products', true),
    ('Pet Supplies', 'Pet food, toys, and pet care products', true),

    -- Office & School
    ('Office Supplies', 'Office furniture, stationery, and business supplies', true),
    ('School Supplies', 'School supplies, backpacks, and educational materials', true),

    -- Garden & Tools
    ('Garden & Outdoor', 'Gardening tools, plants, and outdoor maintenance', true),
    ('Tools & Home Improvement', 'Power tools, hand tools, and home improvement supplies', true),

    -- Inactive category for testing
    ('Discontinued Products', 'Old products no longer sold', false);

-- Verify the insert
SELECT COUNT(*) as "Total Categories" FROM "Categories";
SELECT COUNT(*) as "Active Categories" FROM "Categories" WHERE "IsActive" = true;
SELECT COUNT(*) as "Inactive Categories" FROM "Categories" WHERE "IsActive" = false;

-- Display all categories
SELECT "Id", "Name", "IsActive" FROM "Categories" ORDER BY "Id";
