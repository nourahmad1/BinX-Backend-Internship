<div align="center">

# Day 3 — Implementing Core Routes I: Catalog & Read Operations

*Field notes from the day `GET /api/Patients` grew up from "return everything" into a real, scalable list endpoint.*

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity%20Framework-Core-512BD4?logo=dotnet&logoColor=white)
![LINQ](https://img.shields.io/badge/LINQ-Query%20Projection-5E5E5E?logo=dotnet&logoColor=white)
![Swagger](https://img.shields.io/badge/Verified%20with-Swagger-85EA2D?logo=swagger&logoColor=black)
![Status](https://img.shields.io/badge/status-complete-2ea44c)

`⏱ 8 hours` · `📄 Pagination` · `🔍 Filtering` · `🔃 Sorting` · `📦 DTO Projection`

</div>

---

## 📌 Today in one sentence

Rebuilding `GET /api/Patients` from a basic "return every row" endpoint into a realistic, scalable one — with pagination, optional filtering, sorting, and a dedicated response DTO, all pushed down into the database query instead of handled in memory.

## 📌 Learning objectives

- Implement paginated list endpoints rather than returning entire tables at once
- Support filtering and sorting through query parameters
- Project entities to DTOs rather than exposing them directly

## 📌 Key topics

- Paginated list endpoints
- Filtering and sorting via query parameters
- Projecting to DTOs
- Avoiding over-fetching

## 📌 What I learned

### 1. Pagination stops "works in dev" from becoming "breaks in prod"

Returning an entire table in one response works fine with 10 rows in development and breaks down completely with 100,000 rows in production. A paginated endpoint accepts a page number and page size as query parameters, applies `Skip` and `Take` in the LINQ query, and returns both the requested page and the total count so a client can build pagination controls.

`GET /api/Patients` now accepts `page` and `pageSize`, letting the client request a specific slice of patient records instead of loading everything at once:

```csharp
var patients = await _context.Patients
    .OrderBy(p => p.LastName)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

var totalCount = await _context.Patients.CountAsync();
```

> **Note to self:** an unpaginated "list everything" endpoint is one of the most common causes of a slow, unresponsive API once real data volume arrives. Build pagination in from the very first list endpoint, not as a later optimization.

### 2. Optional filters and sort options, applied conditionally

Optional query parameters let a client narrow and order results without the API needing a separate endpoint for every possible combination. Each filter is applied conditionally — only adding a `Where` clause when that parameter was actually supplied — so the same endpoint serves both an unfiltered browse and a specific filtered search.

Two filters were added to `GET /api/Patients`:

- **`search`** — matches patients by first or last name
- **`gender`** — filters patients by gender

Plus a **`sort`** query parameter supporting multiple options: first name, last name, or creation date.

```csharp
if (!string.IsNullOrWhiteSpace(search))
{
    query = query.Where(p =>
        p.FirstName.Contains(search) || p.LastName.Contains(search));
}

if (!string.IsNullOrWhiteSpace(gender))
{
    query = query.Where(p => p.Gender == gender);
}

query = sort switch
{
    "firstName" => query.OrderBy(p => p.FirstName),
    "lastName" => query.OrderBy(p => p.LastName),
    "createdAt" => query.OrderBy(p => p.CreatedAt),
    _ => query.OrderBy(p => p.LastName)
};
```

The query is built using Entity Framework Core and LINQ, so filtering, sorting, and pagination are all applied to the database query **before** the data is retrieved — not filtered in memory after the fact — which is what makes the endpoint efficient.

### 3. Projecting to a DTO instead of exposing the entity

Returning an EF Core entity directly from an endpoint risks leaking internal fields never meant for a client, and can trigger unwanted lazy-loading of related data. Projecting to a dedicated response DTO with `.Select(...)` — explicitly listing exactly which fields the client should see — keeps the API's public contract intentional and decoupled from the database schema's internal shape.

`PatientResponseDto` now stands between `Patient` and the client:

```csharp
var result = await query
    .Select(p => new PatientResponseDto
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Gender = p.Gender,
        PhoneNumber = p.PhoneNumber,
        CreatedAt = p.CreatedAt
    })
    .ToListAsync();
```

### 4. Avoiding over-fetching

A query that pulls every column and every related entity eagerly, even when the endpoint only needs a patient's name and contact info, wastes database and network resources at scale. Selecting only the fields actually needed — which projecting to a DTO naturally encourages — keeps the endpoint's actual database cost proportional to what it really needs to return.

Two more habits reinforced this today:

- **`AsNoTracking()`** — since this endpoint is read-only, no EF Core change-tracking overhead is needed
- **`CountAsync()`** — the total matching count is calculated *before* pagination is applied, so the API can return useful metadata (current page, page size, total records, total pages) alongside the actual data

```csharp
var query = _context.Patients.AsNoTracking().AsQueryable();

// ...filters and sort applied above...

var totalCount = await query.CountAsync();

var items = await query
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .Select(p => new PatientResponseDto { /* ... */ })
    .ToListAsync();
```

## 📌 What I built — hands-on lab

- [x] Implemented a paginated `GET /api/Patients` endpoint accepting `page` and `pageSize`
- [x] Added a `search` filter matching first or last name
- [x] Added a `gender` filter
- [x] Added a `sort` query parameter supporting first name, last name, and creation date
- [x] Created `PatientResponseDto` and projected the query to it with `.Select(...)`
- [x] Used `AsNoTracking()` since the endpoint is read-only
- [x] Used `CountAsync()` to calculate total matching records before pagination
- [x] Returned pagination metadata alongside the patient data (current page, page size, total records, total pages)
- [x] Verified the endpoint in Swagger with pagination, filtering, sorting, and combinations of all three

**Tools:** ASP.NET Core · Entity Framework Core · Swagger

## 📌 Request shape

```text
GET /api/Patients?page=2&pageSize=10&search=ahmad&gender=Male&sort=lastName
```

```text
Query pipeline
──────────────
Patients (AsNoTracking)
    │
    ├── Where: search  (FirstName / LastName contains)
    ├── Where: gender
    ├── OrderBy: sort  (firstName | lastName | createdAt)
    │
    ├── CountAsync()  → totalCount
    │
    ├── Skip((page - 1) * pageSize)
    ├── Take(pageSize)
    └── Select → PatientResponseDto
```

## 📌 Response shape

```json
{
  "page": 2,
  "pageSize": 10,
  "totalCount": 47,
  "totalPages": 5,
  "items": [
    {
      "id": 12,
      "firstName": "Ahmad",
      "lastName": "Hassan",
      "gender": "Male",
      "phoneNumber": "0599123456",
      "createdAt": "2026-08-20T09:15:00Z"
    }
  ]
}
```

## 📌 Why this is different from Day 2

```text
Day 2
Model the domain and get the schema live
        ↓
Entity Classes + Fluent API + Migrations
        ↓
Database matches the ERD

Day 3
Make reading that data actually scale
        ↓
Pagination + Filtering + Sorting + DTO Projection
        ↓
GET /api/Patients returns exactly what's needed, nothing more
```

Day 2 was about proving the data model and database were correct. Day 3 is about proving the API built on top of that model won't fall over once it's handling real volume — the same query pipeline (filter → sort → count → paginate → project) is the pattern every other list endpoint in the project will follow.

## 📌 Verification status

| Check | Method | Result |
|---|---|---|
| Pagination (`page`, `pageSize`) | Swagger | ✅ Verified |
| Filter — `search` | Swagger | ✅ Verified |
| Filter — `gender` | Swagger | ✅ Verified |
| Sort — first name / last name / created date | Swagger | ✅ Verified |
| Combined pagination + filtering + sorting | Swagger | ✅ Verified |

Postman testing was deliberately postponed to Day 5, where the endpoint will be run through the full set of parameter combinations and the results documented.

---

<div align="center">

**Day 3 — Complete ✅**

`Pagination` · `Filtering` · `Sorting` · `DTO Projection` · `AsNoTracking` · `Swagger Verified`

*— end of Day 3*

</div>
