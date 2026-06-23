USE nutrition_db;
GO

/*
    NutriTEC Database Project
    Script: seed_data_sqlserver_deploy.sql
    Purpose: Deploy seed

    Prerequisites:
      - Run create_schema_sqlserver.sql and
        create_programmability_objects_sqlserver.sql before this seed script.

    Content:
      - 3 app_user records
      - 1 admin
      - 1 nutritionist
      - 1 client
      - 1 active nutritionist-client relation
      - 4 client measures
      - 20 ACTIVE products 
      - 3 initial recipes for the client
      - Basic meal times, 1 nutrition plan, assignment, and sample daily consume rows

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
    (1, '1985-03-14', 'Andrea', 'Mora', '$2a$11$bgFVqqLArz2sD6eNs33e2O/aiEezYuWfwdlyGK.mJ1nZ9tPCQgGWK', 'andrea.mora@nutritec.test'),
    (2, '1989-07-22', 'Carlos', 'Vargas', '$2a$11$bgFVqqLArz2sD6eNs33e2O/aiEezYuWfwdlyGK.mJ1nZ9tPCQgGWK', 'carlos.vargas@nutritec.test'),
    (3, '1997-02-05', 'Daniel', 'Rojas', '$2a$11$bgFVqqLArz2sD6eNs33e2O/aiEezYuWfwdlyGK.mJ1nZ9tPCQgGWK', 'daniel.rojas@nutritec.test');

SET IDENTITY_INSERT app_user OFF;
GO

-- ============================================================================
-- Profiles
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
    billing_frequency,
    photo,
    address,
    id_number,
    encrypted_credit_card,
    weight,
    body_mass_index,
    user_id
)
VALUES
    (1, 'CARD', 'MONTHLY', 'photos/carlos-vargas.jpg', 'San Pedro, San Jose', 'CR-108540221', 'enc_card_carlos_001', 76.50, 24.20, 2);

SET IDENTITY_INSERT nutritionist OFF;
GO

SET IDENTITY_INSERT client ON;

INSERT INTO client (client_id, max_daily_calories, country, user_id)
VALUES
    (1, 2200.00, 'Costa Rica', 3);

SET IDENTITY_INSERT client OFF;
GO

INSERT INTO nutritionist_client (
    start_date,
    end_date,
    status,
    nutritionist_code,
    client_id
)
VALUES
    ('2026-01-01', NULL, 'ACTIVE', 1, 1);
GO

-- ============================================================================
-- Products
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
    ('P-0001', 'g', 5.00, 'ACTIVE', 0.70, 12.00, 'B1,B6', 100.00, 389.00, 16.90, 66.30, 6.90, 'Avena integral', 2),
    ('P-0002', 'g', 36.00, 'ACTIVE', 0.10, 110.00, 'B12,D', 100.00, 61.00, 3.50, 4.70, 3.30, 'Yogurt natural', 2),
    ('P-0003', 'g', 1.00, 'ACTIVE', 0.30, 6.00, 'C,B6', 100.00, 89.00, 1.10, 22.80, 0.30, 'Banano', 2),
    ('P-0004', 'g', 2.00, 'ACTIVE', 0.40, 16.00, 'C,K', 100.00, 57.00, 0.70, 14.50, 0.30, 'Manzana roja', 2),
    ('P-0005', 'g', 74.00, 'ACTIVE', 1.00, 11.00, 'B3,B6', 100.00, 165.00, 31.00, 0.00, 3.60, 'Pechuga de pollo', 2),
    ('P-0006', 'g', 7.00, 'ACTIVE', 1.50, 10.00, 'B1,B3', 100.00, 120.00, 2.60, 23.00, 0.90, 'Arroz integral', 2),
    ('P-0007', 'g', 5.00, 'ACTIVE', 1.50, 17.00, 'B6,C', 100.00, 86.00, 1.60, 20.10, 0.10, 'Camote horneado', 2),
    ('P-0008', 'g', 6.00, 'ACTIVE', 2.90, 49.00, 'A,K,C', 100.00, 23.00, 2.90, 3.60, 0.40, 'Espinaca fresca', 2),
    ('P-0009', 'g', 7.00, 'ACTIVE', 0.60, 12.00, 'E,K', 100.00, 160.00, 2.00, 8.50, 14.70, 'Aguacate', 2),
    ('P-0010', 'g', 2.00, 'ACTIVE', 0.80, 25.00, 'C,K', 100.00, 18.00, 0.90, 3.90, 0.20, 'Tomate', 2),
    ('P-0011', 'g', 70.00, 'ACTIVE', 2.10, 18.00, 'D,B12', 100.00, 208.00, 20.00, 0.00, 13.00, 'Filete de salmon', 2),
    ('P-0012', 'g', 38.00, 'ACTIVE', 1.20, 24.00, 'B9,B6', 100.00, 164.00, 8.90, 27.40, 2.60, 'Frijoles negros', 2),
    ('P-0013', 'g', 13.00, 'ACTIVE', 1.50, 47.00, 'B9,B1', 100.00, 116.00, 9.00, 20.10, 0.40, 'Lentejas cocidas', 2),
    ('P-0014', 'g', 54.00, 'ACTIVE', 2.70, 56.00, 'B12,D', 100.00, 155.00, 13.00, 1.10, 11.00, 'Huevo entero', 2),
    ('P-0015', 'g', 72.00, 'ACTIVE', 1.00, 15.00, 'B3,B6', 100.00, 132.00, 28.00, 0.00, 1.00, 'Atun en agua', 2),
    ('P-0016', 'g', 2.00, 'ACTIVE', 0.30, 5.00, 'C,Mn', 100.00, 50.00, 1.00, 12.00, 0.30, 'Fresas', 2),
    ('P-0017', 'g', 1.00, 'ACTIVE', 0.30, 6.00, 'C,K', 100.00, 57.00, 0.70, 14.50, 0.30, 'Arandanos', 2),
    ('P-0018', 'g', 6.00, 'ACTIVE', 0.60, 24.00, 'E,B2', 100.00, 579.00, 21.20, 21.60, 49.90, 'Almendras', 2),
    ('P-0019', 'ml', 2.00, 'ACTIVE', 0.00, 1.00, 'E,K', 15.00, 119.00, 0.00, 0.00, 13.50, 'Aceite de oliva', 2),
    ('P-0020', 'g', 5.00, 'ACTIVE', 4.60, 47.00, 'B9,Mg', 100.00, 120.00, 4.40, 21.30, 1.90, 'Quinoa cocida', 2);
GO

-- ============================================================================
-- Meal times, plan, assignment and plan products
-- ============================================================================

SET IDENTITY_INSERT meal_time ON;

INSERT INTO meal_time (meal_time_id, meal_type)
VALUES
    (1, 'BREAKFAST'),
    (2, 'LUNCH'),
    (3, 'DINNER'),
    (4, 'SNACK'),
    (5, 'OTHER');

SET IDENTITY_INSERT meal_time OFF;
GO

SET IDENTITY_INSERT nutrition_plan ON;

INSERT INTO nutrition_plan (plan_id, plan_name, total_calories, nutritionist_code)
VALUES
    (1, 'Plan balanceado inicial', 2180.00, 1);

SET IDENTITY_INSERT nutrition_plan OFF;
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
    (1, '2026-01-01', NULL, 'ACTIVE', 1, 1);

SET IDENTITY_INSERT plan_assignment OFF;
GO

SET IDENTITY_INSERT plan_meal_time ON;

INSERT INTO plan_meal_time (plan_meal_time_id, meal_time_id, plan_id)
VALUES
    (1, 1, 1),
    (2, 2, 1),
    (3, 3, 1),
    (4, 4, 1);

SET IDENTITY_INSERT plan_meal_time OFF;
GO

INSERT INTO meal_time_product (
    meal_time_id,
    product_code,
    calories,
    quantity
)
VALUES
    (1, 'P-0001', 194.50, 0.50),
    (1, 'P-0002', 122.00, 2.00),
    (1, 'P-0003', 89.00, 1.00),
    (2, 'P-0005', 247.50, 1.50),
    (2, 'P-0006', 240.00, 2.00),
    (2, 'P-0008', 23.00, 1.00),
    (3, 'P-0011', 312.00, 1.50),
    (3, 'P-0020', 240.00, 2.00),
    (3, 'P-0010', 18.00, 1.00),
    (4, 'P-0016', 75.00, 1.50),
    (4, 'P-0018', 173.70, 0.30);
GO

-- ============================================================================
-- Recipes
-- ============================================================================

SET IDENTITY_INSERT recipe ON;

INSERT INTO recipe (recipe_id, recipe_name, total_calories, client_id)
VALUES
    (1, 'Bowl de avena con frutas', 405.50, 1),
    (2, 'Pollo con arroz integral y espinaca', 510.50, 1),
    (3, 'Salmon con quinoa y aguacate', 712.00, 1);

SET IDENTITY_INSERT recipe OFF;
GO

INSERT INTO recipe_product (recipe_id, product_code, quantity)
VALUES
    (1, 'P-0001', 0.50),
    (1, 'P-0002', 1.00),
    (1, 'P-0003', 1.00),
    (1, 'P-0016', 0.50),
    (2, 'P-0005', 1.50),
    (2, 'P-0006', 2.00),
    (2, 'P-0008', 1.00),
    (3, 'P-0011', 1.50),
    (3, 'P-0020', 2.00),
    (3, 'P-0009', 1.00);
GO

-- ============================================================================
-- Client measures
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
    ('2026-01-05 08:00:00', 38.20, 41.50, 79.40, 98.00, 88.00, 22.50, 25.10, 1),
    ('2026-02-05 08:00:00', 37.90, 42.00, 78.30, 97.40, 86.90, 21.90, 24.70, 1),
    ('2026-03-05 08:00:00', 37.50, 42.80, 77.10, 96.80, 85.70, 21.10, 24.30, 1),
    ('2026-04-05 08:00:00', 37.20, 43.40, 75.90, 96.10, 84.80, 20.40, 23.90, 1);
GO

-- ============================================================================
-- Simple daily consume examples
-- ============================================================================

INSERT INTO daily_consume (
    consume_date,
    total_calories,
    client_id
)
VALUES
    ('2026-04-01', 1942.00, 1),
    ('2026-04-02', 2088.00, 1),
    ('2026-04-03', 1850.00, 1);
GO

INSERT INTO daily_meal_time (
    client_id,
    consume_date,
    meal_time_id
)
VALUES
    (1, '2026-04-01', 1),
    (1, '2026-04-01', 2),
    (1, '2026-04-01', 3),
    (1, '2026-04-02', 1),
    (1, '2026-04-02', 2),
    (1, '2026-04-02', 4),
    (1, '2026-04-03', 1),
    (1, '2026-04-03', 3);
GO
