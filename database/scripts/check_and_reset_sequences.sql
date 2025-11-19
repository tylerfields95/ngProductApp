-- =============================================
-- Check and Reset Sequences
-- =============================================
-- This script checks the current state of sequences and can reset them

-- Check current sequence values vs actual data
SELECT
    'Products' AS "Table",
    (SELECT COALESCE(MAX("Id"), 0) FROM "Products") AS "Max ID in Table",
    (SELECT last_value FROM "Products_Id_seq") AS "Current Sequence Value",
    (SELECT COALESCE(MIN("Id"), 0) FROM "Products") AS "Min ID in Table",
    (SELECT COUNT(*) FROM "Products") AS "Row Count";

SELECT
    'Categories' AS "Table",
    (SELECT COALESCE(MAX("Id"), 0) FROM "Categories") AS "Max ID in Table",
    (SELECT last_value FROM "Categories_Id_seq") AS "Current Sequence Value",
    (SELECT COALESCE(MIN("Id"), 0) FROM "Categories") AS "Min ID in Table",
    (SELECT COUNT(*) FROM "Categories") AS "Row Count";

-- Show sample IDs from Products table
SELECT "Id", "Name", "CategoryId"
FROM "Products"
ORDER BY "Id"
LIMIT 10;

-- =============================================
-- OPTIONAL: Reset Sequences (Uncomment to run)
-- =============================================
-- WARNING: Only run this if you want to reset the sequences
-- This is useful if you've deleted all data and want to start fresh

-- Reset Products sequence to max ID + 1
-- SELECT setval('"Products_Id_seq"', COALESCE((SELECT MAX("Id") FROM "Products"), 0) + 1, false);

-- Reset Categories sequence to max ID + 1
-- SELECT setval('"Categories_Id_seq"', COALESCE((SELECT MAX("Id") FROM "Categories"), 0) + 1, false);

-- =============================================
-- OPTIONAL: Clear and Reseed Tables
-- =============================================
-- WARNING: This will DELETE ALL DATA!
-- Uncomment to clear tables and reset sequences to 1

/*
-- Delete all products (Categories will remain due to FK constraint)
DELETE FROM "Products";

-- Delete all categories
DELETE FROM "Categories";

-- Reset sequences to 1
ALTER SEQUENCE "Products_Id_seq" RESTART WITH 1;
ALTER SEQUENCE "Categories_Id_seq" RESTART WITH 1;

-- Verify sequences are reset
SELECT 'Products_Id_seq' AS "Sequence", last_value FROM "Products_Id_seq";
SELECT 'Categories_Id_seq' AS "Sequence", last_value FROM "Categories_Id_seq";
*/
