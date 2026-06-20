USE nutrition_db;
GO

/*
    Recipe product detail view
    Exposes ingredient quantities and their calculated nutritional contribution
    so API reads share one database definition of recipe composition.
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
