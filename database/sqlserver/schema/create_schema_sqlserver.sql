/*
    NutriTEC Database Project
    Script: create_schema_sqlserver.sql
    Purpose: Create the SQL Server schema, constraints, and base validation.

    Notes:
      - Run this script after create_database_sqlserver.sql.
      - Tables are created first, then foreign keys are added at the end.
      - Table and column names use the current SQL Server naming convention.
*/

USE nutrition_db;
GO

-- ============================================================================
-- User And Profile Tables
-- ============================================================================

CREATE TABLE app_user (
    user_id INT IDENTITY(1,1) NOT NULL,
    birthday DATE NOT NULL,
    name VARCHAR(80) NOT NULL,
    last_name VARCHAR(80) NOT NULL,
    hash_password VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,

    CONSTRAINT pk_user PRIMARY KEY (user_id),
    CONSTRAINT uq_user_email UNIQUE (email),
    CONSTRAINT ck_user_name_not_blank CHECK (LTRIM(RTRIM(name)) <> ''),
    CONSTRAINT ck_user_last_name_not_blank CHECK (LTRIM(RTRIM(last_name)) <> ''),
    CONSTRAINT ck_user_hash_password_not_blank CHECK (LTRIM(RTRIM(hash_password)) <> ''),
    CONSTRAINT ck_user_birthday_valid CHECK (birthday >= '1900-01-01' AND birthday <= CONVERT(DATE, GETDATE())),
    CONSTRAINT ck_user_email_not_blank CHECK (LTRIM(RTRIM(email)) <> ''),
    CONSTRAINT ck_user_email_format CHECK (email LIKE '%_@_%._%' AND email NOT LIKE '% %')
);
GO

CREATE TABLE admin (
    admin_id INT IDENTITY(1,1) NOT NULL,
    user_id INT NOT NULL,

    CONSTRAINT pk_admin PRIMARY KEY (admin_id),
    CONSTRAINT uq_admin_user_id UNIQUE (user_id)
);
GO

CREATE TABLE nutritionist (
    nutritionist_code INT IDENTITY(1,1) NOT NULL,
    payment_method VARCHAR(20) NOT NULL,
    billing_frequency VARCHAR(20) NOT NULL CONSTRAINT df_nutritionist_billing_frequency DEFAULT 'MONTHLY',
    photo VARCHAR(255) NULL,
    address VARCHAR(255) NOT NULL,
    id_number VARCHAR(40) NOT NULL,
    encrypted_credit_card VARCHAR(255) NULL,
    weight NUMERIC(6, 2) NOT NULL,
    body_mass_index NUMERIC(5, 2) NOT NULL,
    user_id INT NOT NULL,

    CONSTRAINT pk_nutritionist PRIMARY KEY (nutritionist_code),
    CONSTRAINT uq_nutritionist_user_id UNIQUE (user_id),
    CONSTRAINT uq_nutritionist_id_number UNIQUE (id_number),
    CONSTRAINT ck_nutritionist_payment_method CHECK (
        payment_method IN ('CASH', 'CARD', 'TRANSFER', 'SINPE', 'PAYPAL')
    ),
    CONSTRAINT ck_nutritionist_billing_frequency CHECK (
        billing_frequency IN ('WEEKLY', 'MONTHLY', 'ANNUAL')
    ),
    CONSTRAINT ck_nutritionist_photo_not_blank CHECK (
        photo IS NULL OR LTRIM(RTRIM(photo)) <> ''
    ),
    CONSTRAINT ck_nutritionist_address_not_blank CHECK (LTRIM(RTRIM(address)) <> ''),
    CONSTRAINT ck_nutritionist_id_number_not_blank CHECK (LTRIM(RTRIM(id_number)) <> ''),
    CONSTRAINT ck_nutritionist_encrypted_credit_card_not_blank CHECK (
        encrypted_credit_card IS NULL OR LTRIM(RTRIM(encrypted_credit_card)) <> ''
    ),
    CONSTRAINT ck_nutritionist_weight_positive CHECK (weight > 0),
    CONSTRAINT ck_nutritionist_body_mass_index_positive CHECK (body_mass_index > 0)
);
GO

CREATE TABLE client (
    client_id INT IDENTITY(1,1) NOT NULL,
    max_daily_calories NUMERIC(10, 2) NOT NULL,
    country VARCHAR(80) NOT NULL,
    user_id INT NOT NULL,

    CONSTRAINT pk_client PRIMARY KEY (client_id),
    CONSTRAINT uq_client_user_id UNIQUE (user_id),
    CONSTRAINT ck_client_max_daily_calories_positive CHECK (max_daily_calories > 0),
    CONSTRAINT ck_client_country_not_blank CHECK (LTRIM(RTRIM(country)) <> '')
);
GO

