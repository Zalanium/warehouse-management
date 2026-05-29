-- MySQL Warehouse Management System Database Schema
-- Elektronikai Alkatrész Raktár

-- Create Database
CREATE DATABASE IF NOT EXISTS warehouse_management;
USE warehouse_management;

-- 1. CATEGORIES TABLE
CREATE TABLE categories (
    category_id INT PRIMARY KEY AUTO_INCREMENT,
    category_name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- 2. SUPPLIERS TABLE
CREATE TABLE suppliers (
    supplier_id INT PRIMARY KEY AUTO_INCREMENT,
    supplier_name VARCHAR(100) NOT NULL,
    contact_person VARCHAR(100),
    email VARCHAR(100),
    phone VARCHAR(20),
    address VARCHAR(255),
    city VARCHAR(50),
    postal_code VARCHAR(10),
    country VARCHAR(50),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- 3. PRODUCTS TABLE
CREATE TABLE products (
    product_id INT PRIMARY KEY AUTO_INCREMENT,
    product_code VARCHAR(50) NOT NULL UNIQUE,
    product_name VARCHAR(150) NOT NULL,
    description TEXT,
    category_id INT NOT NULL,
    supplier_id INT NOT NULL,
    unit_price DECIMAL(10, 2) NOT NULL,
    min_stock_level INT DEFAULT 10,
    max_stock_level INT DEFAULT 1000,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (category_id) REFERENCES categories(category_id),
    FOREIGN KEY (supplier_id) REFERENCES suppliers(supplier_id),
    INDEX idx_product_code (product_code),
    INDEX idx_category_id (category_id)
);

-- 4. WAREHOUSE LOCATIONS TABLE
CREATE TABLE warehouse_locations (
    location_id INT PRIMARY KEY AUTO_INCREMENT,
    location_code VARCHAR(50) NOT NULL UNIQUE,
    section VARCHAR(50),
    shelf VARCHAR(50),
    row_num INT,
    column_num INT,
    capacity INT,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 5. STOCK TABLE
CREATE TABLE stock (
    stock_id INT PRIMARY KEY AUTO_INCREMENT,
    product_id INT NOT NULL UNIQUE,
    location_id INT NOT NULL,
    quantity INT DEFAULT 0,
    last_counted_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (product_id) REFERENCES products(product_id),
    FOREIGN KEY (location_id) REFERENCES warehouse_locations(location_id),
    CHECK (quantity >= 0)
);

-- 6. STOCK_MOVEMENTS TABLE
CREATE TABLE stock_movements (
    movement_id INT PRIMARY KEY AUTO_INCREMENT,
    product_id INT NOT NULL,
    movement_type ENUM('BEVÉTELEZÉS', 'KIADÁS', 'SELEJTEZÉS', 'KORREKCIÓ') NOT NULL,
    quantity INT NOT NULL,
    reason VARCHAR(255),
    old_quantity INT,
    new_quantity INT,
    created_by VARCHAR(100),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (product_id) REFERENCES products(product_id),
    INDEX idx_product_id (product_id),
    INDEX idx_movement_type (movement_type),
    INDEX idx_created_at (created_at)
);

-- 7. UNITS TABLE
CREATE TABLE units (
    unit_id INT PRIMARY KEY AUTO_INCREMENT,
    unit_name VARCHAR(50) NOT NULL UNIQUE,
    unit_symbol VARCHAR(10) NOT NULL,
    description VARCHAR(255),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 8. PRODUCT_UNITS TABLE
CREATE TABLE product_units (
    product_unit_id INT PRIMARY KEY AUTO_INCREMENT,
    product_id INT NOT NULL,
    unit_id INT NOT NULL,
    quantity_per_unit DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (product_id) REFERENCES products(product_id) ON DELETE CASCADE,
    FOREIGN KEY (unit_id) REFERENCES units(unit_id),
    UNIQUE KEY unique_product_unit (product_id, unit_id)
);

-- INSERT Sample Data
INSERT INTO categories (category_name, description) VALUES
('Processzor', 'CPU processzor chip-ek'),
('RAM Memória', 'Random Access Memory modulok'),
('Merevlemez', 'HDD és SSD tárolók'),
('Tápegység', 'Számítógép tápegységek'),
('Hűtés', 'Processzor és videokártya hűtők'),
('Kábel', 'Egyéb elektronikai kábelek');

INSERT INTO suppliers (supplier_name, contact_person, email, phone, city, country, is_active) VALUES
('TechCorp Hungary', 'Kiss János', 'john@techcorp.hu', '+36-1-123-4567', 'Budapest', 'Hungary', TRUE),
('GlobalElectronics Ltd', 'Smith Michael', 'mike@globalelec.com', '+44-20-1234-5678', 'London', 'UK', TRUE),
('AsiaComponents Inc', 'Wang Chen', 'chen@asiacomp.cn', '+86-10-9876-5432', 'Beijing', 'China', TRUE);

INSERT INTO units (unit_name, unit_symbol, description) VALUES
('Darab', 'db', 'Egyedi darab'),
('Doboz', 'dob', 'Egy doboz'),
('Csomag', 'csomag', 'Egy csomag');

INSERT INTO warehouse_locations (location_code, section, shelf, row_num, column_num, capacity, is_active) VALUES
('A1-01', 'A', '1', 1, 1, 50, TRUE),
('A1-02', 'A', '1', 1, 2, 50, TRUE),
('A2-01', 'A', '2', 2, 1, 50, TRUE),
('B1-01', 'B', '1', 1, 1, 100, TRUE),
('B2-01', 'B', '2', 2, 1, 100, TRUE);

INSERT INTO products (product_code, product_name, description, category_id, supplier_id, unit_price, min_stock_level, max_stock_level, is_active) VALUES
('PROC-001', 'Intel Core i7-13700K', 'High-end desktop processor', 1, 1, 450.00, 5, 50, TRUE),
('PROC-002', 'AMD Ryzen 9 7950X', 'High-end desktop processor', 1, 2, 550.00, 5, 50, TRUE),
('RAM-001', 'Kingston DDR5 32GB', 'DDR5 Memory Module', 2, 1, 120.00, 10, 100, TRUE),
('RAM-002', 'Corsair DDR5 64GB', 'DDR5 Memory Module', 2, 3, 200.00, 5, 50, TRUE),
('SSD-001', 'Samsung 870 QVO 1TB', 'SATA SSD Storage', 3, 1, 85.00, 15, 150, TRUE),
('SSD-002', 'WD Black SN850X 2TB', 'NVMe SSD Storage', 3, 2, 180.00, 10, 100, TRUE),
('PSU-001', 'Corsair RM1000x 1000W', 'Modular Power Supply', 4, 1, 180.00, 5, 50, TRUE),
('COOL-001', 'Noctua NH-D15', 'CPU Cooler', 5, 3, 95.00, 8, 80, TRUE);

INSERT INTO stock (product_id, location_id, quantity) VALUES
(1, 1, 25),
(2, 1, 15),
(3, 2, 45),
(4, 2, 30),
(5, 3, 60),
(6, 4, 35),
(7, 4, 20),
(8, 5, 40);

INSERT INTO stock_movements (product_id, movement_type, quantity, reason, old_quantity, new_quantity, created_by) VALUES
(1, 'BEVÉTELEZÉS', 25, 'Szállítás', 0, 25, 'admin'),
(3, 'BEVÉTELEZÉS', 45, 'Szállítás', 0, 45, 'admin'),
(5, 'KIADÁS', 5, 'Ügyfélnek eladva', 60, 55, 'sales_user');

INSERT INTO product_units (product_id, unit_id, quantity_per_unit) VALUES
(1, 1, 1),
(3, 1, 1),
(5, 1, 1);
