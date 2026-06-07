/*
    NutriTEC Database Project
    Script: create_database_sqlserver.sql
    Purpose: Create the project database in SQL Server.

    Execution order:
      1. Run this script from SQL Server Management Studio connected to master.
      2. Run create_schema_sqlserver.sql.
      3. Run seed_data_sqlserver.sql.
      4. Run seed_data_tests_sqlserver.sql.
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