CREATE TABLE nutritionist_client (
    start_date DATE NOT NULL,
    end_date DATE NULL,
    status VARCHAR(20) NOT NULL,
    nutritionist_code INT NOT NULL,
    client_id INT NOT NULL,

    CONSTRAINT pk_nutritionist_client PRIMARY KEY (nutritionist_code, client_id, start_date),
    CONSTRAINT ck_nutritionist_client_date_range CHECK (
        end_date IS NULL OR end_date >= start_date
    ),
    CONSTRAINT ck_nutritionist_client_status CHECK (
        status IN ('ACTIVE', 'PAUSED', 'FINISHED', 'CANCELLED')
    )
);
GO

-- ============================================================================
-- Plans And Meals
-- ============================================================================

CREATE TABLE nutrition_plan (
    plan_id INT IDENTITY(1,1) NOT NULL,
    plan_name VARCHAR(120) NOT NULL,
    total_calories NUMERIC(10, 2) NOT NULL DEFAULT 0,
    nutritionist_code INT NOT NULL,

    CONSTRAINT pk_nutrition_plan PRIMARY KEY (plan_id),
    CONSTRAINT ck_nutrition_plan_name_not_blank CHECK (LTRIM(RTRIM(plan_name)) <> ''),
    CONSTRAINT ck_nutrition_plan_total_calories_non_negative CHECK (total_calories >= 0)
);
GO

CREATE TABLE plan_assignment (
    assignment_id INT IDENTITY(1,1) NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NULL,
    assignment_status VARCHAR(20) NOT NULL,
    plan_id INT NOT NULL,
    client_id INT NOT NULL,

    CONSTRAINT pk_plan_assignment PRIMARY KEY (assignment_id),
    CONSTRAINT ck_plan_assignment_date_range CHECK (
        end_date IS NULL OR end_date >= start_date
    ),
    CONSTRAINT ck_plan_assignment_status CHECK (
        assignment_status IN ('ACTIVE', 'PAUSED', 'FINISHED', 'CANCELLED')
    )
);
GO

-- A filtered unique index preserves assignment history while allowing only one active plan per client.
-- QUOTED_IDENTIFIER ON is required by SQL Server for filtered indexes.
SET QUOTED_IDENTIFIER ON;
CREATE UNIQUE INDEX uq_plan_assignment_active_client
ON plan_assignment (client_id)
WHERE assignment_status = 'ACTIVE';
GO

CREATE TABLE meal_time (
    meal_time_id INT IDENTITY(1,1) NOT NULL,
    meal_type VARCHAR(20) NOT NULL,

    CONSTRAINT pk_meal_time PRIMARY KEY (meal_time_id),
    CONSTRAINT ck_meal_time_type CHECK (
        meal_type IN ('BREAKFAST', 'LUNCH', 'DINNER', 'SNACK', 'OTHER')
    )
);
GO

CREATE TABLE plan_meal_time (
    plan_meal_time_id INT IDENTITY(1,1) NOT NULL,
    meal_time_id INT NOT NULL,
    plan_id INT NOT NULL,

    CONSTRAINT pk_plan_meal_time PRIMARY KEY (plan_meal_time_id),
    CONSTRAINT uq_plan_meal_time_plan_meal UNIQUE (plan_id, meal_time_id)
);
GO

-- ============================================================================
-- Recipes And Products
-- ============================================================================

CREATE TABLE recipe (
    recipe_id INT IDENTITY(1,1) NOT NULL,
    recipe_name VARCHAR(120) NOT NULL,
    total_calories NUMERIC(10, 2) NOT NULL DEFAULT 0,
    client_id INT NOT NULL,

    CONSTRAINT pk_recipe PRIMARY KEY (recipe_id),
    CONSTRAINT ck_recipe_name_not_blank CHECK (LTRIM(RTRIM(recipe_name)) <> ''),
    CONSTRAINT ck_recipe_total_calories_non_negative CHECK (total_calories >= 0)
);
GO

