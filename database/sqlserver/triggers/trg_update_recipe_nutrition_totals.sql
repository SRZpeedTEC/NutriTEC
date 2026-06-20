USE nutrition_db;
GO

/*
    Recipe calorie total trigger
    Recalculates stored totals for every recipe affected by a multi-row ingredient
    insert, update, or delete while product nutrition remains the source value.
*/
CREATE OR ALTER TRIGGER dbo.trg_update_recipe_nutrition_totals
ON dbo.recipe_product
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Both transition tables are required because an update can move an ingredient between recipes.
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