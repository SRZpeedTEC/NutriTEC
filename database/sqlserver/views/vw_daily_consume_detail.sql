USE nutrition_db;
GO

/*
    Daily consumption detail view
    Combines the existing client/date, meal-time, product-detail, and product
    tables so API reads do not duplicate the relationship joins.
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