CREATE TABLE product (
    bar_code VARCHAR(40) NOT NULL,
    portion_unit VARCHAR(30) NOT NULL,
    sodium NUMERIC(10, 2) NOT NULL DEFAULT 0,
    product_status VARCHAR(20) NOT NULL,
    iron NUMERIC(10, 2) NOT NULL DEFAULT 0,
    calcium NUMERIC(10, 2) NOT NULL DEFAULT 0,
    vitamins VARCHAR(120) NOT NULL,
    portion_size NUMERIC(10, 2) NOT NULL,
    calories NUMERIC(10, 2) NOT NULL DEFAULT 0,
    protein NUMERIC(10, 2) NOT NULL DEFAULT 0,
    carbohydrates NUMERIC(10, 2) NOT NULL DEFAULT 0,
    fat NUMERIC(10, 2) NOT NULL DEFAULT 0,
    product_name VARCHAR(120) NOT NULL,
    user_id INT NOT NULL,

    CONSTRAINT pk_product PRIMARY KEY (bar_code),
    CONSTRAINT ck_product_bar_code_not_blank CHECK (LTRIM(RTRIM(bar_code)) <> ''),
    CONSTRAINT ck_product_portion_unit_not_blank CHECK (LTRIM(RTRIM(portion_unit)) <> ''),
    CONSTRAINT ck_product_status CHECK (
        product_status IN ('ACTIVE', 'PENDING_REVIEW', 'REJECTED')
    ),
    CONSTRAINT ck_product_iron_non_negative CHECK (iron >= 0),
    CONSTRAINT ck_product_calcium_non_negative CHECK (calcium >= 0),
    CONSTRAINT ck_product_vitamins_not_blank CHECK (LTRIM(RTRIM(vitamins)) <> ''),
    CONSTRAINT ck_product_portion_size_positive CHECK (portion_size > 0),
    CONSTRAINT ck_product_calories_non_negative CHECK (calories >= 0),
    CONSTRAINT ck_product_protein_non_negative CHECK (protein >= 0),
    CONSTRAINT ck_product_carbohydrates_non_negative CHECK (carbohydrates >= 0),
    CONSTRAINT ck_product_fat_non_negative CHECK (fat >= 0),
    CONSTRAINT ck_product_sodium_non_negative CHECK (sodium >= 0),
    CONSTRAINT ck_product_name_not_blank CHECK (LTRIM(RTRIM(product_name)) <> '')
);
GO

CREATE TABLE recipe_product (
    recipe_id INT NOT NULL,
    product_code VARCHAR(40) NOT NULL,
    quantity NUMERIC(10, 2) NOT NULL,

    CONSTRAINT pk_recipe_product PRIMARY KEY (recipe_id, product_code),
    CONSTRAINT ck_recipe_product_quantity_positive CHECK (quantity > 0)
);
GO

CREATE TABLE meal_time_product (
    meal_time_id INT NOT NULL,
    product_code VARCHAR(40) NOT NULL,
    calories NUMERIC(10, 2) NOT NULL,
    quantity NUMERIC(10, 2) NOT NULL,

    CONSTRAINT pk_meal_time_product PRIMARY KEY (meal_time_id, product_code),
    CONSTRAINT ck_meal_time_product_calories_non_negative CHECK (calories >= 0),
    CONSTRAINT ck_meal_time_product_quantity_positive CHECK (quantity > 0)
);
GO

-- ============================================================================
-- Client Tracking
-- ============================================================================

CREATE TABLE measure (
    measure_datetime DATETIME2 NOT NULL,
    neck NUMERIC(6, 2) NOT NULL,
    muscle_percentage NUMERIC(5, 2) NOT NULL,
    body_weight NUMERIC(6, 2) NOT NULL,
    hip NUMERIC(6, 2) NOT NULL,
    waist NUMERIC(6, 2) NOT NULL,
    fat_percentage NUMERIC(5, 2) NOT NULL,
    body_mass_index NUMERIC(5, 2) NOT NULL,
    client_id INT NOT NULL,

    CONSTRAINT pk_measure PRIMARY KEY (client_id, measure_datetime),
    CONSTRAINT ck_measure_neck_positive CHECK (neck > 0),
    CONSTRAINT ck_measure_muscle_percentage_range CHECK (muscle_percentage BETWEEN 0 AND 100),
    CONSTRAINT ck_measure_body_weight_positive CHECK (body_weight > 0),
    CONSTRAINT ck_measure_hip_positive CHECK (hip > 0),
    CONSTRAINT ck_measure_waist_positive CHECK (waist > 0),
    CONSTRAINT ck_measure_fat_percentage_range CHECK (fat_percentage BETWEEN 0 AND 100),
    CONSTRAINT ck_measure_body_mass_index_positive CHECK (body_mass_index > 0),
    CONSTRAINT ck_measure_body_percentage_total CHECK (muscle_percentage + fat_percentage <= 100)
);
GO

CREATE TABLE daily_consume (
    consume_date DATE NOT NULL,
    total_calories NUMERIC(10, 2) NOT NULL,
    client_id INT NOT NULL,

    CONSTRAINT pk_daily_consume PRIMARY KEY (client_id, consume_date),
    CONSTRAINT ck_daily_consume_total_calories_non_negative CHECK (total_calories >= 0)
);
GO

