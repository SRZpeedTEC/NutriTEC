USE nutrition_db;
GO

/*
    Admin login lookup
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
