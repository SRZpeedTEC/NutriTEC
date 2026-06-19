USE nutrition_db;
GO

/*
    Daily calorie total trigger
    Recalculates daily_consume.total_calories after meal-time products change.
    This version uses LEFT JOIN instead of OUTER APPLY for easier readability.
*/
CREATE OR ALTER TRIGGER trg_update_daily_consume_totals
ON meal_time_product
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
        FROM daily_meal_time AS dmt
        INNER JOIN affected_meal_times AS affected
            ON affected.meal_time_id = dmt.meal_time_id
    ),
    daily_totals AS (
        SELECT
            dmt.client_id,
            dmt.consume_date,
            SUM(mtp.calories) AS total_calories
        FROM daily_meal_time AS dmt
        INNER JOIN meal_time_product AS mtp
            ON mtp.meal_time_id = dmt.meal_time_id
        GROUP BY
            dmt.client_id,
            dmt.consume_date
    )
    UPDATE dc
    SET dc.total_calories = COALESCE(totals.total_calories, 0)
    FROM daily_consume AS dc
    INNER JOIN affected_daily_consumes AS affected
        ON affected.client_id = dc.client_id
        AND affected.consume_date = dc.consume_date
    LEFT JOIN daily_totals AS totals
        ON totals.client_id = dc.client_id
        AND totals.consume_date = dc.consume_date;
END;
GO