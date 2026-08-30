# SARA ROSE Nigeria Limited

Company application for **SARA ROSE NIGERIA LIMITED** — a heavy-equipment trader based in Sagamu, Ogun State, established in 2012. Content follows the client company profile: about the business, five equipment categories, location, reasons to work with the company, vision, mission, values, and a named-contact enquiry flow.

## Stack

| Layer | Technology |
| --- | --- |
| Website | Angular 19 |
| API | ASP.NET Core 8 Web API |
| Database | MySQL 8 (Docker Compose on local machines) |

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

## Local setup on Windows

Install these on Windows (not only inside WSL):

1. [Docker Desktop](https://www.docker.com/products/docker-desktop/) — start it and wait until it is running
2. [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
3. [Node.js 20 LTS](https://nodejs.org/)

Confirm in **PowerShell**:

```powershell
docker version
dotnet --version
node -v
npm -v
```

### Fastest start (PowerShell)

From the repo root:

```powershell
docker compose up -d
cd backend
dotnet run --urls "http://127.0.0.1:43124"
```

Second PowerShell window:

```powershell
cd frontend
npm install
npm start
```

Or one command that opens both windows after MySQL is up:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\dev.ps1
```

Then open:

- Website: http://127.0.0.1:43123
- API health: http://127.0.0.1:43124/api/health
- Swagger: http://127.0.0.1:43124/swagger

The API creates tables and seeds the equipment catalogue on first successful MySQL connection.

### If port 3306 is already in use

Stop the other MySQL service, or run Compose on another host port:

```powershell
$env:MYSQL_PORT=3307
docker compose up -d
```

Then set the API connection string (PowerShell, same window as `dotnet run`):

```powershell
$env:ConnectionStrings__Default="Server=127.0.0.1;Port=3307;Database=sararose;User=sararose;Password=SaraRose_Dev_2024;"
```

Or edit `backend/appsettings.Development.json`.

## WSL / macOS / Linux

```bash
docker compose up -d
cd backend && dotnet run --urls "http://127.0.0.1:43124"
```

```bash
cd frontend && npm install && npm start
```

Or: `bash scripts/dev.sh`

## Database (without Docker)

```sql
CREATE DATABASE sararose CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER 'sararose'@'%' IDENTIFIED BY 'SaraRose_Dev_2024';
GRANT ALL PRIVILEGES ON sararose.* TO 'sararose'@'%';
FLUSH PRIVILEGES;
```

Default connection string:

`Server=127.0.0.1;Port=3306;Database=sararose;User=sararose;Password=SaraRose_Dev_2024;`

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

This stack is intended to run as a .NET process plus MySQL. It is not a Vercel serverless app.
