# SARA ROSE Nigeria Limited — Project Documentation

**Application:** Company website, enquiry desk, and administration panel  
**Organisation:** SARA ROSE NIGERIA LIMITED  
**Established:** 2012  
**Location:** Km 12, Sagamu–Benin Express Way, Opposite Navy Merchant, Ogun State, Nigeria  
**Business:** Trader in heavy equipment and construction / industrial machinery  
**Named contact:** Mr. Akram Haider · +234 80 6665 1111 · contact@sararose.com

This document describes what the application does, how it is built, how to run a live demonstration, and how the main screens fit together.

---

## 1. Purpose

SARA ROSE trades in heavy equipment. The application puts that business on a professional website so a customer can:

- understand who the company is and how it works
- browse the five equipment categories the company deals in
- send an enquiry that is stored for follow-up
- register and sign in
- (for staff) manage home images, header links, registrations and enquiries from a dashboard

Brands, models, capacities and availability are **not** listed as a stock catalogue. They are confirmed when the customer enquires, which matches the commercial model in the company profile.

---

## 2. What the client can show in a demo

| Area | What the audience sees |
| --- | --- |
| Public site | Home slider, company story, vision and values, reasons to work with SARA ROSE |
| Equipment | Five groups and eleven machine types, each with a path to enquire |
| Enquire | Form stored in MySQL, directed commercially to Mr. Akram Haider |
| Registration | Public form that creates a login (username + password) |
| Login | Username and password checked against the `userMaster` table |
| Dashboard | Overview tiles, recent registrations, company facts |
| Slider admin | Add, view, reorder and delete home slider images |
| Header admin | Add, hide, reorder and delete navigation links |
| Registrations admin | List of people who registered on the public form |

**Live URLs (local demonstration):**

- Website: http://127.0.0.1:43123
- API health: http://127.0.0.1:43124/api/health
- API documentation (Swagger): http://127.0.0.1:43124/swagger

---

## 3. Public pages

| Path | Screen | Role in the demo |
| --- | --- | --- |
| `/` | Home | Opening impression: slider, logo, Sagamu presence, calls to portfolio / enquiry / login |
| `/about` | About | Trading since 2012, how the company works, sectors served |
| `/equipment` | Equipment portfolio | Five categories |
| `/equipment/{slug}` | Machine type | Description and typical use; link to enquire |
| `/why-sara-rose` | Why us | Five commercial reasons (longevity, focus, portfolio, local office, named contact) |
| `/vision-values` | Vision, mission, values | Reliability, integrity, customer focus, professionalism, quality, long-term partnerships |
| `/contact` | Enquire | Customer requirement captured in the database |
| `/register` | Registration | Username, name, email, telephone, user type, role, password |
| `/login` | Login | Username, password, number captcha |

Header navigation can be driven from the database when the site is in dynamic mode, so the client can change labels and order without a developer.

---

## 4. Administration panel

The dashboard is behind login (`/dashboard`). After a successful sign-in the user is asked whether to continue to the dashboard.

**Seeded demonstration account**

| Field | Value |
| --- | --- |
| Username | `admin` |
| Email | `admin@sararose.com` |
| Password | `SaraRose_Admin_2024` |
| Role | Admin |
| User type | Internal |

**Dashboard modules**

1. **Overview** — date greeting, role and username, counts for categories, machines, enquiries and registrations, quick links.
2. **Home slider** — upload images used on the public home page (`/dashboard/slider`, add, view).
3. **Header links** — labels, paths, sort order, visibility, enquire-style call-to-action (`/dashboard/header`).
4. **Equipment & machine type masters** — add and edit catalogue groups and machine types (`/dashboard/masters`). Changes show on the public Equipment pages.
5. **Registrations** — people who used the public Registration page (`/dashboard/registrations`).
5. **Public site / equipment / enquiries** — jump back to the customer-facing pages or stored enquiries via the API.

Passwords are never shown on the registrations list. Login checks the `userMaster` table.

---

## 5. Equipment catalogue (seeded)

Five categories and eleven machine types are seeded so the demo is never an empty catalogue.

| Category | Machine types |
| --- | --- |
| Earthmoving | Excavators, bulldozers, wheel loaders |
| Construction | Backhoe loaders, motor graders |
| Material handling | Forklifts, other machinery |
| Road & compaction | Compactors / rollers, skid-steer rollers |
| Heavy transport & lifting | Dump trucks, cranes |

Copy on each page is written as a trader’s advice, not as a manufacturer brochure.

---

## 6. How login works

