# Raktárkezelő Asztali Alkalmazás + WordPress CMS

## Projekt Leírása
Ez egy komplex raktárkezelő rendszer elektronikai alkatrészek kezelésére. Tartalmaz:
- **WPF Desktop App (.NET 8)** - Grafikus asztali alkalmazás
- **MySQL Database** - Adatbázis sémák
- **WordPress CMS** - Webes felület Docker-ben

## Projekt Struktúra

```
warehouse-management/
├── database/              # MySQL sémák és scripts
├── desktop-app/           # WPF alkalmazás (.NET 8)
│   ├── WarehouseApp/      # Fő alkalmazás
│   └── WarehouseApp.Tests/# Unit tesztek
├── cms-website/           # WordPress Docker setup
└── docs/                  # Dokumentáció
```

## Gyorskezdés

### 1. Adatbázis Beállítása
```bash
mysql -u root -p < database/schema.sql
```

### 2. WPF App Futtatása
```bash
cd desktop-app/WarehouseApp
dotnet restore
dotnet run
```

### 3. WordPress CMS Docker-ben
```bash
cd cms-website
docker-compose up -d
# Megnyitás: http://localhost:8080
```

## Technológiák
- **Frontend:** WPF (.NET 8)
- **Backend:** C# Business Logic
- **Database:** MySQL 8.0+
- **CMS:** WordPress 6.x
- **Containerization:** Docker & Docker Compose

## Funkciók
✅ Termékkezelés (CRUD)
✅ Készletkezelés (Bevételezés, Kiadás, Selejtezés, Korrekció)
✅ Lekérdezések és Listázások
✅ Raktári helyek kezelése
✅ Szállító kezelés
✅ Unit tesztek (8+)
✅ Rétegzett architektúra
✅ Clean Code szabályok

## Unit Tesztek
```bash
cd desktop-app/WarehouseApp.Tests
dotnet test
```

## Kapcsolat
Fejlesztő: Zalanium
Email: zalanium@github.com
