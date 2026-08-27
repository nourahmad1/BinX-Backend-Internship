<div align="center">

# Day 4 — Implementing Core Routes II: Write Operations & Business Logic; Mentor Code Review

*Field notes from the day a prescription became more than an insert — stock checks, line totals, a real transaction, and a pull request ready for review.*

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity%20Framework-Core-512BD4?logo=dotnet&logoColor=white)
![Transactions](https://img.shields.io/badge/EF%20Core-Transactions-5E5E5E?logo=dotnet&logoColor=white)
![RBAC](https://img.shields.io/badge/Auth-Role--Based%20Access-000000?logo=jsonwebtokens&logoColor=white)
![GitHub](https://img.shields.io/badge/Review-Pull%20Request-181717?logo=github&logoColor=white)
![Status](https://img.shields.io/badge/status-complete-2ea44c)

`⏱ 8 hours` · `💊 Prescription Workflow` · `🔒 Transactional Writes` · `👀 Mentor Review`

</div>

---

## 📌 Today in one sentence

Building the prescription and medication inventory workflow for the Cardiac Patient Monitoring API — real business logic beyond field mapping, a multi-step write wrapped in a database transaction, role-based authorization, and a clean pull request opened for mentor review.

## 📌 Learning objectives

- Implement a write operation with real business logic beyond simple field mapping
- Wrap a multi-step write operation in a database transaction
- Prepare a clean pull request for mentor code review

## 📌 Key topics

- Business logic beyond simple CRUD
- Transactions for multi-step operations
- Preparing a clean pull request
- The mentor code review

## 📌 What I learned

### 1. Business logic beyond simple CRUD

Creating a prescription isn't just inserting a row — it needs to check medication stock availability, calculate line totals and an overall prescription total, and decrement stock quantities, all as part of one coherent operation. This is exactly the kind of logic that belongs in the controller's write path with real validation, not just DTO-to-entity mapping — and exactly the kind of logic Week 5's unit tests are best suited to cover thoroughly.

Before touching prescriptions, the existing `Medication` entity was extended to support inventory: `StockQuantity` and `UnitPrice` were added to the Create, Update, and Response DTOs, and `MedicationsController` was updated to reject negative stock or price values. Response mappings were updated so medication data now includes current stock and price — verified through Swagger.

### 2. Modeling the prescription itself

`Prescription` and `PrescriptionItem` entities represent a prescription and the medications it contains. A prescription belongs to a patient and contains multiple prescription items; each item stores the medication, quantity, unit price, and line total.

```csharp
public class Prescription
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
}

public class PrescriptionItem
{
    public int Id { get; set; }
    public int PrescriptionId { get; set; }
    public Prescription Prescription { get; set; } = null!;
    public int MedicationId { get; set; }
    public Medication Medication { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
```

`PrescriptionCreateDto` and the response DTOs were added alongside the entities to control exactly what the API accepts and returns.

`AppDbContext` registered the new `Prescriptions` and `PrescriptionItems` DbSets, with Fluent API relationships configured so a `Patient` can have multiple `Prescriptions`, a `Prescription` can contain multiple `PrescriptionItems`, and each `PrescriptionItem` connects to a `Medication`. Cascade deletion was used between prescriptions and their items, while restricted deletion was used between medications and prescription items — protecting existing prescription records from disappearing if a medication is removed. Decimal precision was configured for medication prices, prescription totals, and line totals.

### 3. Transactions for multi-step operations

A prescription creation that inserts the prescription, inserts several prescription items, and decrements stock across multiple medications needs all of those steps to succeed or fail together. If stock decrementing fails partway through, the prescription and its items shouldn't be left behind as inconsistent, prescription-without-matching-stock-update data.

Wrapping the whole operation in an EF Core transaction guarantees this all-or-nothing behavior:

```csharp
using var transaction = await _context.Database.BeginTransactionAsync();

try
{
    foreach (var item in dto.Items)
    {
        var medication = await _context.Medications.FindAsync(item.MedicationId);

        var lineTotal = medication.UnitPrice * item.Quantity;

        medication.StockQuantity -= item.Quantity;

        prescription.Items.Add(new PrescriptionItem
        {
            MedicationId = medication.Id,
            Quantity = item.Quantity,
            UnitPrice = medication.UnitPrice,
            LineTotal = lineTotal
        });
    }

    prescription.TotalAmount = prescription.Items.Sum(i => i.LineTotal);

    _context.Prescriptions.Add(prescription);

    await _context.SaveChangesAsync();
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

For each item, the line total is calculated as unit price × quantity, the medication's stock is decremented, and the overall prescription total is the sum of all line totals. If anything unexpected happens, the transaction rolls back — no partial data is saved.

> **Note to self:** a multi-step write operation with no transaction is one that can leave the database in a half-finished, inconsistent state the moment any single step fails. If a real transaction boundary should exist and doesn't, that's the single most important thing to catch in a review.

### 4. Validation before the transaction even starts

Before any database changes happen, `PrescriptionsController` validates that:

- the patient exists
- the prescription contains at least one item
- every requested quantity is greater than zero
- every requested medication actually exists
- **available stock covers the requested quantity** — if the requested quantity exceeds stock, the API returns `400 Bad Request` instead of creating the prescription

Only `ADMIN` or `DOCTOR` roles are allowed to create or access prescriptions — enforced through role-based authorization on the controller.

### 5. Reading a prescription back

A `GET` endpoint retrieves a prescription by ID, loading it together with its items and related medication information, then mapping the result into a `PrescriptionResponseDto`. Tested successfully — `200 OK` with the correct prescription details: medication name, quantity, unit price, line total, and total prescription amount.

### 6. What real testing looked like today

Authorization and validation were tested directly against the running API:

- A **Doctor** creating a prescription → `201 Created` ✅
- A **Patient** attempting to create a prescription → `403 Forbidden` ✅ (role-based authorization confirmed working)
- Requesting **100 units of Atorvastatin** when only **18 were in stock** → `400 Bad Request`, with a clear message showing available stock vs. requested quantity ✅

### 7. Preparing a clean pull request

A pull request ready for review contains focused, logically grouped commits with clear messages, a description explaining what the branch adds and any open questions, and passes locally before it's even opened. A reviewer's time is better spent on design and logic feedback than on catching a build error a local `dotnet build` would have caught first.

### 8. The mentor code review

The mid-sprint mentor code review looks specifically at API design decisions and business logic correctness: is the transaction boundary drawn correctly, does the stock-check logic handle a concurrent request for the last unit of stock, is the DTO projection appropriately scoped. Structured inline GitHub comments let feedback attach to the exact line it concerns — far more actionable than a general comment on the whole pull request.

## 📌 What I built — hands-on lab

- [x] Extended `Medication` with `StockQuantity` and `UnitPrice`, with validation against negative values
- [x] Created `Prescription` and `PrescriptionItem` entities and their DTOs
- [x] Registered `Prescriptions` and `PrescriptionItems` in `AppDbContext` with Fluent API relationships
- [x] Configured cascade deletion (Prescription → Items) and restricted deletion (Medication → Items)
- [x] Configured decimal precision for prices, line totals, and prescription totals
- [x] Implemented `PrescriptionsController` restricted to `ADMIN` and `DOCTOR` roles
- [x] Validated patient existence, item count, quantities, medication existence, and available stock before any writes
- [x] Wrapped prescription creation, item insertion, stock decrement, and total calculation in a single EF Core transaction
- [x] Implemented the `GET` endpoint for retrieving a prescription with its items and medication details
- [x] Verified Doctor creation success (`201`), Patient creation rejection (`403`), and insufficient-stock rejection (`400`)
- [x] Pushed the Sprint 1 branch and opened a pull request with a clear description
- [x] Requested mentor review and addressed feedback

**Tools:** Entity Framework Core · ASP.NET Core · GitHub

## 📌 Prescription creation flow

```text
POST /api/Prescriptions
        │
        ▼
Validate: patient exists?
        │
        ▼
Validate: at least 1 item, quantities > 0?
        │
        ▼
Validate: all medications exist?
        │
        ▼
Validate: stock ≥ requested quantity for every item?
        │
        ├── No  → 400 Bad Request (available vs requested)
        │
        ▼ Yes
BEGIN TRANSACTION
        │
        ├── For each item: LineTotal = UnitPrice × Quantity
        ├── Decrement Medication.StockQuantity
        ├── Prescription.TotalAmount = Σ LineTotal
        ├── SaveChangesAsync()
        │
        ├── Success → COMMIT → 201 Created
        └── Failure → ROLLBACK → error propagated
```

## 📌 Authorization & validation results

| Scenario | Actor | Expected | Result |
|---|---|---|---|
| Create prescription | Doctor | `201 Created` | ✅ Confirmed |
| Create prescription | Patient | `403 Forbidden` | ✅ Confirmed |
| Request 100 units, 18 in stock (Atorvastatin) | Doctor | `400 Bad Request` | ✅ Confirmed |
| Get prescription by ID | Doctor/Admin | `200 OK` with full details | ✅ Confirmed |

## 📌 Why this is different from Day 3

```text
Day 3
Make reading data scale
        ↓
Pagination + Filtering + Sorting + DTO Projection
        ↓
GET /api/Patients returns exactly what's needed

Day 4
Make writing data safe
        ↓
Stock validation + Transactions + Role-based authorization
        ↓
POST /api/Prescriptions either fully succeeds or fully rolls back
```

Day 3 was about efficient reads. Day 4 is about correct writes — a prescription touches three things (the prescription, its items, and medication stock) that all have to change together or not at all, which is exactly what a transaction boundary exists to guarantee.

## 📌 Pull request

- **Branch:** `feature/week6-day4-prescriptions`
- **Contents:** medication stock/price fields, `Prescription`/`PrescriptionItem` entities and DTOs, `AppDbContext` configuration, `PrescriptionsController`, transaction-wrapped creation logic
- **Status:** Opened for mentor review, `dotnet build` passing locally before submission
- **Reviewer:** Mentor, focused on transaction boundary correctness and role-based authorization

---

<div align="center">

**Day 4 — Complete ✅**

`Prescriptions` · `EF Core Transactions` · `Stock Validation` · `Role-Based Access` · `Pull Request Opened`

*— end of Day 4*

</div>
