USE nutrition_db;
GO

/*
    Admin product view
    Exposes product nutrition, creator identity, and review metadata for the
    administrator product-management screen.
*/
CREATE OR ALTER VIEW dbo.vw_admin_products
AS
SELECT
    p.bar_code,
    p.product_name,
    p.portion_unit,
    p.portion_size,
    p.calories,
    p.fat,
    p.sodium,
    p.carbohydrates,
    p.protein,
    p.vitamins,
    p.calcium,
    p.iron,
    p.product_status,
    p.user_id AS created_by_user_id,
    CONCAT(creator.name, ' ', creator.last_name) AS created_by_name,
    creator.email AS created_by_email
FROM dbo.product AS p
INNER JOIN dbo.app_user AS creator
    ON creator.user_id = p.user_id;
GO
