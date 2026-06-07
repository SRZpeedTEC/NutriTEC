/*
    NutriTEC Database Project
    Script: seed_data.sql
    Purpose: Insert realistic seed data that satisfies all schema constraints.

    Notes:
      - Run this script after 01_create_schema.sql.
      - Fixed identity values are used so sample queries are easy to review.
      - PostgreSQL identity sequences are reset at the end of each seeded table.
*/

-- ============================================================================
-- Application Users
-- ============================================================================

INSERT INTO app_user (
    user_id,
    birthday,
    name,
    last_name,
    hash_password,
    age,
    email
)
OVERRIDING SYSTEM VALUE
VALUES
    (1, DATE '1985-03-14', 'Andrea', 'Mora', 'hash_admin_001', 41, 'andrea.mora@nutritec.test'),
    (2, DATE '1989-07-22', 'Carlos', 'Vargas', 'hash_nutri_001', 36, 'carlos.vargas@nutritec.test'),
    (3, DATE '1991-11-08', 'Mariana', 'Solis', 'hash_nutri_002', 34, 'mariana.solis@nutritec.test'),
    (4, DATE '1997-02-05', 'Daniel', 'Rojas', 'hash_client_001', 29, 'daniel.rojas@nutritec.test'),
    (5, DATE '1994-09-17', 'Sofia', 'Castro', 'hash_client_002', 31, 'sofia.castro@nutritec.test'),
    (6, DATE '1988-12-01', 'Luis', 'Herrera', 'hash_client_003', 37, 'luis.herrera@nutritec.test'),
    (7, DATE '1999-04-26', 'Valeria', 'Jimenez', 'hash_user_007', 27, 'valeria.jimenez@nutritec.test');

SELECT setval(pg_get_serial_sequence('app_user', 'user_id'), 7, true);

-- ============================================================================
-- User Profiles
-- ============================================================================

INSERT INTO admin (admin_id, user_id)
OVERRIDING SYSTEM VALUE
VALUES
    (1, 1);

SELECT setval(pg_get_serial_sequence('admin', 'admin_id'), 1, true);

INSERT INTO nutritionist (
    nutritionist_code,
    payment_method,
    photo,
    address,
    id_number,
    encrypted_credit_card,
    body_weight,
    body_mass_index,
    user_id
)
OVERRIDING SYSTEM VALUE
VALUES
    (1, 'CARD', 'photos/carlos-vargas.jpg', 'San Pedro, San Jose', 'CR-108540221', 'enc_card_carlos_001', 76.50, 24.20, 2),
    (2, 'SINPE', 'photos/mariana-solis.jpg', 'Cartago Centro, Cartago', 'CR-207760554', 'enc_card_mariana_002', 62.10, 22.40, 3);

SELECT setval(pg_get_serial_sequence('nutritionist', 'nutritionist_code'), 2, true);

INSERT INTO client (client_id, max_daily_calories, country, user_id)
OVERRIDING SYSTEM VALUE
VALUES
    (1, 2200.00, 'Costa Rica', 4),
    (2, 1850.00, 'Costa Rica', 5),
    (3, 2400.00, 'Panama', 6);

SELECT setval(pg_get_serial_sequence('client', 'client_id'), 3, true);

INSERT INTO nutritionist_client (
    start_date,
    end_date,
    relation_status,
    nutritionist_code,
    client_id
)
VALUES
    (DATE '2026-01-10', NULL, 'ACTIVE', 1, 1),
    (DATE '2026-02-01', NULL, 'ACTIVE', 1, 2),
    (DATE '2025-10-15', DATE '2026-03-31', 'FINISHED', 2, 2),
    (DATE '2026-03-05', NULL, 'ACTIVE', 2, 3);

-- ============================================================================
-- Plans And Assignments
-- ============================================================================

INSERT INTO plan (plan_id, plan_name, total_calories, nutritionist_code)
OVERRIDING SYSTEM VALUE
VALUES
    (1, 'Balanced Maintenance Plan', 1475.00, 1),
    (2, 'High Protein Training Plan', 1850.00, 2);

SELECT setval(pg_get_serial_sequence('plan', 'plan_id'), 2, true);

INSERT INTO plan_assignment (
    assignment_id,
    start_date,
    end_date,
    assignment_status,
    plan_id,
    client_id
)
OVERRIDING SYSTEM VALUE
VALUES
    (1, DATE '2026-01-15', NULL, 'ACTIVE', 1, 1),
    (2, DATE '2026-02-10', NULL, 'ACTIVE', 1, 2),
    (3, DATE '2026-03-10', NULL, 'ACTIVE', 2, 3),
    (4, DATE '2025-11-01', DATE '2026-01-31', 'FINISHED', 2, 2);

SELECT setval(pg_get_serial_sequence('plan_assignment', 'assignment_id'), 4, true);

INSERT INTO meal_time (meal_time_id, meal_type, plan_id)
OVERRIDING SYSTEM VALUE
VALUES
    (1, 'BREAKFAST', 1),
    (2, 'LUNCH', 1),
    (3, 'DINNER', 1),
    (4, 'SNACK', 1),
    (5, 'BREAKFAST', 2),
    (6, 'LUNCH', 2),
    (7, 'DINNER', 2),
    (8, 'SNACK', 2);

