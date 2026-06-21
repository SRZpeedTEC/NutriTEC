USE nutrition_db;
GO

/*
    Admin product detail lookup
    Reads from vw_admin_products so list and detail responses stay aligned.
*/
CREATE OR ALTER PROCEDURE dbo.sp_get_admin_product_by_barcode
    @bar_code VARCHAR(40)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        bar_code,
        product_name,
        portion_unit,
        portion_size,
        calories,
        fat,
        sodium,
        carbohydrates,
        protein,
        vitamins,
        calcium,
        iron,
        product_status,
        created_by_user_id,
        created_by_name,
        created_by_email
    FROM dbo.vw_admin_products
    WHERE bar_code = LTRIM(RTRIM(@bar_code));
END;
GO
