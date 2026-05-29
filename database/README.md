# Adatbázis Dokumentáció

## MySQL Séma - Elektronikai Alkatrész Raktár

### Táblák Áttekintése

#### 1. **categories** - Termékkategóriák
- `category_id` - Elsődleges kulcs
- `category_name` - Kategória neve (egyedi)
- `description` - Leírás
- Timestamp mezzők

#### 2. **suppliers** - Szállítók
- `supplier_id` - Elsődleges kulcs
- `supplier_name` - Szállító neve
- `contact_person` - Kapcsolattartó
- `email`, `phone`, `address` - Kapcsolati adatok
- `is_active` - Aktív/Inaktív flag
- Timestamp mezzők

#### 3. **products** - Termékek
- `product_id` - Elsődleges kulcs
- `product_code` - Termékkód (egyedi)
- `product_name` - Terméknév
- `description` - Leírás
- `category_id` - Idegen kulcs (categories)
- `supplier_id` - Idegen kulcs (suppliers)
- `unit_price` - Egységár
- `min_stock_level`, `max_stock_level` - Készlet szintek
- `is_active` - Aktív/Inaktív flag
- Indexek: product_code, category_id

#### 4. **warehouse_locations** - Raktári helyek
- `location_id` - Elsődleges kulcs
- `location_code` - Helyazonosító (pl. A1-01)
- `section`, `shelf` - Szekció, polc
- `row_num`, `column_num` - Sor és oszlop szám
- `capacity` - Kapacitás
- `is_active` - Aktív/Inaktív flag

#### 5. **stock** - Készletnívó
- `stock_id` - Elsödleges kulcs
- `product_id` - Idegen kulcs (products) - EGYEDI
- `location_id` - Idegen kulcs (warehouse_locations)
- `quantity` - Aktuális mennyiség (CHECK: >= 0)
- `last_counted_at` - Utolsó leltár dátuma
- Timestamp mezzők

#### 6. **stock_movements** - Készletmozgások
- `movement_id` - Elsödleges kulcs
- `product_id` - Idegen kulcs (products)
- `movement_type` - ENUM: BEVÉTELEZÉS, KIADÁS, SELEJTEZÉS, KORREKCIÓ
- `quantity` - Mozgatott mennyiség
- `reason` - Ok/megjegyzés
- `old_quantity`, `new_quantity` - Régi és új készlet
- `created_by` - Ki hajtotta végre
- Indexek: product_id, movement_type, created_at

#### 7. **units** - Kiszerelési egységek
- `unit_id` - Elsödleges kulcs
- `unit_name` - Egység neve (pl. "Darab")
- `unit_symbol` - Szimbólum (pl. "db")
- `description` - Leírás

#### 8. **product_units** - Termék-Kiszerelés kapcsolat
- `product_unit_id` - Elsödleges kulcs
- `product_id` - Idegen kulcs (products)
- `unit_id` - Idegen kulcs (units)
- `quantity_per_unit` - Mennyiség per egység
- UNIQUE: (product_id, unit_id)

## Telepítés

### 1. MySQL Server indítása
```bash
# Docker-ben
docker run --name mysql-warehouse -e MYSQL_ROOT_PASSWORD=root -p 3306:3306 -d mysql:8.0
```

### 2. Séma importálása
```bash
mysql -u root -p < schema.sql
```

### 3. Ellenőrzés
```bash
mysql -u root -p warehouse_management
SHOW TABLES;
DESC products;
```

## Kapcsolatok (Foreign Keys)

```
products ──> categories (category_id)
products ──> suppliers (supplier_id)
stock ──> products (product_id)
stock ──> warehouse_locations (location_id)
stock_movements ──> products (product_id)
product_units ──> products (product_id)
product_units ──> units (unit_id)
```

## Lekérdezési Példák

### Alacsony készletű termékek
```sql
SELECT p.product_name, s.quantity, p.min_stock_level
FROM products p
JOIN stock s ON p.product_id = s.product_id
WHERE s.quantity < p.min_stock_level
ORDER BY s.quantity ASC;
```

### Kategóriánkénti készlet
```sql
SELECT c.category_name, COUNT(p.product_id) as product_count, SUM(s.quantity) as total_quantity
FROM categories c
LEFT JOIN products p ON c.category_id = p.category_id
LEFT JOIN stock s ON p.product_id = s.product_id
GROUP BY c.category_id, c.category_name;
```

### Termék mozgástörténete
```sql
SELECT movement_id, movement_type, quantity, reason, created_at, created_by
FROM stock_movements
WHERE product_id = ?
ORDER BY created_at DESC;
```
