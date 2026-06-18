USE nutrition_db;
GO

/*
    NutriTEC Database Project
    Script: seed_data_tests_sqlserver.sql
    Purpose: Provide read-only sample queries that verify schema relationships
             and seed data.

    Notes:
      - Run this script after database/sqlserver/seed/seed_data_sqlserver.sql.
      - These queries match the SQL Server schema in database/sqlserver.
*/

-- ============================================================================
-- 1. List all users with their role/profile type.
-- ============================================================================

SELECT
    u.user_id,
    u.name,
    u.last_name,
    u.email,
    CASE
        WHEN a.admin_id IS NOT NULL THEN 'ADMIN'
        WHEN n.nutritionist_code IS NOT NULL THEN 'NUTRITIONIST'
        WHEN c.client_id IS NOT NULL THEN 'CLIENT'
        ELSE 'UNASSIGNED'
    END AS profile_type
FROM app_user AS u
LEFT JOIN admin AS a
    ON a.user_id = u.user_id
LEFT JOIN nutritionist AS n
    ON n.user_id = u.user_id
LEFT JOIN client AS c
    ON c.user_id = u.user_id
ORDER BY u.user_id;

-- ============================================================================
-- 2. List all nutritionists with their assigned clients.
-- ============================================================================

SELECT
    n.nutritionist_code,
    CONCAT(nu.name, ' ', nu.last_name) AS nutritionist_name,
    c.client_id,
    CONCAT(cu.name, ' ', cu.last_name) AS client_name,
    nc.start_date,
    nc.end_date,
    nc.status
FROM nutritionist AS n
JOIN app_user AS nu
    ON nu.user_id = n.user_id
JOIN nutritionist_client AS nc
    ON nc.nutritionist_code = n.nutritionist_code
JOIN client AS c
    ON c.client_id = nc.client_id
JOIN app_user AS cu
    ON cu.user_id = c.user_id
ORDER BY n.nutritionist_code, c.client_id, nc.start_date;

-- ============================================================================
-- 3. List all plans with their nutritionist.
-- ============================================================================

SELECT
    p.plan_id,
    p.plan_name,
    p.total_calories,
    n.nutritionist_code,
    CONCAT(u.name, ' ', u.last_name) AS nutritionist_name
FROM user_plan AS p
JOIN nutritionist AS n
    ON n.nutritionist_code = p.nutritionist_code
JOIN app_user AS u
    ON u.user_id = n.user_id
ORDER BY p.plan_id;

-- ============================================================================
-- 4. List each plan with its meal times and products.
-- ============================================================================

SELECT
    p.plan_id,
    p.plan_name,
    pmt.plan_meal_time_id,
    mt.meal_type,
    pr.bar_code,
    pr.product_name,
    mtp.quantity,
    mtp.calories AS contributed_calories
FROM user_plan AS p
JOIN plan_meal_time AS pmt
    ON pmt.plan_id = p.plan_id
JOIN meal_time AS mt
    ON mt.meal_time_id = pmt.meal_time_id
JOIN meal_time_product AS mtp
    ON mtp.meal_time_id = mt.meal_time_id
JOIN product AS pr
    ON pr.bar_code = mtp.product_code
ORDER BY p.plan_id, pmt.plan_meal_time_id, pr.product_name;

-- ============================================================================
-- 5. Calculate total calories per plan from products.
-- ============================================================================

SELECT
    p.plan_id,
    p.plan_name,
    SUM(mtp.calories) AS calculated_total_calories
FROM user_plan AS p
JOIN plan_meal_time AS pmt
    ON pmt.plan_id = p.plan_id
JOIN meal_time_product AS mtp
    ON mtp.meal_time_id = pmt.meal_time_id
GROUP BY p.plan_id, p.plan_name
ORDER BY p.plan_id;

-- ============================================================================
-- 6. Compare stored plan calories with calculated product calories.
-- ============================================================================

SELECT
    p.plan_id,
    p.plan_name,
    p.total_calories AS stored_total_calories,
    COALESCE(SUM(mtp.calories), 0) AS calculated_total_calories,
    p.total_calories - COALESCE(SUM(mtp.calories), 0) AS calorie_difference
FROM user_plan AS p
LEFT JOIN plan_meal_time AS pmt
    ON pmt.plan_id = p.plan_id
LEFT JOIN meal_time_product AS mtp
    ON mtp.meal_time_id = pmt.meal_time_id
GROUP BY p.plan_id, p.plan_name, p.total_calories
ORDER BY p.plan_id;

-- ============================================================================
-- 7. List active plan assignments per client.
-- ============================================================================

