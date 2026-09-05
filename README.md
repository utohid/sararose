# SARA ROSE Nigeria Limited

Company application for **SARA ROSE NIGERIA LIMITED** — a heavy-equipment trader based in Sagamu, Ogun State, established in 2012. Content follows the client company profile: about the business, five equipment categories, location, reasons to work with the company, vision, mission, values, and a named-contact enquiry flow.

## Stack

| Layer | Technology |
| --- | --- |
| Website | Angular 19 |
| API | ASP.NET Core 8 Web API |
| Database | MySQL 8 |

## What you can do

- Read the company story, vision, values and reasons to work with SARA ROSE
- Browse the five equipment groups (earthmoving, construction, material handling, road & compaction, transport & lifting)
- Open a machine type and send an enquiry stored in MySQL
- Inspect the same data from Swagger at `/swagger`

Enquiries are handled commercially by **Mr. Akram Haider** (`+234 80 6665 1111`, `contact@sararose.com`). Brands, models and availability are confirmed at enquiry, as in the profile.

## Get the code (Windows)

Origin CLI is not available in PowerShell. Use **WSL**, then open the cloned folder in Windows (Explorer, Visual Studio, or VS Code).

```bash
# Run in WSL (Origin CLI is not available in PowerShell)
# Install the Origin CLI
curl -fsSL https://downloads.cursor.com/origin/install.sh | sh

# Sign in (also sets up git credentials)
origin auth login

# Clone the repository
origin repo clone dilwala-noor/genesis
```

If `origin` is not found after install:

```bash
echo 'export PATH="$HOME/.local/bin:$PATH"' >> ~/.bashrc
source ~/.bashrc
```

Browse the private repo: https://cursor.com/codebase/dilwala-noor/genesis  
Origin CLI docs: https://cursor.com/docs/origin/cli

After clone, the project folder is typically something like `\\wsl$\Ubuntu\home\<you>\genesis`. You can run the app from **PowerShell** against that folder, or stay in WSL.

If you already have a clone:

```bash
git pull origin main
```

## Local setup on Windows

Install these on Windows:

1. [MySQL 8](https://dev.mysql.com/downloads/installer/) — during setup, note the root password and keep port **3306**
2. [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
3. [Node.js 20 LTS](https://nodejs.org/)

Confirm in **PowerShell**:

```powershell
dotnet --version
node -v
npm -v
mysql --version
```

### Create the database

The full script (create database, user, tables, and catalogue seed) is in
`backend/sql/update-all-db.txt`. Run it as MySQL root, or paste it into MySQL Workbench.

In MySQL Workbench or a MySQL command prompt (as root):

```sql
CREATE DATABASE IF NOT EXISTS sararose CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER IF NOT EXISTS 'sararose'@'localhost' IDENTIFIED BY 'SaraRose_Dev_2024';
CREATE USER IF NOT EXISTS 'sararose'@'127.0.0.1' IDENTIFIED BY 'SaraRose_Dev_2024';
GRANT ALL PRIVILEGES ON sararose.* TO 'sararose'@'localhost';
GRANT ALL PRIVILEGES ON sararose.* TO 'sararose'@'127.0.0.1';
FLUSH PRIVILEGES;
```

Default connection string (already in `backend/appsettings.json` and `backend/appsettings.Development.json`):

`Server=127.0.0.1;Port=3306;Database=sararose;User=sararose;Password=SaraRose_Dev_2024;`

If your MySQL uses another port or password, edit those files or set this in PowerShell before `dotnet run`:

```powershell
$env:ConnectionStrings__Default="Server=127.0.0.1;Port=3306;Database=sararose;User=sararose;Password=YOUR_PASSWORD;"
```

### Run the API and website

Start the **MySQL** Windows service if it is not running (Services app, or `net start MySQL80`).

Window 1 — API:

```powershell
cd backend
dotnet run --urls "http://127.0.0.1:43124"
```

Window 2 — Angular:

```powershell
cd frontend
npm install
npm start
```

Or from the repo root:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\dev.ps1
```

Then open:

- Website: http://127.0.0.1:43123
- API health: http://127.0.0.1:43124/api/health
- Swagger: http://127.0.0.1:43124/swagger

The API creates tables and seeds the equipment catalogue on first successful MySQL connection. Login is validated against the `userMaster` table (`Username` + `HashPassword` / `NormalPassword`). Seeded admin:

- Username: `admin`
- Email: `admin@sararose.com`
- Password: `SaraRose_Admin_2024`
- Role: `Admin`
- User type: `Internal`

Public registration stores a row in `registrations` and a matching login row in `userMaster`.

## WSL / macOS / Linux

Create the same database and user, then:

```bash
cd backend && dotnet run --urls "http://127.0.0.1:43124"
```

```bash
cd frontend && npm install && npm start
```

Or: `bash scripts/dev.sh`

## API

| Method | Path | Purpose |
| --- | --- | --- |
| GET | `/api/health` | Liveness |
| GET | `/api/company` | Profile copy from the client document |
| GET | `/api/categories` | Five equipment groups |
| GET | `/api/equipment?category=earthmoving` | Machine types |
| GET | `/api/equipment/{slug}` | One machine type |
| POST | `/api/enquiries` | Store a customer enquiry |
| GET | `/api/enquiries` | List stored enquiries |
| POST | `/api/registrations` | Create a user and a `userMaster` login row |
| GET | `/api/registrations` | List registered users |
| POST | `/api/auth/login` | Validate username and password from `userMaster` |

This stack is intended to run as a .NET process plus MySQL. It is not a Vercel serverless app.
