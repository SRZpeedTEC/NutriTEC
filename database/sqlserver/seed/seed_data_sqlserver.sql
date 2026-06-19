USE nutrition_db;
GO

/*
    NutriTEC Database Project
    Script: seed_data_sqlserver.sql
    Purpose: Insert seed data in foreign-key-safe order.

    Prerequisites:
      - Run the schema, view, and trigger scripts before this seed script.

    Order:
      1. app_user
      2. admin, nutritionist, client
      3. product, meal_time, nutrition_plan, recipe
      4. intermediate and dependent tables
*/

-- ============================================================================
-- Users
-- ============================================================================

SET IDENTITY_INSERT app_user ON;

INSERT INTO app_user (
    user_id,
    birthday,
    name,
    last_name,
    hash_password,
    email
)
VALUES
    (1, '1985-03-14', 'Andrea', 'Mora', 'hash_admin_001', 'andrea.mora@nutritec.test'),
    (2, '1989-07-22', 'Carlos', 'Vargas', 'hash_nutri_001', 'carlos.vargas@nutritec.test'),
    (3, '1991-11-08', 'Mariana', 'Solis', 'hash_nutri_002', 'mariana.solis@nutritec.test'),
    (4, '1997-02-05', 'Daniel', 'Rojas', 'hash_client_001', 'daniel.rojas@nutritec.test'),
    (5, '1994-09-17', 'Sofia', 'Castro', 'hash_client_002', 'sofia.castro@nutritec.test'),
    (6, '1988-12-01', 'Luis', 'Herrera', 'hash_client_003', 'luis.herrera@nutritec.test');

SET IDENTITY_INSERT app_user OFF;
GO

-- ============================================================================
-- User Profiles
-- ============================================================================

SET IDENTITY_INSERT admin ON;

INSERT INTO admin (admin_id, user_id)
VALUES
    (1, 1);

SET IDENTITY_INSERT admin OFF;
GO

SET IDENTITY_INSERT nutritionist ON;

INSERT INTO nutritionist (
    nutritionist_code,
    payment_method,
    photo,
    address,
    id_number,
    encrypted_credit_card,
    weight,
    body_mass_index,
    user_id
)
VALUES
    (1, 'CARD', 'photos/carlos-vargas.jpg', 'San Pedro, San Jose', 'CR-108540221', 'enc_card_carlos_001', 76.50, 24.20, 2),
    (2, 'SINPE', 'photos/mariana-solis.jpg', 'Cartago Centro, Cartago', 'CR-207760554', 'enc_card_mariana_002', 62.10, 22.40, 3);

SET IDENTITY_INSERT nutritionist OFF;
GO

SET IDENTITY_INSERT client ON;

INSERT INTO client (client_id, max_daily_calories, country, user_id)
VALUES
    (1, 2200.00, 'Costa Rica', 4),
    (2, 1850.00, 'Costa Rica', 5),
    (3, 2400.00, 'Panama', 6);

SET IDENTITY_INSERT client OFF;
GO

-- ============================================================================
-- Products, Meal Times, Plans, And Recipes
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
    ('P-0005', 'g', 7.00, 'ACTIVE', 1.00, 9.00, 5.00, 100.00, 160.00, 2.00, 8.50, 14.70, 'Avocado', 6),
    ('P-0006', 'g', 70.00, 'ACTIVE', 2.10, 18.00, 2.50, 100.00, 208.00, 20.00, 0.00, 13.00, 'Salmon Fillet', 6);
GO

SET IDENTITY_INSERT meal_time ON;

INSERT INTO meal_time (meal_time_id, meal_type)
VALUES
    -- IDs 1-5 are stable templates used by clients to select a meal type.
    (1, 'BREAKFAST'),
    (2, 'LUNCH'),
    (3, 'DINNER'),
    (4, 'SNACK'),
    (5, 'OTHER'),
    -- IDs 6-13 isolate each nutrition plan's product collection by meal type.
    (6, 'BREAKFAST'),
    (7, 'LUNCH'),
    (8, 'DINNER'),
    (9, 'SNACK'),
    (10, 'BREAKFAST'),
    (11, 'LUNCH'),
    (12, 'DINNER'),
    (13, 'SNACK'),
    -- IDs 14-21 isolate every seeded client, date, and meal-type consumption.
    (14, 'BREAKFAST'),
    (15, 'LUNCH'),
    (16, 'DINNER'),
    (17, 'SNACK'),
    (18, 'BREAKFAST'),
    (19, 'LUNCH'),
    (20, 'DINNER'),
    (21, 'SNACK');

SET IDENTITY_INSERT meal_time OFF;
GO

SET IDENTITY_INSERT nutrition_plan ON;

INSERT INTO nutrition_plan (plan_id, plan_name, total_calories, nutritionist_code)
VALUES
    (1, 'Balanced Maintenance Plan', 1475.00, 1),
    (2, 'High Protein Training Plan', 1850.00, 2);

SET IDENTITY_INSERT nutrition_plan OFF;
GO

SET IDENTITY_INSERT recipe ON;

INSERT INTO recipe (recipe_id, client_id)
VALUES
    (1, 1),
    (2, 2),
    (3, 3);

SET IDENTITY_INSERT recipe OFF;
GO

