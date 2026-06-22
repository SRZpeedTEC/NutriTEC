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
      - Admin product-management stored procedures are consumed by the SQL
        Server API administrator endpoints.
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
    p.vitamins AS calculated_vitamins,
    CAST(ROUND(p.calcium * rp.quantity, 2) AS NUMERIC(20, 2)) AS calculated_calcium,
    CAST(ROUND(p.iron * rp.quantity, 2) AS NUMERIC(20, 2)) AS calculated_iron
FROM dbo.recipe AS r
INNER JOIN dbo.recipe_product AS rp
    ON rp.recipe_id = r.recipe_id
INNER JOIN dbo.product AS p
    ON p.bar_code = rp.product_code;
GO

/*
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

-- ============================================================================
-- Stored Procedures
-- ============================================================================

/*
    Returns the persisted admin identity and password hash; password comparison
    remains in C# using the backend hashing abstraction.
*/
CREATE OR ALTER PROCEDURE dbo.sp_admin_login
    @email VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        a.admin_id,
        u.user_id,
        u.email,
        u.name,
        u.last_name,
        u.birthday,
        u.hash_password
    FROM dbo.admin AS a
    INNER JOIN dbo.app_user AS u
        ON u.user_id = a.user_id
    WHERE u.email = LOWER(LTRIM(RTRIM(@email)));
END;
GO

/*
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

/*
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

/*
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

/*
    Calculates prorated billing charges per nutritionist and client for one
    billing cycle. The API groups this rowset into the admin report response.
*/
CREATE OR ALTER PROCEDURE dbo.sp_get_admin_billing_report
    @cycle_start_date DATE,
    @cycle_end_date DATE,
    @frequency VARCHAR(20) = NULL,
    @price_per_patient NUMERIC(18, 2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @cycle_start_date IS NULL OR @cycle_end_date IS NULL OR @cycle_start_date > @cycle_end_date
    BEGIN
        THROW 51020, 'Invalid billing cycle date range.', 1;
    END;

    DECLARE @normalized_frequency VARCHAR(20) = NULL;
    IF @frequency IS NOT NULL AND LTRIM(RTRIM(@frequency)) <> ''
    BEGIN
        SET @normalized_frequency = UPPER(LTRIM(RTRIM(@frequency)));
    END;

    IF @normalized_frequency IS NOT NULL
        AND @normalized_frequency NOT IN ('WEEKLY', 'MONTHLY', 'ANNUAL')
    BEGIN
        THROW 51021, 'Invalid billing frequency.', 1;
    END;

    IF @price_per_patient IS NOT NULL AND @price_per_patient <= 0
    BEGIN
        THROW 51022, 'Billing price per patient must be positive.', 1;
    END;

    DECLARE @cycle_days INT = DATEDIFF(DAY, @cycle_start_date, DATEADD(DAY, 1, @cycle_end_date));

    DECLARE @billing_rules TABLE (
        billing_frequency VARCHAR(20) NOT NULL PRIMARY KEY,
        price_per_patient NUMERIC(18, 2) NOT NULL,
        discount_rate NUMERIC(5, 4) NOT NULL
    );

    INSERT INTO @billing_rules (billing_frequency, price_per_patient, discount_rate)
    VALUES
        ('WEEKLY', 1.00, 0.0000),
        ('MONTHLY', 4.00, 0.0500),
        ('ANNUAL', 52.00, 0.1000);

    IF @price_per_patient IS NOT NULL
        AND @normalized_frequency IS NULL
    BEGIN
        THROW 51023, 'Billing frequency is required when price per patient is provided.', 1;
    END;

    IF @price_per_patient IS NOT NULL
        AND NOT EXISTS (
            SELECT 1
            FROM @billing_rules AS br
            WHERE br.billing_frequency = @normalized_frequency
                AND br.price_per_patient = @price_per_patient
        )
    BEGIN
        THROW 51024, 'Billing price per patient does not match the selected frequency.', 1;
    END;

    WITH overlapping_relationships AS (
        SELECT
            nc.nutritionist_code,
            nc.client_id,
            CASE
                WHEN nc.start_date < @cycle_start_date THEN @cycle_start_date
                ELSE nc.start_date
            END AS active_from,
            CASE
                WHEN COALESCE(nc.end_date, @cycle_end_date) > @cycle_end_date THEN @cycle_end_date
                ELSE COALESCE(nc.end_date, @cycle_end_date)
            END AS active_to
        FROM dbo.nutritionist_client AS nc
        INNER JOIN dbo.nutritionist AS n
            ON n.nutritionist_code = nc.nutritionist_code
        WHERE nc.status IN ('ACTIVE', 'FINISHED', 'CANCELLED')
            AND nc.start_date <= @cycle_end_date
            AND COALESCE(nc.end_date, @cycle_end_date) >= @cycle_start_date
            AND (@normalized_frequency IS NULL OR n.billing_frequency = @normalized_frequency)
    ),
    patient_days AS (
        SELECT
            nutritionist_code,
            client_id,
            MIN(active_from) AS active_from,
            MAX(active_to) AS active_to,
            SUM(DATEDIFF(DAY, active_from, DATEADD(DAY, 1, active_to))) AS active_days
        FROM overlapping_relationships
        WHERE active_from <= active_to
        GROUP BY nutritionist_code, client_id
    )
    SELECT
        n.billing_frequency,
        n.nutritionist_code,
        CONCAT(nu.name, ' ', nu.last_name) AS nutritionist_name,
        nu.email AS nutritionist_email,
        n.payment_method,
        n.encrypted_credit_card AS credit_card_number,
        br.price_per_patient,
        br.discount_rate,
        c.client_id,
        CONCAT(cu.name, ' ', cu.last_name) AS client_name,
        pd.active_from,
        pd.active_to,
        pd.active_days,
        CAST(ROUND(CAST(pd.active_days AS NUMERIC(18, 6)) / @cycle_days, 6) AS NUMERIC(18, 6)) AS proration_factor,
        CAST(ROUND(br.price_per_patient * CAST(pd.active_days AS NUMERIC(18, 6)) / @cycle_days, 2) AS NUMERIC(18, 2)) AS amount_before_discount
    FROM patient_days AS pd
    INNER JOIN dbo.nutritionist AS n
        ON n.nutritionist_code = pd.nutritionist_code
    INNER JOIN @billing_rules AS br
        ON br.billing_frequency = n.billing_frequency
    INNER JOIN dbo.app_user AS nu
        ON nu.user_id = n.user_id
    INNER JOIN dbo.client AS c
        ON c.client_id = pd.client_id
    INNER JOIN dbo.app_user AS cu
        ON cu.user_id = c.user_id
    ORDER BY
        n.billing_frequency,
        nutritionist_name,
        n.nutritionist_code,
        client_name,
        c.client_id;
END;
GO

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