SELECT
    c.client_id,
    CONCAT(u.name, ' ', u.last_name) AS client_name,
    p.plan_id,
    p.plan_name,
    pa.start_date,
    pa.end_date,
    pa.status
FROM plan_assignment AS pa
JOIN client AS c
    ON c.client_id = pa.client_id
JOIN app_user AS u
    ON u.user_id = c.user_id
JOIN user_plan AS p
    ON p.plan_id = pa.plan_id
WHERE pa.status = 'ACTIVE'
ORDER BY c.client_id, pa.start_date;

-- ============================================================================
-- 8. List recipes with their products.
-- ============================================================================

SELECT
    r.recipe_id,
    CONCAT(u.name, ' ', u.last_name) AS client_name,
    pr.bar_code,
    pr.product_name
FROM recipe AS r
JOIN client AS c
    ON c.client_id = r.client_id
JOIN app_user AS u
    ON u.user_id = c.user_id
JOIN recipe_product AS rp
    ON rp.recipe_id = r.recipe_id
JOIN product AS pr
    ON pr.bar_code = rp.product_code
ORDER BY r.recipe_id, pr.product_name;

-- ============================================================================
-- 9. List client measurements ordered by date.
-- ============================================================================

SELECT
    c.client_id,
    CONCAT(u.name, ' ', u.last_name) AS client_name,
    m.measure_date,
    m.weight,
    m.body_mass_index,
    m.muscle,
    m.fat,
    m.neck,
    m.waist,
    m.hip
FROM measure AS m
JOIN client AS c
    ON c.client_id = m.client_id
JOIN app_user AS u
    ON u.user_id = c.user_id
ORDER BY c.client_id, m.measure_date;

-- ============================================================================
-- 10. List daily consume records with client, meal time, and plan.
-- ============================================================================

SELECT
    dc.consume_date,
    c.client_id,
    CONCAT(u.name, ' ', u.last_name) AS client_name,
    dmt.plan_meal_time_id,
    mt.meal_time_id,
    mt.meal_type,
    p.plan_name,
    dc.total_calories
FROM daily_consume AS dc
JOIN daily_meal_time AS dmt
    ON dmt.consume_date = dc.consume_date
JOIN client AS c
    ON c.client_id = dmt.client_id
JOIN app_user AS u
    ON u.user_id = c.user_id
JOIN plan_meal_time AS pmt
    ON pmt.plan_meal_time_id = dmt.plan_meal_time_id
JOIN user_plan AS p
    ON p.plan_id = pmt.plan_id
JOIN meal_time AS mt
    ON mt.meal_time_id = dmt.meal_time_id
WHERE dc.client_id = dmt.client_id
ORDER BY dc.consume_date, c.client_id, dmt.plan_meal_time_id;

-- ============================================================================
-- 11. Show products created by each user.
-- ============================================================================

SELECT
    u.user_id,
    CONCAT(u.name, ' ', u.last_name) AS creator_name,
    COUNT(pr.bar_code) AS product_count,
    STRING_AGG(pr.product_name, ', ') WITHIN GROUP (ORDER BY pr.product_name) AS products
FROM app_user AS u
JOIN product AS pr
    ON pr.user_id = u.user_id
GROUP BY u.user_id, u.name, u.last_name
ORDER BY u.user_id;

-- ============================================================================
-- 12. Verify rows from relationship and dependent tables.
-- ============================================================================

SELECT
    'recipe_product' AS relationship_table,
    CAST(rp.recipe_id AS VARCHAR(40)) AS parent_id,
    rp.product_code AS child_id
FROM recipe_product AS rp
UNION ALL
SELECT
    'meal_time_product' AS relationship_table,
    CAST(mtp.meal_time_id AS VARCHAR(40)) AS parent_id,
    mtp.product_code AS child_id
FROM meal_time_product AS mtp
UNION ALL
SELECT
    'nutritionist_client' AS relationship_table,
    CAST(nc.nutritionist_code AS VARCHAR(40)) AS parent_id,
    CAST(nc.client_id AS VARCHAR(40)) AS child_id
FROM nutritionist_client AS nc
UNION ALL
SELECT
    'plan_meal_time' AS relationship_table,
    CAST(pmt.plan_id AS VARCHAR(40)) AS parent_id,
    CAST(pmt.meal_time_id AS VARCHAR(40)) AS child_id
FROM plan_meal_time AS pmt
UNION ALL
SELECT
    'daily_meal_time' AS relationship_table,
    CAST(dmt.consume_date AS VARCHAR(40)) AS parent_id,
    CAST(dmt.plan_meal_time_id AS VARCHAR(40)) AS child_id
FROM daily_meal_time AS dmt
ORDER BY relationship_table, parent_id, child_id;