-- ============================================================================
-- Intermediate And Dependent Tables
-- ============================================================================

INSERT INTO nutritionist_client (
    start_date,
    end_date,
    status,
    nutritionist_code,
    client_id
)
VALUES
    ('2026-01-10', NULL, 'ACTIVE', 1, 1),
    ('2026-02-01', NULL, 'ACTIVE', 1, 2),
    ('2025-10-15', '2026-03-31', 'FINISHED', 2, 2),
    ('2026-03-05', NULL, 'ACTIVE', 2, 3);
GO

SET IDENTITY_INSERT plan_assignment ON;

INSERT INTO plan_assignment (
    assignment_id,
    start_date,
    end_date,
    assignment_status,
    plan_id,
    client_id
)
VALUES
    (1, '2026-01-15', NULL, 'ACTIVE', 1, 1),
    (2, '2026-02-10', NULL, 'ACTIVE', 1, 2),
    (3, '2026-03-10', NULL, 'ACTIVE', 2, 3),
    (4, '2025-11-01', '2026-01-31', 'FINISHED', 2, 2);

SET IDENTITY_INSERT plan_assignment OFF;
GO

SET IDENTITY_INSERT plan_meal_time ON;

INSERT INTO plan_meal_time (plan_meal_time_id, meal_time_id, plan_id)
VALUES
    (1, 6, 1),
    (2, 7, 1),
    (3, 8, 1),
    (4, 9, 1),
    (5, 10, 2),
    (6, 11, 2),
    (7, 12, 2),
    (8, 13, 2);

SET IDENTITY_INSERT plan_meal_time OFF;
GO

INSERT INTO recipe_product (recipe_id, product_code)
VALUES
    (1, 'P-0001'),
    (1, 'P-0002'),
    (2, 'P-0003'),
    (2, 'P-0004'),
    (3, 'P-0005'),
    (3, 'P-0006');
GO

INSERT INTO meal_time_product (
    meal_time_id,
    product_code,
    calories,
    quantity
)
VALUES
    -- Plan 1 details sum to its stored total of 1475 calories.
    (6, 'P-0001', 180.00, 1.50),
    (6, 'P-0002', 95.00, 1.00),
    (7, 'P-0003', 330.00, 2.00),
    (7, 'P-0004', 240.00, 2.00),
    (8, 'P-0006', 416.00, 2.00),
    (8, 'P-0005', 160.00, 1.00),
    (9, 'P-0002', 54.00, 0.57),
    -- Plan 2 details use separate meal-time instances and sum to 1850 calories.
    (10, 'P-0001', 240.00, 2.00),
    (10, 'P-0002', 190.00, 2.00),
    (11, 'P-0003', 495.00, 3.00),
    (11, 'P-0004', 360.00, 3.00),
    (12, 'P-0006', 416.00, 2.00),
    (13, 'P-0001', 8.40, 0.07),
    (13, 'P-0002', 140.60, 1.48),
    -- Daily details remain isolated while matching every seeded daily total.
    (14, 'P-0001', 180.00, 1.50),
    (14, 'P-0002', 95.00, 1.00),
    (15, 'P-0003', 330.00, 2.00),
    (15, 'P-0004', 240.00, 2.00),
    (16, 'P-0006', 416.00, 2.00),
    (16, 'P-0005', 160.00, 1.00),
    (17, 'P-0005', 800.00, 5.00),
    (18, 'P-0001', 240.00, 2.00),
    (18, 'P-0002', 190.00, 2.00),
    (19, 'P-0003', 495.00, 3.00),
    (19, 'P-0004', 360.00, 3.00),
    (20, 'P-0003', 165.00, 1.00),
    (20, 'P-0001', 21.60, 0.18),
    (20, 'P-0002', 258.40, 2.72),
    (21, 'P-0001', 180.00, 1.50),
    (21, 'P-0002', 95.00, 1.00);
GO

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
    ('2026-01-15 08:00:00', 38.20, 41.50, 79.40, 98.00, 88.00, 22.50, 25.10, 1),
    ('2026-02-15 08:10:00', 37.80, 42.10, 78.20, 97.20, 86.50, 21.80, 24.70, 1),
    ('2026-02-10 09:00:00', 33.50, 36.20, 63.10, 94.00, 75.40, 26.30, 23.20, 2),
    ('2026-03-12 09:15:00', 33.20, 36.90, 62.40, 93.50, 74.80, 25.70, 22.90, 2),
    ('2026-03-10 07:45:00', 39.00, 44.80, 84.50, 101.00, 90.00, 20.20, 24.90, 3);
GO

INSERT INTO daily_consume (
    consume_date,
    total_calories,
    client_id
)
VALUES
    ('2026-04-01', 1421.00, 1),
    ('2026-04-02', 800.00, 2),
    ('2026-04-03', 1730.00, 3),
    ('2026-04-04', 275.00, 1);
GO

INSERT INTO daily_meal_time (
    client_id,
    consume_date,
    meal_time_id
)
VALUES
    (1, '2026-04-01', 14),
    (1, '2026-04-01', 15),
    (1, '2026-04-01', 16),
    (2, '2026-04-02', 17),
    (3, '2026-04-03', 18),
    (3, '2026-04-03', 19),
    (3, '2026-04-03', 20),
    (1, '2026-04-04', 21);
GO
