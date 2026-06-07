/*
    NutriTEC Database Project
    Script: create_schema.sql
    Purpose: Create the relational schema, constraints, and basic validation.

    Notes:
      - SQL Server is the target SQL dialect.
      - Run this script after creating and selecting nutrition_db.
      - Tables are created in dependency order so foreign keys are valid.
      - Constraint names use pk_, fk_, ck_, and uq_ prefixes for reviewability.
*/

USE nutrition_db;
GO

-- ============================================================================
-- User And Profile Tables
-- ============================================================================

-- Stores authentication and personal information shared by all application roles.
CREATE TABLE app_user (
    user_id INT IDENTITY(1,1),
    birthday DATE NOT NULL,
    name VARCHAR(80) NOT NULL,
    last_name VARCHAR(80) NOT NULL,
    hash_password VARCHAR(255) NOT NULL,
    age INT NOT NULL,
    email VARCHAR(255) NOT NULL,

    CONSTRAINT pk_app_user PRIMARY KEY (user_id),
    CONSTRAINT uq_app_user_email UNIQUE (email),
    CONSTRAINT ck_app_user_name_not_blank CHECK (LTRIM(RTRIM(name)) <> ''),
    CONSTRAINT ck_app_user_last_name_not_blank CHECK (LTRIM(RTRIM(last_name)) <> ''),
    CONSTRAINT ck_app_user_hash_password_not_blank CHECK (LTRIM(RTRIM(hash_password)) <> ''),
    CONSTRAINT ck_app_user_age_positive CHECK (age > 0),
    CONSTRAINT ck_app_user_birthday_valid CHECK (birthday >= '1900-01-01'),
    CONSTRAINT ck_app_user_email_not_blank CHECK (LTRIM(RTRIM(email)) <> ''),
    CONSTRAINT ck_app_user_email_format CHECK (
        email LIKE '%_@_%._%' AND email NOT LIKE '% %'
    )
);
GO

-- Identifies users with administrative permissions.
CREATE TABLE admin (
    admin_id INT IDENTITY(1,1),
    user_id INT NOT NULL,

    CONSTRAINT pk_admin PRIMARY KEY (admin_id),
    CONSTRAINT uq_admin_user_id UNIQUE (user_id),
    CONSTRAINT fk_admin_app_user FOREIGN KEY (user_id)
        REFERENCES app_user (user_id)
        ON DELETE NO ACTION
);
GO

-- Stores professional profile details for nutritionists.
CREATE TABLE nutritionist (
    nutritionist_code INT IDENTITY(1,1),
    payment_method VARCHAR(20) NOT NULL,
    photo VARCHAR(255),
    address VARCHAR(255) NOT NULL,
    id_number VARCHAR(40) NOT NULL,
    encrypted_credit_card VARCHAR(255),
    body_weight NUMERIC(6, 2) NOT NULL,
    body_mass_index NUMERIC(5, 2) NOT NULL,
    user_id INT NOT NULL,

    CONSTRAINT pk_nutritionist PRIMARY KEY (nutritionist_code),
    CONSTRAINT uq_nutritionist_user_id UNIQUE (user_id),
    CONSTRAINT uq_nutritionist_id_number UNIQUE (id_number),
    CONSTRAINT ck_nutritionist_payment_method CHECK (
        payment_method IN ('CASH', 'CARD', 'TRANSFER', 'SINPE', 'PAYPAL')
    ),
    CONSTRAINT ck_nutritionist_photo_not_blank CHECK (
        photo IS NULL OR LTRIM(RTRIM(photo)) <> ''
    ),
    CONSTRAINT ck_nutritionist_address_not_blank CHECK (LTRIM(RTRIM(address)) <> ''),
    CONSTRAINT ck_nutritionist_id_number_not_blank CHECK (LTRIM(RTRIM(id_number)) <> ''),
    CONSTRAINT ck_nutritionist_encrypted_credit_card_not_blank CHECK (
        encrypted_credit_card IS NULL OR LTRIM(RTRIM(encrypted_credit_card)) <> ''
    ),
    CONSTRAINT ck_nutritionist_body_weight_positive CHECK (body_weight > 0),
    CONSTRAINT ck_nutritionist_body_mass_index_positive CHECK (body_mass_index > 0),
    CONSTRAINT fk_nutritionist_app_user FOREIGN KEY (user_id)
        REFERENCES app_user (user_id)
        ON DELETE NO ACTION
);
GO

-- Stores client-specific nutrition preferences and limits.
CREATE TABLE client (
    client_id INT IDENTITY(1,1),
    max_daily_calories NUMERIC(10, 2) NOT NULL,
    country VARCHAR(80) NOT NULL,
    user_id INT NOT NULL,

    CONSTRAINT pk_client PRIMARY KEY (client_id),
    CONSTRAINT uq_client_user_id UNIQUE (user_id),
    CONSTRAINT ck_client_max_daily_calories_positive CHECK (max_daily_calories > 0),
    CONSTRAINT ck_client_country_not_blank CHECK (LTRIM(RTRIM(country)) <> ''),
    CONSTRAINT fk_client_app_user FOREIGN KEY (user_id)
        REFERENCES app_user (user_id)
        ON DELETE NO ACTION
);
GO