1. A person registers on `/register` **or** an administrator is seeded.
2. The application stores a profile in `registrations` and a login row in `userMaster`.
3. On `/login` they enter **username** and **password** (and a simple maths captcha).
4. The API looks up `userMaster` by username (email typed in the username field is also accepted).
5. The password must match **HashPassword** (SHA-256) or **NormalPassword**.
6. There is **no** `RegistrationId` column on `userMaster`. Company and city on the session are taken from the matching `registrations` email, when present.
7. The session is kept in the browser (not in MySQL). `NormalPassword` is never returned by the login API.

Roles used in the product: Admin, Staff, User.  
User types: Internal, Customer, Dealer, Contractor.  
The public registration form cannot assign Admin.

---

## 7. Architecture

```
Browser (Angular 19, port 43123)
        |  /api  and  /uploads  proxied
        v
ASP.NET Core 8 API (port 43124)
        |
        v
MySQL 8 database  sararose
```

| Layer | Technology |
| --- | --- |
| Website | Angular 19 |
| API | ASP.NET Core 8 |
| Database | MySQL 8 |
| Files | Slider images under `backend/wwwroot/uploads/slides/` |

Company profile text (about, vision, mission, values, reasons) is supplied by the API from the client document. It is not stored as rows in MySQL.

---

## 8. Main MySQL tables

| Table | Holds |
| --- | --- |
| `categories` | Five equipment groups |
| `equipment` | Eleven machine types |
| `enquiries` | Customer requirements from `/contact` |
| `slider_slides` | Home slider files and captions |
| `header_links` | Navigation labels and paths |
| `registrations` | Public registration profiles |
| `userMaster` | Login: Username, Email, FullName, Phone, Role, UserType, HashPassword, NormalPassword, Active, CreatedAtUtc |

Full rebuild script (Workbench): `backend/sql/update-all-db.sql` (copy also at `backend/sql/update-all-db.txt`).

Default API connection:

`Server=127.0.0.1;Port=3306;Database=sararose;User=sararose;Password=SaraRose_Dev_2024;`

---

## 9. API (for technical stakeholders)

| Method | Path | Purpose |
| --- | --- | --- |
| GET | `/api/health` | Liveness |
| GET | `/api/company` | Company profile copy |
| GET | `/api/categories` | Equipment groups |
| GET | `/api/equipment?category=earthmoving` | Machine types |
| GET | `/api/equipment/{slug}` | One machine type |
| POST | `/api/enquiries` | Store an enquiry |
| GET | `/api/enquiries` | List enquiries |
| GET / POST / PUT / DELETE | `/api/slides` | Home slider |
| GET / POST / PUT / DELETE | `/api/header-links` | Header navigation |
| POST | `/api/registrations` | Register (also writes `userMaster`) |
| GET | `/api/registrations` | List registrations |
| POST | `/api/auth/login` | Username + password against `userMaster` |

---

## 10. How to start a live demonstration

1. Start MySQL on port 3306. Create database `sararose` and user `sararose` (see README or run `backend/sql/update-all-db.sql` as root in MySQL Workbench).
2. API: `cd backend` then `dotnet run --urls "http://127.0.0.1:43124"`
3. Website: `cd frontend` then `npm install` and `npm start`
4. Open http://127.0.0.1:43123
5. Follow the slide-by-slide script in **SARA-ROSE-Client-Demo.pptx** (folder `docs/`)

If tables are empty, the API seeds categories, equipment, header links and the admin login on first successful connection.

---

## 11. Suggested live-demo script (about 8–10 minutes)

1. **Home** — slider, Sagamu, “See the portfolio” / “Start an enquiry”.
2. **About** — trader since 2012, named contact, not a general merchant.
3. **Equipment** — open Earthmoving, then Excavators; point out that specification is confirmed at enquiry.
4. **Enquire** — fill a short requirement; explain it is stored for Mr. Akram Haider.
5. **Why us / Vision** — five reasons and six values.
6. **Register** (optional) — create a username, then **Login** with it.
7. **Login as admin** — username `admin`, password `SaraRose_Admin_2024`, solve the captcha, continue to dashboard.
8. **Dashboard** — tiles and recent registrations.
9. **Slider / Header** — show that the client can change the public home images and menu without editing code.

---

## 12. Related files

| File | Use |
| --- | --- |
| `README.md` | Developer setup (Windows, WSL, Linux) |
| `docs/SARA-ROSE-Project-Documentation.md` | This document |
| `docs/SARA-ROSE-Client-Demo.pptx` | PowerPoint for the client presentation |
| `backend/sql/update-all-db.sql` | Full database create / seed script |

---

© SARA ROSE Nigeria Limited — project documentation for demonstration and handover.
