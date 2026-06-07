/*
    NutriTEC Database Project
    Script: 00_create_database.sql
    Purpose: Create the project database.

    Execution order:
      1. Run this script from a PostgreSQL administrative database.
      2. Connect to nutrition_db.
      3. Run 01_create_schema.sql, 02_seed_data.sql, and 03_visualization_samples.sql.
*/

-- ============================================================================
-- Database Creation
-- ============================================================================

CREATE DATABASE nutrition_db;