-- Represents the professional relationship between nutritionists and clients.
CREATE TABLE nutritionist_client (
    start_date DATE NOT NULL,
    end_date DATE,
    relation_status VARCHAR(20) NOT NULL,
    nutritionist_code INT NOT NULL,
    client_id INT NOT NULL,

    CONSTRAINT pk_nutritionist_client PRIMARY KEY (
        nutritionist_code,
        client_id,
        start_date
    ),
    CONSTRAINT ck_nutritionist_client_date_range CHECK (
        end_date IS NULL OR end_date >= start_date
    ),
    CONSTRAINT ck_nutritionist_client_relation_status CHECK (
        relation_status IN ('ACTIVE', 'PAUSED', 'FINISHED', 'CANCELLED')
    ),
    CONSTRAINT fk_nutritionist_client_nutritionist FOREIGN KEY (nutritionist_code)
        REFERENCES nutritionist (nutritionist_code)
        ON DELETE NO ACTION,
    CONSTRAINT fk_nutritionist_client_client FOREIGN KEY (client_id)
        REFERENCES client (client_id)
        ON DELETE NO ACTION
);
GO

-- ============================================================================
-- Nutrition Plans
-- ============================================================================

-- Stores nutrition plans created by nutritionists.
-- total_calories is derived from meal_time_product rows and should be maintained
-- by application logic, a trigger, or a view rather than a simple CHECK.
CREATE TABLE nutrition_plan (
    plan_id INT IDENTITY(1,1),
    plan_name VARCHAR(120) NOT NULL,
    total_calories NUMERIC(10, 2) NOT NULL DEFAULT 0,
    nutritionist_code INT NOT NULL,

    CONSTRAINT pk_nutrition_plan PRIMARY KEY (plan_id),
    CONSTRAINT ck_nutrition_plan_name_not_blank CHECK (LTRIM(RTRIM(plan_name)) <> ''),
    CONSTRAINT ck_nutrition_plan_total_calories_non_negative CHECK (total_calories >= 0),
    CONSTRAINT fk_nutrition_plan_nutritionist FOREIGN KEY (nutritionist_code)
        REFERENCES nutritionist (nutritionist_code)
        ON DELETE NO ACTION
);
GO

-- Assigns plans to clients for a date range.
CREATE TABLE plan_assignment (
    assignment_id INT IDENTITY(1,1),
    start_date DATE NOT NULL,
    end_date DATE,
    assignment_status VARCHAR(20) NOT NULL,
    plan_id INT NOT NULL,
    client_id INT NOT NULL,

    CONSTRAINT pk_plan_assignment PRIMARY KEY (assignment_id),
    CONSTRAINT ck_plan_assignment_date_range CHECK (
        end_date IS NULL OR end_date >= start_date
    ),
    CONSTRAINT ck_plan_assignment_status CHECK (
        assignment_status IN ('ACTIVE', 'PAUSED', 'FINISHED', 'CANCELLED')
    ),
    CONSTRAINT fk_plan_assignment_nutrition_plan FOREIGN KEY (plan_id)
        REFERENCES nutrition_plan (plan_id)
        ON DELETE NO ACTION,
    CONSTRAINT fk_plan_assignment_client FOREIGN KEY (client_id)
        REFERENCES client (client_id)
        ON DELETE NO ACTION
);
GO

-- Defines eating moments within a nutrition plan.
CREATE TABLE meal_time (
    meal_time_id INT IDENTITY(1,1),
    meal_type VARCHAR(20) NOT NULL,
    plan_id INT NOT NULL,

    CONSTRAINT pk_meal_time PRIMARY KEY (meal_time_id),
    CONSTRAINT ck_meal_time_meal_type CHECK (
        meal_type IN ('BREAKFAST', 'LUNCH', 'DINNER', 'SNACK', 'OTHER')
    ),
    CONSTRAINT fk_meal_time_nutrition_plan FOREIGN KEY (plan_id)
        REFERENCES nutrition_plan (plan_id)
        ON DELETE CASCADE
);
GO

-- ============================================================================
-- Recipes And Products
-- ============================================================================

-- Stores recipes registered for clients.
CREATE TABLE recipe (
    recipe_id INT IDENTITY(1,1),
    nutritional_values VARCHAR(500) NOT NULL,
    client_id INT NOT NULL,

    CONSTRAINT pk_recipe PRIMARY KEY (recipe_id),
    CONSTRAINT ck_recipe_nutritional_values_not_blank CHECK (
        LTRIM(RTRIM(nutritional_values)) <> ''
    ),
    CONSTRAINT fk_recipe_client FOREIGN KEY (client_id)
        REFERENCES client (client_id)
        ON DELETE NO ACTION
);
GO