SELECT setval(pg_get_serial_sequence('meal_time', 'meal_time_id'), 8, true);

-- ============================================================================
-- Products And Meal Composition
-- ============================================================================

INSERT INTO product (
    bar_code,
    portion_unit,
    sodium,
    product_status,
    iron,
    calcium,
    vitamins,
    portion_size,
    calories,
    protein,
    carbohydrates,
    fat,
    product_name,
    user_id
)
VALUES
    ('P-0001', 'g', 2.00, 'ACTIVE', 1.20, 15.00, 4.00, 100.00, 120.00, 4.00, 21.00, 2.00, 'Greek Yogurt', 2),
    ('P-0002', 'g', 1.00, 'ACTIVE', 0.80, 8.00, 6.00, 100.00, 95.00, 2.50, 20.00, 0.50, 'Fresh Berries', 2),
    ('P-0003', 'g', 60.00, 'ACTIVE', 1.50, 12.00, 2.00, 100.00, 165.00, 31.00, 0.00, 3.60, 'Grilled Chicken Breast', 3),
    ('P-0004', 'g', 5.00, 'ACTIVE', 2.80, 20.00, 3.00, 100.00, 120.00, 4.40, 21.30, 1.90, 'Brown Rice', 3),
    ('P-0005', 'g', 7.00, 'ACTIVE', 1.00, 9.00, 5.00, 100.00, 160.00, 2.00, 8.50, 14.70, 'Avocado', 2),
    ('P-0006', 'g', 70.00, 'ACTIVE', 2.10, 18.00, 2.50, 100.00, 208.00, 20.00, 0.00, 13.00, 'Salmon Fillet', 3);

INSERT INTO meal_time_product (
    meal_time_id,
    product_code,
    calories,
    quantity
)
VALUES
    (1, 'P-0001', 180.00, 1.50),
    (1, 'P-0002', 95.00, 1.00),
    (2, 'P-0003', 330.00, 2.00),
    (2, 'P-0004', 240.00, 2.00),
    (3, 'P-0006', 416.00, 2.00),
    (3, 'P-0005', 160.00, 1.00),
    (4, 'P-0002', 54.00, 0.57),
    (5, 'P-0001', 240.00, 2.00),
    (5, 'P-0002', 95.00, 1.00),
    (6, 'P-0003', 495.00, 3.00),
    (6, 'P-0004', 300.00, 2.50),
    (7, 'P-0006', 520.00, 2.50),
    (7, 'P-0005', 80.00, 0.50),
    (8, 'P-0001', 120.00, 1.00);

-- ============================================================================
-- Recipes
-- ============================================================================

INSERT INTO recipe (recipe_id, nutritional_values, client_id)
OVERRIDING SYSTEM VALUE
VALUES
    (1, 'High-protein yogurt bowl with berries. Approx. 275 kcal.', 1),
    (2, 'Chicken and brown rice lunch bowl. Approx. 570 kcal.', 2),
    (3, 'Salmon and avocado dinner plate. Approx. 576 kcal.', 3);

SELECT setval(pg_get_serial_sequence('recipe', 'recipe_id'), 3, true);

INSERT INTO recipe_product (recipe_id, product_code)
VALUES
    (1, 'P-0001'),
    (1, 'P-0002'),
    (2, 'P-0003'),
    (2, 'P-0004'),
    (3, 'P-0005'),
    (3, 'P-0006');

-- ============================================================================
-- Client Measurements
-- ============================================================================

INSERT INTO measure (
    measure_datetime,
    neck,
    muscle_percentage,
    body_weight,
    hip,
    waist,
    fat_percentage,
    body_mass_index,
    client_id
)
VALUES
    (TIMESTAMP '2026-01-15 08:00:00', 38.20, 41.50, 79.40, 98.00, 88.00, 22.50, 25.10, 1),
    (TIMESTAMP '2026-02-15 08:10:00', 37.80, 42.10, 78.20, 97.20, 86.50, 21.80, 24.70, 1),
    (TIMESTAMP '2026-02-10 09:00:00', 33.50, 36.20, 63.10, 94.00, 75.40, 26.30, 23.20, 2),
    (TIMESTAMP '2026-03-12 09:15:00', 33.20, 36.90, 62.40, 93.50, 74.80, 25.70, 22.90, 2),
    (TIMESTAMP '2026-03-10 07:45:00', 39.00, 44.80, 84.50, 101.00, 90.00, 20.20, 24.90, 3);

-- ============================================================================
-- Daily Consumption
-- ============================================================================

INSERT INTO daily_consume (
    consume_date,
    total_calories,
    client_id,
    meal_time_id
)
VALUES
    (DATE '2026-04-01', 275.00, 1, 1),
    (DATE '2026-04-01', 570.00, 1, 2),
    (DATE '2026-04-01', 576.00, 1, 3),
    (DATE '2026-04-02', 250.00, 2, 1),
    (DATE '2026-04-02', 550.00, 2, 2),
    (DATE '2026-04-03', 335.00, 3, 5),
    (DATE '2026-04-03', 795.00, 3, 6),
    (DATE '2026-04-03', 600.00, 3, 7);
