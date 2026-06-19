/*
    NutriTEC Database Project
    Script: create_database_sqlserver.sql
    Purpose: Create the project database in SQL Server.

    Execution order:
      1. Run this script from SQL Server Management Studio connected to master.
      2. Run create_schema_sqlserver.sql.
      3. Run views/vw_daily_consume_detail.sql.
      4. Run triggers/trg_update_daily_consume_totals.sql.
      5. Run triggers/trg_validate_daily_meal_time_uniqueness.sql.
      6. Run seed/seed_data_sqlserver.sql.
      7. Run the SQL Server sample or REST scripts, if needed.
*/

USE master;
GO

IF DB_ID('nutrition_db') IS NULL
BEGIN
    CREATE DATABASE nutrition_db;
END;
GO

USE nutrition_db;
GO
