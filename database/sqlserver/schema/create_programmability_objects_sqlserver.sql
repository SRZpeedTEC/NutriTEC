/*
    NutriTEC Database Project
    Script: create_programmability_objects_sqlserver.sql
    Purpose: Create or update every SQL Server view, trigger, and stored
             procedure required by the current backend in one execution.

    Notes:
      - Run this script after create_schema_sqlserver.sql.
      - The script is standalone T-SQL and does not require SQLCMD mode.
      - Individual source files remain organized under views, triggers, and
        stored-procedures for focused maintenance.
      - No stored procedures are currently required by the project.
*/

USE nutrition_db;
GO

-- ============================================================================
-- Views
-- ============================================================================

/*
    Combines daily consumption relationships so API reads can return products
    grouped by client, date, and meal-time instance.
*/
CREATE OR ALTER VIEW dbo.vw_daily_consume_detail
AS
SELECT
    dc.client_id,
    dc.consume_date,
    mt.meal_time_id,
    mt.meal_type,
    p.bar_code,
    p.product_name,
    p.portion_unit,
    p.portion_size,
    mtp.quantity,
    mtp.calories,
    p.protein,
    p.carbohydrates,
    p.fat,
    p.sodium
FROM dbo.daily_consume AS dc
INNER JOIN dbo.daily_meal_time AS dmt
    ON dmt.client_id = dc.client_id
    AND dmt.consume_date = dc.consume_date
INNER JOIN dbo.meal_time AS mt
    ON mt.meal_time_id = dmt.meal_time_id
INNER JOIN dbo.meal_time_product AS mtp
    ON mtp.meal_time_id = mt.meal_time_id
INNER JOIN dbo.product AS p
    ON p.bar_code = mtp.product_code;
GO

/*
    Exposes recipe ingredients with their quantity-scaled nutritional values
    while retaining recipe ownership and stored calorie totals.
*/
CREATE OR ALTER VIEW dbo.vw_recipe_product_detail
AS
SELECT
    r.recipe_id,
    r.recipe_name,
    r.total_calories,
    r.client_id,
    p.bar_code,
    p.product_name,
    p.portion_unit,
    p.portion_size,
    rp.quantity,
    CAST(ROUND(p.calories * rp.quantity, 2) AS NUMERIC(20, 2)) AS calculated_calories,
    CAST(ROUND(p.fat * rp.quantity, 2) AS NUMERIC(20, 2)) AS calculated_fat,
    CAST(ROUND(p.sodium * rp.quantity, 2) AS NUMERIC(20, 2)) AS calculated_sodium,
    CAST(ROUND(p.carbohydrates * rp.quantity, 2) AS NUMERIC(20, 2)) AS calculated_carbohydrates,
    CAST(ROUND(p.protein * rp.quantity, 2) AS NUMERIC(20, 2)) AS calculated_protein,
    CAST(ROUND(p.vitamins * rp.quantity, 2) AS NUMERIC(20, 2)) AS calculated_vitamins,
    CAST(ROUND(p.calcium * rp.quantity, 2) AS NUMERIC(20, 2)) AS calculated_calcium,
    CAST(ROUND(p.iron * rp.quantity, 2) AS NUMERIC(20, 2)) AS calculated_iron
FROM dbo.recipe AS r
INNER JOIN dbo.recipe_product AS rp
    ON rp.recipe_id = r.recipe_id
INNER JOIN dbo.product AS p
    ON p.bar_code = rp.product_code;
GO

-- ============================================================================
-- Stored Procedures
-- ============================================================================

-- No stored procedures are currently required by the NutriTEC backend.

-- ============================================================================
-- Triggers
-- ============================================================================

/*
    Recalculates daily_consume.total_calories for each daily record affected by
    inserted, updated, or deleted meal-time products.
*/
CREATE OR ALTER TRIGGER dbo.trg_update_daily_consume_totals
ON dbo.meal_time_product
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    WITH affected_meal_times AS (
        SELECT meal_time_id FROM inserted
        UNION
        SELECT meal_time_id FROM deleted
    ),
    affected_daily_consumes AS (
        SELECT DISTINCT
            dmt.client_id,
            dmt.consume_date
        FROM dbo.daily_meal_time AS dmt
        INNER JOIN affected_meal_times AS affected
            ON affected.meal_time_id = dmt.meal_time_id
    ),
    daily_totals AS (
        SELECT
            dmt.client_id,
            dmt.consume_date,
            SUM(mtp.calories) AS total_calories
        FROM dbo.daily_meal_time AS dmt
        INNER JOIN dbo.meal_time_product AS mtp
            ON mtp.meal_time_id = dmt.meal_time_id
        GROUP BY
            dmt.client_id,
            dmt.consume_date
    )
    UPDATE dc
    SET dc.total_calories = COALESCE(totals.total_calories, 0)
    FROM dbo.daily_consume AS dc
    INNER JOIN affected_daily_consumes AS affected
        ON affected.client_id = dc.client_id
        AND affected.consume_date = dc.consume_date
    LEFT JOIN daily_totals AS totals
        ON totals.client_id = dc.client_id
        AND totals.consume_date = dc.consume_date;
END;
GO

/*
    Rejects multiple meal-time instances of the same type for one client and
    date, including writes performed outside the API.
*/
CREATE OR ALTER TRIGGER dbo.trg_validate_daily_meal_time_uniqueness
ON dbo.daily_meal_time
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM inserted AS candidate
        INNER JOIN dbo.meal_time AS candidate_meal_time
            ON candidate_meal_time.meal_time_id = candidate.meal_time_id
        INNER JOIN dbo.daily_meal_time AS existing
            ON existing.client_id = candidate.client_id
            AND existing.consume_date = candidate.consume_date
            AND existing.daily_meal_time_id <> candidate.daily_meal_time_id
        INNER JOIN dbo.meal_time AS existing_meal_time
            ON existing_meal_time.meal_time_id = existing.meal_time_id
            AND existing_meal_time.meal_type = candidate_meal_time.meal_type
    )
    BEGIN
        THROW 51001, 'A client can have only one daily meal-time instance of each type per date.', 1;
    END;
END;
GO

/*
    Recalculates stored recipe calories for every recipe affected by a
    multi-row ingredient insert, update, or delete.
*/
CREATE OR ALTER TRIGGER dbo.trg_update_recipe_nutrition_totals
ON dbo.recipe_product
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    WITH affected_recipes AS (
        SELECT recipe_id FROM inserted
        UNION
        SELECT recipe_id FROM deleted
    ),
    recipe_totals AS (
        SELECT
            ingredient.recipe_id,
            CAST(
                ROUND(SUM(product.calories * ingredient.quantity), 2)
                AS NUMERIC(10, 2)
            ) AS total_calories
        FROM dbo.recipe_product AS ingredient
        INNER JOIN dbo.product AS product
            ON product.bar_code = ingredient.product_code
        GROUP BY ingredient.recipe_id
    )
    UPDATE recipe
    SET recipe.total_calories = COALESCE(totals.total_calories, 0)
    FROM dbo.recipe AS recipe
    INNER JOIN affected_recipes AS affected
        ON affected.recipe_id = recipe.recipe_id
    LEFT JOIN recipe_totals AS totals
        ON totals.recipe_id = recipe.recipe_id;
END;
GO
