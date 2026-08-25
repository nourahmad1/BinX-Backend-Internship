<div align="center">

# Day 2 — Building the EF Core Data Model & Migrations

*Field notes from the day the draw.io blueprint finally became real tables in a real database.*

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity%20Framework-Core-512BD4?logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-CC2927?logo=microsoftsqlserver&logoColor=white)
![Identity](https://img.shields.io/badge/ASP.NET%20Core-Identity-512BD4?logo=dotnet&logoColor=white)
![Status](https://img.shields.io/badge/status-complete-2ea44c)

`⏱ 8 hours` · `🧬 5 Entities Modeled` · `📦 4 Migrations Verified`

</div>

---

## 📌 Today in one sentence

Turning Day 1's ERD from a diagram into a living EF Core model — entity classes, Fluent API relationships, four reviewed migrations, and a SQL Server database confirmed to match the blueprint, table by table.

## 📌 Learning objectives

- Model the full capstone domain as EF Core entity classes
- Configure relationships explicitly using the Fluent API
- Seed initial reference data and apply the migration
- Verify the resulting database schema against the original ERD

## 📌 Key topics

- Modeling the full domain in code
- Configuring relationships with the Fluent API
- Seeding initial data
- Applying and reviewing the migration

## 📌 What I learned

### 1. Every ERD box becomes a C# class

Every entity from Day 1's ERD becomes a C# class with properties matching its columns and navigation properties representing its relationships. The domain model reviewed today:

- **`ApplicationUser`** — inherits from ASP.NET Core Identity's `IdentityUser`, so username, email, password hash, and roles are managed by Identity. A `FullName` property and a navigation property to the linked `Patient` profile were added on top.
- **`Patient`** — `Id`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `PhoneNumber`, `CreatedAt`, plus a nullable `ApplicationUserId` (a patient profile can exist without being linked to an Identity account) and navigation properties for `ApplicationUser`, `VitalSigns`, `Medications`, and `Appointments`.
- **`VitalSign`** — `PatientId` as the foreign key, plus `HeartRate`, `SystolicPressure`, `DiastolicPressure`, `OxygenSaturation`, `RecordedAt`, and optional `Notes`, with a `Patient` navigation property.
- **`Medication`** — `PatientId`, `Name`, `Dosage`, `Frequency`, `StartDate`, optional `EndDate`, optional `Notes`.
- **`Appointment`** — `PatientId`, `AppointmentDate`, `DoctorName`, `Reason`, `Status`, optional `Notes`.

> **Note to self:** `DoctorName` stayed as a plain string rather than becoming a `DoctorId`, since that's what the current ERD specifies. An earlier discussion about a dedicated Doctor entity only ever made it into the diagram, not into the entity model — worth remembering before assuming the code and the ERD always say the same thing.

Keeping entity classes focused on data shape, with business logic living in service classes rather than the entities themselves, keeps the model layer simple and predictable as the project grows.

> 📸 **Attach a photo here** — a screenshot of the `Patient`, `VitalSign`, `Medication`, and `Appointment` entity classes side by side.

> 📸 **Attach a photo here** — a screenshot of `ApplicationUser : IdentityUser`, including the `FullName` property and its navigation to `Patient`.

### 2. The Fluent API makes relationships explicit instead of implicit

While EF Core can infer many relationships by convention, the Fluent API — configured in `OnModelCreating` — makes relationships, required fields, and constraints explicit. This matters most whenever a relationship's cardinality or delete behavior isn't the default EF Core would guess.

**`ApplicationUser` ↔ `Patient` — one-to-one.** `Patient.ApplicationUserId` is the foreign key pointing to `AspNetUsers.Id`, backed by a unique filtered index so one Identity user can never be linked to more than one patient profile. Delete behavior is `SetNull`: deleting an Identity user does **not** delete the patient record — it just clears `ApplicationUserId`.

**`Patient` ↔ `VitalSign` / `Medication` / `Appointment` — one-to-many, each.** One patient can have many of each; delete behavior is `Cascade`, so deleting a patient removes their related records too.

```csharp
modelBuilder.Entity<Patient>()
    .HasOne(p => p.ApplicationUser)
    .WithOne(u => u.Patient)
    .HasForeignKey<Patient>(p => p.ApplicationUserId)
    .OnDelete(DeleteBehavior.SetNull);

modelBuilder.Entity<Patient>()
    .HasMany(p => p.VitalSigns)
    .WithOne(v => v.Patient)
    .HasForeignKey(v => v.PatientId)
    .OnDelete(DeleteBehavior.Cascade);

modelBuilder.Entity<VitalSign>()
    .Property(v => v.OxygenSaturation)
    .HasPrecision(5, 2);
```

`OxygenSaturation` also got an explicit `.HasPrecision(5, 2)` so the database stores it with a defined decimal precision, rather than letting EF Core pick an unspecified configuration.

> **Note to self:** explicitly configuring cascade delete behavior avoids the classic surprise of an unexpected mass deletion when a parent record disappears. `SetNull` vs `Cascade` isn't a default worth trusting blindly — it's a decision worth writing down.

> 📸 **Attach a photo here** — a screenshot of `AppDbContext`'s `OnModelCreating`, showing the Fluent API relationships and delete behaviors.

### 3. Seed data means the API has something to return on day one

Seed data — starter roles, a couple of sample patients — gives the API something real to return immediately after the first migration, without manually inserting test data through Postman before any endpoint can be demoed.

`SeedData.cs` creates the three application roles — **Admin, Doctor, and Patient** — plus default accounts for each, assigns the appropriate roles, and creates sample patient data along with sample vital signs, medications, and appointments.

> 📸 **Attach a photo here** — a screenshot of `SeedData.cs`, showing the Admin/Doctor/Patient roles and the sample data creation.

### 4. Reviewing a migration before trusting it

`dotnet ef migrations add <Name>` generates a migration from the full entity set — and it's worth actually reading the generated file before applying it, confirming the expected tables, columns, and foreign keys are present, and that nothing unexpected (an accidental cascade delete, a missing index) slipped in silently.

The project currently has four migrations:

```text
20260816202728_InitialCreate
20260819135442_AddIdentityAuthentication
20260823194443_LinkPatientToApplicationUser
20260823200017_UpdatePatientApplicationUserLink
```

The latest, `UpdatePatientApplicationUserLink`, was reviewed line by line. It renamed the patient foreign-key column from `UserId` to `ApplicationUserId`, removed the old foreign key and index, created a new unique index on `ApplicationUserId`, added the new foreign key from `Patients.ApplicationUserId` to `AspNetUsers.Id`, and configured `SetNull` delete behavior — matching the current `Patient` entity and Fluent API configuration exactly.

> **Note to self:** reviewing a generated migration file before applying it is a five-minute habit that catches schema mistakes before they reach a real database. Skipping this review is how an unintended cascade delete or a missing `NOT NULL` constraint quietly ships.

> 📸 **Attach a photo here** — a screenshot of the migrations folder, showing all four migrations.

> 📸 **Attach a photo here** — a screenshot of `UpdatePatientApplicationUserLink.cs`, highlighting the rename, unique index, and foreign key.

### 5. Applying the migration — and reading "no changes" correctly

```powershell
dotnet ef database update
```

```text
Build succeeded.
No migrations were applied. The database is already up to date.
Done.
```

This was **not** an error. It meant every existing migration was already applied to the SQL Server database — EF Core had no new schema changes to push.

> 📸 **Attach a photo here** — a terminal screenshot showing `dotnet ef migrations list` output (`InitialCreate`, `AddIdentityAuthentication`, `LinkPatientToApplicationUser`, `UpdatePatientApplicationUserLink`).

> 📸 **Attach a photo here** — a terminal screenshot showing the `dotnet ef database update` result above.

### 6. Verifying the schema directly against the database

Working in **Visual Studio Code** rather than SQL Server Management Studio, the **MSSQL extension** was used to connect to the database and run:

```sql
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

The query returned:

```text
__EFMigrationsHistory
Appointments
AspNetRoleClaims
AspNetRoles
AspNetUserClaims
AspNetUserLogins
AspNetUserRoles
AspNetUsers
AspNetUserTokens
Medications
Patients
VitalSigns
```

This confirmed the application tables (`Patients`, `VitalSigns`, `Medications`, `Appointments`) exist alongside the Identity tables (`AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, and others). `__EFMigrationsHistory` is EF Core's own record of which migrations have already been applied.

> 📸 **Attach a photo here** — a screenshot of the MSSQL extension connection to `(localdb)\MSSQLLocalDB` and the `CardiacPatientMonitoringDb` database.

> 📸 **Attach a photo here** — a screenshot of the `INFORMATION_SCHEMA.TABLES` query in the VS Code SQL editor.

> 📸 **Attach a photo here** — a screenshot of the query results, showing the full table list above.

## 📌 What I built — hands-on lab

- [x] Implemented entity classes for every table in Day 1's ERD, including navigation properties
- [x] Configured the `ApplicationUser` ↔ `Patient` one-to-one relationship via the Fluent API, with `SetNull` delete behavior
- [x] Configured `Patient` ↔ `VitalSign`, `Patient` ↔ `Medication`, and `Patient` ↔ `Appointment` as one-to-many, with `Cascade` delete behavior
- [x] Set explicit decimal precision on `OxygenSaturation` with `.HasPrecision(5, 2)`
- [x] Reviewed all four existing migrations, focusing closely on the latest one
- [x] Ran `dotnet ef database update` and confirmed the database was already current
- [x] Verified the live schema against `INFORMATION_SCHEMA.TABLES` using the MSSQL extension in VS Code
- [x] Reviewed `SeedData.cs` and confirmed Admin, Doctor, and Patient roles plus sample data exist

**Tools:** Entity Framework Core · SQL Server · MSSQL extension for VS Code

## 📌 From blueprint to database

```text
Day 1
ERD in draw.io
        ↓
Day 2
C# Entity Classes
        ↓
Fluent API Configuration (OnModelCreating)
        ↓
EF Core Migrations (4 reviewed)
        ↓
dotnet ef database update
        ↓
SQL Server Schema (verified via INFORMATION_SCHEMA.TABLES)
        ↓
SeedData.cs (Admin, Doctor, Patient + sample data)
```

The ERD from Day 1 wasn't just a reference — it was the actual spec the entity model, Fluent API config, and migrations were checked against, box by box.

## 📌 Migration history

| Migration | What it did |
|---|---|
| `InitialCreate` | Established the baseline schema |
| `AddIdentityAuthentication` | Added ASP.NET Core Identity tables (`AspNetUsers`, `AspNetRoles`, etc.) |
| `LinkPatientToApplicationUser` | First pass at connecting `Patient` to an Identity user |
| `UpdatePatientApplicationUserLink` | Renamed `UserId` → `ApplicationUserId`, added a unique index, reconfigured the FK with `SetNull` |

## 📌 Verified tables

```text
Application tables          Identity tables              EF Core internal
├── Patients                ├── AspNetUsers               └── __EFMigrationsHistory
├── VitalSigns               ├── AspNetRoles
├── Medications              ├── AspNetUserRoles
└── Appointments             ├── AspNetUserClaims
                             ├── AspNetRoleClaims
                             ├── AspNetUserLogins
                             └── AspNetUserTokens
```

## 📌 Result

By the end of Day 2, the C# entity model, navigation properties, Fluent API relationships, foreign keys, delete behaviors, Identity structure, migrations, seed data, and SQL Server database schema were all confirmed to be connected correctly — matching the ERD from Day 1. The main objectives of Day 2 are complete.

---

<div align="center">

**Day 2 — Complete ✅**

`EF Core` · `Fluent API` · `Migrations` · `SQL Server` · `Identity` · `Seed Data` · `Schema Verified`

*— end of Day 2*

</div>