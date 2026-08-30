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

## Prerequisites

- Node.js 20+
- .NET 8 SDK
- MySQL 8

## Database

```sql
CREATE DATABASE sararose CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER 'sararose'@'%' IDENTIFIED BY 'SaraRose_Dev_2024';
GRANT ALL PRIVILEGES ON sararose.* TO 'sararose'@'%';
FLUSH PRIVILEGES;
```

Connection string (override with `ConnectionStrings__Default` if needed):

`Server=127.0.0.1;Port=3306;Database=sararose;User=sararose;Password=SaraRose_Dev_2024;`

The API creates tables and seeds the equipment catalogue on first run.

## Run locally

Terminal 1 — API (http://127.0.0.1:43124):

```bash
cd backend
dotnet run --urls "http://127.0.0.1:43124"
```

Terminal 2 — Angular (http://127.0.0.1:43123), proxied to the API:

```bash
cd frontend
npm install
npm start
```

Open http://127.0.0.1:43123. Swagger: http://127.0.0.1:43124/swagger.

Or from the repo root: `bash scripts/dev.sh` (expects MySQL already running).

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