CREATE TABLE daily_meal_time (
    daily_meal_time_id INT IDENTITY(1,1) NOT NULL,
    client_id INT NOT NULL,
    consume_date DATE NOT NULL,
    meal_time_id INT NOT NULL,

    CONSTRAINT pk_daily_meal_time PRIMARY KEY (daily_meal_time_id)
);
GO

-- ============================================================================
-- Foreign Keys
-- ============================================================================

ALTER TABLE admin
ADD CONSTRAINT FK_ADMIN_APP_USER
FOREIGN KEY (user_id)
REFERENCES app_user (user_id);
GO

ALTER TABLE nutritionist
ADD CONSTRAINT FK_NUTRITIONIST_APP_USER
FOREIGN KEY (user_id)
REFERENCES app_user (user_id);
GO

ALTER TABLE client
ADD CONSTRAINT FK_CLIENT_APP_USER
FOREIGN KEY (user_id)
REFERENCES app_user (user_id);
GO

ALTER TABLE nutritionist_client
ADD CONSTRAINT FK_NUTRITIONIST_CLIENT_NUTRITIONIST
FOREIGN KEY (nutritionist_code)
REFERENCES nutritionist (nutritionist_code);
GO

ALTER TABLE nutritionist_client
ADD CONSTRAINT FK_NUTRITIONIST_CLIENT_CLIENT
FOREIGN KEY (client_id)
REFERENCES client (client_id);
GO

ALTER TABLE nutrition_plan
ADD CONSTRAINT FK_NUTRITION_PLAN_NUTRITIONIST
FOREIGN KEY (nutritionist_code)
REFERENCES nutritionist (nutritionist_code);
GO

ALTER TABLE plan_assignment
ADD CONSTRAINT FK_PLAN_ASSIGNMENT_NUTRITION_PLAN
FOREIGN KEY (plan_id)
REFERENCES nutrition_plan (plan_id);
GO

ALTER TABLE plan_assignment
ADD CONSTRAINT FK_PLAN_ASSIGNMENT_CLIENT
FOREIGN KEY (client_id)
REFERENCES client (client_id);
GO

ALTER TABLE plan_meal_time
ADD CONSTRAINT FK_PLAN_MEAL_TIME_MEAL_TIME
FOREIGN KEY (meal_time_id)
REFERENCES meal_time (meal_time_id);
GO

ALTER TABLE plan_meal_time
ADD CONSTRAINT FK_PLAN_MEAL_TIME_NUTRITION_PLAN
FOREIGN KEY (plan_id)
REFERENCES nutrition_plan (plan_id);
GO

ALTER TABLE recipe
ADD CONSTRAINT FK_RECIPE_CLIENT
FOREIGN KEY (client_id)
REFERENCES client (client_id);
GO

ALTER TABLE product
ADD CONSTRAINT FK_PRODUCT_APP_USER
FOREIGN KEY (user_id)
REFERENCES app_user (user_id);
GO

ALTER TABLE recipe_product
ADD CONSTRAINT FK_RECIPE_PRODUCT_RECIPE
FOREIGN KEY (recipe_id)
REFERENCES recipe (recipe_id);
GO

ALTER TABLE recipe_product
ADD CONSTRAINT FK_RECIPE_PRODUCT_PRODUCT
FOREIGN KEY (product_code)
REFERENCES product (bar_code);
GO

ALTER TABLE meal_time_product
ADD CONSTRAINT FK_MEAL_TIME_PRODUCT_MEAL_TIME
FOREIGN KEY (meal_time_id)
REFERENCES meal_time (meal_time_id);
GO

ALTER TABLE meal_time_product
ADD CONSTRAINT FK_MEAL_TIME_PRODUCT_PRODUCT
FOREIGN KEY (product_code)
REFERENCES product (bar_code);
GO

ALTER TABLE measure
ADD CONSTRAINT FK_MEASURE_CLIENT
FOREIGN KEY (client_id)
REFERENCES client (client_id);
GO

ALTER TABLE daily_consume
ADD CONSTRAINT FK_DAILY_CONSUME_CLIENT
FOREIGN KEY (client_id)
REFERENCES client (client_id);
GO

ALTER TABLE daily_meal_time
ADD CONSTRAINT FK_DAILY_MEAL_TIME_CLIENT
FOREIGN KEY (client_id)
REFERENCES client (client_id);
GO

ALTER TABLE daily_meal_time
ADD CONSTRAINT FK_DAILY_MEAL_TIME_DAILY_CONSUME
FOREIGN KEY (client_id, consume_date)
REFERENCES daily_consume (client_id, consume_date);
GO

ALTER TABLE daily_meal_time
ADD CONSTRAINT FK_DAILY_MEAL_TIME_MEAL_TIME
FOREIGN KEY (meal_time_id)
REFERENCES meal_time (meal_time_id);
GO
