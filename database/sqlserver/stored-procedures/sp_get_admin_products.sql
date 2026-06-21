USE nutrition_db;
GO

/*
    Admin product listing
    Accepts an optional product_status filter and validates the same status set
    enforced by the product table.
*/
CREATE OR ALTER PROCEDURE dbo.sp_get_admin_products
    @product_status VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @normalized_status VARCHAR(20) = NULL;

    IF @product_status IS NOT NULL
    BEGIN
        SET @normalized_status = UPPER(LTRIM(RTRIM(@product_status)));

        IF @normalized_status NOT IN ('ACTIVE', 'PENDING_REVIEW', 'REJECTED')
        BEGIN
            THROW 51010, 'Invalid product status for admin product listing.', 1;
        END;
    END;

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
    WHERE @normalized_status IS NULL
        OR product_status = @normalized_status
    ORDER BY product_name, bar_code;
END;
GO
