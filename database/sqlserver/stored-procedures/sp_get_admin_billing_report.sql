USE nutrition_db;
GO

/*
    Admin billing report
    Calculates prorated charges by nutritionist and client for one billing cycle.
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