-- Stores food products and their nutritional facts.
CREATE TABLE product (
    bar_code VARCHAR(40) NOT NULL,
    portion_unit VARCHAR(30) NOT NULL,
    sodium NUMERIC(10, 2) NOT NULL DEFAULT 0,
    product_status VARCHAR(20) NOT NULL,
    iron NUMERIC(10, 2) NOT NULL DEFAULT 0,
    calcium NUMERIC(10, 2) NOT NULL DEFAULT 0,
    vitamins NUMERIC(10, 2) NOT NULL DEFAULT 0,
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
    CONSTRAINT ck_product_name_not_blank CHECK (LTRIM(RTRIM(product_name)) <> ''),
    CONSTRAINT ck_product_status CHECK (
        product_status IN ('ACTIVE', 'INACTIVE', 'PENDING_REVIEW', 'REJECTED')
    ),
    CONSTRAINT ck_product_sodium_non_negative CHECK (sodium >= 0),
    CONSTRAINT ck_product_iron_non_negative CHECK (iron >= 0),
    CONSTRAINT ck_product_calcium_non_negative CHECK (calcium >= 0),
    CONSTRAINT ck_product_vitamins_non_negative CHECK (vitamins >= 0),
    CONSTRAINT ck_product_portion_size_positive CHECK (portion_size > 0),
    CONSTRAINT ck_product_calories_non_negative CHECK (calories >= 0),
    CONSTRAINT ck_product_protein_non_negative CHECK (protein >= 0),
    CONSTRAINT ck_product_carbohydrates_non_negative CHECK (carbohydrates >= 0),
    CONSTRAINT ck_product_fat_non_negative CHECK (fat >= 0),
    CONSTRAINT fk_product_app_user FOREIGN KEY (user_id)
        REFERENCES app_user (user_id)
        ON DELETE NO ACTION
);
GO

-- Links recipes with the products used to prepare them.
CREATE TABLE recipe_product (
    recipe_id INT NOT NULL,
    product_code VARCHAR(40) NOT NULL,

    CONSTRAINT pk_recipe_product PRIMARY KEY (recipe_id, product_code),
    CONSTRAINT fk_recipe_product_recipe FOREIGN KEY (recipe_id)
        REFERENCES recipe (recipe_id)
        ON DELETE CASCADE,
    CONSTRAINT fk_recipe_product_product FOREIGN KEY (product_code)
        REFERENCES product (bar_code)
        ON DELETE NO ACTION
);
GO

-- Links meal times with the products assigned to each eating moment.
-- calories represents the calories contributed by the selected quantity of that
-- product in that meal time.
CREATE TABLE meal_time_product (
    meal_time_id INT NOT NULL,
    product_code VARCHAR(40) NOT NULL,
    calories NUMERIC(10, 2) NOT NULL,
    quantity NUMERIC(10, 2) NOT NULL,

    CONSTRAINT pk_meal_time_product PRIMARY KEY (meal_time_id, product_code),
    CONSTRAINT ck_meal_time_product_calories_non_negative CHECK (calories >= 0),
    CONSTRAINT ck_meal_time_product_quantity_positive CHECK (quantity > 0),
    CONSTRAINT fk_meal_time_product_meal_time FOREIGN KEY (meal_time_id)
        REFERENCES meal_time (meal_time_id)
        ON DELETE CASCADE,
    CONSTRAINT fk_meal_time_product_product FOREIGN KEY (product_code)
        REFERENCES product (bar_code)
        ON DELETE NO ACTION
);
GO

-- ============================================================================
-- Client Tracking
-- ============================================================================

-- Stores body measurements captured for clients over time.
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
    CONSTRAINT ck_measure_muscle_percentage_range CHECK (
        muscle_percentage BETWEEN 0 AND 100
    ),
    CONSTRAINT ck_measure_body_weight_positive CHECK (body_weight > 0),
    CONSTRAINT ck_measure_hip_positive CHECK (hip > 0),
    CONSTRAINT ck_measure_waist_positive CHECK (waist > 0),
    CONSTRAINT ck_measure_fat_percentage_range CHECK (
        fat_percentage BETWEEN 0 AND 100
    ),
    CONSTRAINT ck_measure_body_mass_index_positive CHECK (body_mass_index > 0),
    CONSTRAINT fk_measure_client FOREIGN KEY (client_id)
        REFERENCES client (client_id)
        ON DELETE NO ACTION
);
GO

-- Stores calories consumed by clients for specific meal times and dates.
CREATE TABLE daily_consume (
    consume_date DATE NOT NULL,
    total_calories NUMERIC(10, 2) NOT NULL,
    client_id INT NOT NULL,
    meal_time_id INT NOT NULL,

    CONSTRAINT pk_daily_consume PRIMARY KEY (
        client_id,
        meal_time_id,
        consume_date
    ),
    CONSTRAINT ck_daily_consume_total_calories_non_negative CHECK (
        total_calories >= 0
    ),
    CONSTRAINT fk_daily_consume_client FOREIGN KEY (client_id)
        REFERENCES client (client_id)
        ON DELETE NO ACTION,
    CONSTRAINT fk_daily_consume_meal_time FOREIGN KEY (meal_time_id)
        REFERENCES meal_time (meal_time_id)
        ON DELETE NO ACTION
);
GO
