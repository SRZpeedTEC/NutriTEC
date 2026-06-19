USE nutrition_db;
GO

/*
    Daily meal-time uniqueness trigger
    Protects the service invariant that one client may have only one meal-time
    instance of each type on a given date, even when data is written outside the API.
*/
CREATE OR ALTER TRIGGER dbo.trg_validate_daily_meal_time_uniqueness
ON dbo.daily_meal_time
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Compare inserted rows with the persisted relationship set using the meal type, not only its instance id.
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
