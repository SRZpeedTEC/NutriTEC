USE nutrition_db;
GO

/*
    Admin product review update
    Approves or rejects an existing product and records who performed the
    review. The product_status audit trigger stores the history row.
*/
CREATE OR ALTER PROCEDURE dbo.sp_update_product_status_by_admin
    @bar_code VARCHAR(40),
    @admin_id INT,
    @new_status VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @normalized_bar_code VARCHAR(40) = LTRIM(RTRIM(@bar_code));
    DECLARE @normalized_status VARCHAR(20) = UPPER(LTRIM(RTRIM(@new_status)));
    IF @normalized_status NOT IN ('ACTIVE', 'REJECTED')
    BEGIN
        THROW 51011, 'Admin review status must be ACTIVE or REJECTED.', 1;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.product WHERE bar_code = @normalized_bar_code)
    BEGIN
        THROW 51012, 'Product not found.', 1;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.admin WHERE admin_id = @admin_id)
    BEGIN
        THROW 51013, 'Admin not found.', 1;
    END;

    UPDATE dbo.product
    SET product_status = @normalized_status
    WHERE bar_code = @normalized_bar_code;

    EXEC dbo.sp_get_admin_product_by_barcode @bar_code = @normalized_bar_code;
END;
GO
