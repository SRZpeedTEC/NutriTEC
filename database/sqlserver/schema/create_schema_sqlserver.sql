/*
    NutriTEC Database Project
    Script: create_schema_sqlserver.sql
    Purpose: Create the SQL Server schema, constraints, and base validation.

    Notes:
      - Run this script after create_database_sqlserver.sql.
      - Tables are created in dependency order so foreign keys are valid.
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
    age INT NOT NULL,
    email VARCHAR(255) NOT NULL,

    CONSTRAINT pk_user PRIMARY KEY (user_id),
    CONSTRAINT uq_user_email UNIQUE (email),
    CONSTRAINT ck_user_name_not_blank CHECK (LTRIM(RTRIM(name)) <> ''),
    CONSTRAINT ck_user_last_name_not_blank CHECK (LTRIM(RTRIM(last_name)) <> ''),
    CONSTRAINT ck_user_hash_password_not_blank CHECK (LTRIM(RTRIM(hash_password)) <> ''),
    CONSTRAINT ck_user_age_non_negative CHECK (age >= 0),
    CONSTRAINT ck_user_birthday_valid CHECK (birthday >= '1900-01-01'),
    CONSTRAINT ck_user_email_not_blank CHECK (LTRIM(RTRIM(email)) <> ''),
    CONSTRAINT ck_user_email_format CHECK (email LIKE '%_@_%._%' AND email NOT LIKE '% %')
);
GO

CREATE TABLE admin (
    admin_id INT IDENTITY(1,1) NOT NULL,
    user_id INT NOT NULL,

    CONSTRAINT pk_admin PRIMARY KEY (admin_id),
    CONSTRAINT uq_admin_user_id UNIQUE (user_id),
    CONSTRAINT fk_admin_user FOREIGN KEY (user_id)
        REFERENCES app_user (user_id)
);
GO

CREATE TABLE nutritionist (
    nutritionist_code INT IDENTITY(1,1) NOT NULL,
    payment_method VARCHAR(20) NOT NULL,
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
    CONSTRAINT ck_nutritionist_photo_not_blank CHECK (
        photo IS NULL OR LTRIM(RTRIM(photo)) <> ''
    ),
    CONSTRAINT ck_nutritionist_address_not_blank CHECK (LTRIM(RTRIM(address)) <> ''),
    CONSTRAINT ck_nutritionist_id_number_not_blank CHECK (LTRIM(RTRIM(id_number)) <> ''),
    CONSTRAINT ck_nutritionist_encrypted_credit_card_not_blank CHECK (
        encrypted_credit_card IS NULL OR LTRIM(RTRIM(encrypted_credit_card)) <> ''
    ),
    CONSTRAINT ck_nutritionist_weight_non_negative CHECK (weight >= 0),
    CONSTRAINT ck_nutritionist_body_mass_index_non_negative CHECK (body_mass_index >= 0),
    CONSTRAINT fk_nutritionist_user FOREIGN KEY (user_id)
        REFERENCES app_user (user_id)
);
GO

CREATE TABLE client (
    client_id INT IDENTITY(1,1) NOT NULL,
    max_daily_calories NUMERIC(10, 2) NOT NULL,
    country VARCHAR(80) NOT NULL,
    user_id INT NOT NULL,

    CONSTRAINT pk_client PRIMARY KEY (client_id),
    CONSTRAINT uq_client_user_id UNIQUE (user_id),
    CONSTRAINT ck_client_max_daily_calories_non_negative CHECK (max_daily_calories >= 0),
    CONSTRAINT ck_client_country_not_blank CHECK (LTRIM(RTRIM(country)) <> ''),
    CONSTRAINT fk_client_user FOREIGN KEY (user_id)
        REFERENCES app_user (user_id)
);
GO

CREATE TABLE nutritionist_client (
    start_date DATE NOT NULL,
    end_date DATE NULL,
    status VARCHAR(20) NOT NULL,
    nutritionist_code INT NOT NULL,
    client_id INT NOT NULL,

    CONSTRAINT pk_nutritionist_client PRIMARY KEY (start_date),
    CONSTRAINT ck_nutritionist_client_date_range CHECK (
        end_date IS NULL OR end_date >= start_date
    ),
    CONSTRAINT ck_nutritionist_client_status CHECK (
        status IN ('ACTIVE', 'PAUSED', 'FINISHED', 'CANCELLED')
    ),
    CONSTRAINT fk_nutritionist_client_nutritionist FOREIGN KEY (nutritionist_code)
        REFERENCES nutritionist (nutritionist_code),
    CONSTRAINT fk_nutritionist_client_client FOREIGN KEY (client_id)
        REFERENCES client (client_id)
);
GO

-- ============================================================================
-- Plans And Meals
-- ============================================================================

CREATE TABLE user_plan (
    plan_id INT IDENTITY(1,1) NOT NULL,
    plan_name VARCHAR(120) NOT NULL,
    total_calories NUMERIC(10, 2) NOT NULL DEFAULT 0,
    nutritionist_code INT NOT NULL,

    CONSTRAINT pk_plan PRIMARY KEY (plan_id),
    CONSTRAINT ck_plan_name_not_blank CHECK (LTRIM(RTRIM(plan_name)) <> ''),
    CONSTRAINT ck_plan_total_calories_non_negative CHECK (total_calories >= 0),
    CONSTRAINT fk_plan_nutritionist FOREIGN KEY (nutritionist_code)
        REFERENCES nutritionist (nutritionist_code)
);
GO

CREATE TABLE plan_assignment (
    assignment_id INT IDENTITY(1,1) NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NULL,
    status VARCHAR(20) NOT NULL,
    plan_id INT NOT NULL,
    client_id INT NOT NULL,

    CONSTRAINT pk_plan_assignment PRIMARY KEY (assignment_id),
    CONSTRAINT ck_plan_assignment_date_range CHECK (
        end_date IS NULL OR end_date >= start_date
    ),
    CONSTRAINT ck_plan_assignment_status CHECK (
        status IN ('ACTIVE', 'PAUSED', 'FINISHED', 'CANCELLED')
    ),
    CONSTRAINT fk_plan_assignment_plan FOREIGN KEY (plan_id)
        REFERENCES user_plan (plan_id),
    CONSTRAINT fk_plan_assignment_client FOREIGN KEY (client_id)
        REFERENCES client (client_id)
);
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
    CONSTRAINT uq_plan_meal_time_plan_meal UNIQUE (plan_id, meal_time_id),
    CONSTRAINT fk_plan_meal_time_meal_time FOREIGN KEY (meal_time_id)
        REFERENCES meal_time (meal_time_id),
    CONSTRAINT fk_plan_meal_time_plan FOREIGN KEY (plan_id)
        REFERENCES user_plan (plan_id)
);
GO

-- ============================================================================
-- Recipes And Products
-- ============================================================================

CREATE TABLE recipe (
    recipe_id INT IDENTITY(1,1) NOT NULL,
    client_id INT NOT NULL,

    CONSTRAINT pk_recipe PRIMARY KEY (recipe_id),
    CONSTRAINT fk_recipe_client FOREIGN KEY (client_id)
        REFERENCES client (client_id)
);
GO

CREATE TABLE product (
    bar_code VARCHAR(40) NOT NULL,
    portion_unit VARCHAR(30) NOT NULL,
    sodium NUMERIC(10, 2) NOT NULL DEFAULT 0,
    status VARCHAR(20) NOT NULL,
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
    CONSTRAINT ck_product_status CHECK (
        status IN ('ACTIVE', 'INACTIVE', 'PENDING_REVIEW', 'REJECTED')
    ),
    CONSTRAINT ck_product_iron_non_negative CHECK (iron >= 0),
    CONSTRAINT ck_product_calcium_non_negative CHECK (calcium >= 0),
    CONSTRAINT ck_product_vitamins_non_negative CHECK (vitamins >= 0),
    CONSTRAINT ck_product_portion_size_non_negative CHECK (portion_size >= 0),
    CONSTRAINT ck_product_calories_non_negative CHECK (calories >= 0),
    CONSTRAINT ck_product_protein_non_negative CHECK (protein >= 0),
    CONSTRAINT ck_product_carbohydrates_non_negative CHECK (carbohydrates >= 0),
    CONSTRAINT ck_product_fat_non_negative CHECK (fat >= 0),
    CONSTRAINT ck_product_sodium_non_negative CHECK (sodium >= 0),
    CONSTRAINT ck_product_name_not_blank CHECK (LTRIM(RTRIM(product_name)) <> ''),
    CONSTRAINT fk_product_user FOREIGN KEY (user_id)
        REFERENCES app_user (user_id)
);
GO

CREATE TABLE recipe_product (
    recipe_id INT NOT NULL,
    product_code VARCHAR(40) NOT NULL,

    CONSTRAINT pk_recipe_product PRIMARY KEY (recipe_id, product_code),
    CONSTRAINT fk_recipe_product_recipe FOREIGN KEY (recipe_id)
        REFERENCES recipe (recipe_id),
    CONSTRAINT fk_recipe_product_product FOREIGN KEY (product_code)
        REFERENCES product (bar_code)
);
GO

CREATE TABLE meal_time_product (
    meal_time_id INT NOT NULL,
    product_code VARCHAR(40) NOT NULL,
    calories NUMERIC(10, 2) NOT NULL,
    quantity NUMERIC(10, 2) NOT NULL,

    CONSTRAINT pk_meal_time_product PRIMARY KEY (meal_time_id, product_code),
    CONSTRAINT ck_meal_time_product_calories_non_negative CHECK (calories >= 0),
    CONSTRAINT ck_meal_time_product_quantity_non_negative CHECK (quantity >= 0),
    CONSTRAINT fk_meal_time_product_meal_time FOREIGN KEY (meal_time_id)
        REFERENCES meal_time (meal_time_id),
    CONSTRAINT fk_meal_time_product_product FOREIGN KEY (product_code)
        REFERENCES product (bar_code)
);
GO

-- ============================================================================
-- Client Tracking
-- ============================================================================

CREATE TABLE measure (
    measure_date DATETIME2 NOT NULL,
    neck NUMERIC(6, 2) NOT NULL,
    muscle NUMERIC(5, 2) NOT NULL,
    weight NUMERIC(6, 2) NOT NULL,
    hip NUMERIC(6, 2) NOT NULL,
    waist NUMERIC(6, 2) NOT NULL,
    fat NUMERIC(5, 2) NOT NULL,
    body_mass_index NUMERIC(5, 2) NOT NULL,
    client_id INT NOT NULL,

    CONSTRAINT pk_measure PRIMARY KEY (measure_date),
    CONSTRAINT ck_measure_neck_non_negative CHECK (neck >= 0),
    CONSTRAINT ck_measure_muscle_percentage_range CHECK (muscle BETWEEN 0 AND 100),
    CONSTRAINT ck_measure_weight_non_negative CHECK (weight >= 0),
    CONSTRAINT ck_measure_hip_non_negative CHECK (hip >= 0),
    CONSTRAINT ck_measure_waist_non_negative CHECK (waist >= 0),
    CONSTRAINT ck_measure_fat_percentage_range CHECK (fat BETWEEN 0 AND 100),
    CONSTRAINT ck_measure_body_mass_index_non_negative CHECK (body_mass_index >= 0),
    CONSTRAINT fk_measure_client FOREIGN KEY (client_id)
        REFERENCES client (client_id)
);
GO

CREATE TABLE daily_consume (
    consume_date DATE NOT NULL,
    total_calories NUMERIC(10, 2) NOT NULL,
    client_id INT NOT NULL,

    CONSTRAINT pk_daily_consume PRIMARY KEY (consume_date),
    CONSTRAINT ck_daily_consume_total_calories_non_negative CHECK (total_calories >= 0),
    CONSTRAINT fk_daily_consume_client FOREIGN KEY (client_id)
        REFERENCES client (client_id)
);
GO

CREATE TABLE daily_meal_time (
    plan_meal_time_id INT NOT NULL,
    client_id INT NOT NULL,
    consume_date DATE NOT NULL,
    meal_time_id INT NOT NULL,

    CONSTRAINT pk_daily_meal_time PRIMARY KEY (plan_meal_time_id),
    CONSTRAINT fk_daily_meal_time_plan_meal_time FOREIGN KEY (plan_meal_time_id)
        REFERENCES plan_meal_time (plan_meal_time_id),
    CONSTRAINT fk_daily_meal_time_client FOREIGN KEY (client_id)
        REFERENCES client (client_id),
    CONSTRAINT fk_daily_meal_time_daily_consume FOREIGN KEY (consume_date)
        REFERENCES daily_consume (consume_date),
    CONSTRAINT fk_daily_meal_time_meal_time FOREIGN KEY (meal_time_id)
        REFERENCES meal_time (meal_time_id)
);
GO
