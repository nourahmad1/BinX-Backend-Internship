<div align="center">

# Day 1 — Sprint 1 Planning & Project Database Design

*Field notes from the day I stopped coding for a moment and designed the whole database before touching a single migration.*

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity%20Framework-Core-512BD4?logo=dotnet&logoColor=white)
![Trello](https://img.shields.io/badge/Sprint%20Board-Trello-0052CC?logo=trello&logoColor=white)
![draw.io](https://img.shields.io/badge/ERD-draw.io-F08705?logo=diagramsdotnet&logoColor=white)
![Status](https://img.shields.io/badge/status-complete-2ea44c)

`⏱ 8 hours` · `📋 Sprint Planning` · `🗂️ Full Database Schema`

</div>

---

## 📌 Today in one sentence

Planning Sprint 1 and designing the complete database foundation for the Cardiac Patient Monitoring API — a sized Sprint Backlog, every entity the full professional baseline will need, a normalized schema, and a finalized ERD, before writing a single line of implementation code.

## 📌 Learning objectives

- Run a Sprint Planning session and translate it into a sized, realistic backlog
- Design the capstone project's complete database schema
- Finalize the schema as a documented ERD

## 📌 Key topics

- Running Sprint Planning
- Designing the capstone's full schema
- Finalizing the ERD
- Sizing tasks realistically

## 📌 What I learned

### 1. Sprint Planning starts with one question

Sprint Planning opens every Phase 3 sprint with a single question: what does "done" look like for this week? For Sprint 1, done means a complete database schema, applied migrations, and a working set of core routes covering the project's primary patient-monitoring flow.

The Sprint Goal gets written as one sentence at the top of the backlog, and every task added afterward should visibly serve it:

> **Sprint Goal:** Build the foundation of the Cardiac Patient Monitoring API by finalizing the database schema, preparing the EF Core data model and migrations, and delivering the first core patient-monitoring routes.

### 2. The backlog is broken into small, trackable tasks

Instead of one large task like "Build the Cardiac Patient Monitoring API," the work was broken into smaller, measurable pieces on the Sprint Backlog in Trello:

```text
Sprint 1 Backlog
├── Design & Finalize Database Schema
├── Implement EF Core Data Model
├── Create & Apply Initial Database Migration
├── Implement Core Read Routes
├── Implement Patient Monitoring Write Operations
├── Validation & Error Handling
├── Test Sprint 1 Features
└── Document Sprint 1
```

Breaking the work down this way makes the sprint easier to manage, allows real progress tracking, and makes the daily stand-up meaningful — a genuine update or a genuine blocker to report, instead of "still working on the same big thing."

### 3. Designing the full schema, not just this sprint's slice

Unlike a small practice schema, Sprint 1 designs the entire capstone project's data model in one pass — every entity the professional baseline will eventually need, even if some won't be touched by an API endpoint until a later sprint.

For the Cardiac Patient Monitoring API, the entities identified were **ApplicationUser, Patient, VitalSign, Medication, and Appointment**:

- `ApplicationUser` — the user's authentication and Identity account
- `Patient` — the patient's personal and basic profile information
- `VitalSign` — medical readings recorded for a patient, such as heart rate, systolic/diastolic blood pressure, and oxygen saturation
- `Medication` — medications prescribed or assigned to a patient, including dosage, frequency, and dates
- `Appointment` — appointments between patients and doctors, including date, doctor, reason, status, and notes

> **Note to self:** identify the entities required by the full project baseline, not only the entities needed for the first few endpoints.

### 4. Applying normalization to keep the schema clean

Normalization principles keep the data organized, reduce unnecessary duplication, and make relationships clear. A patient's name, phone number, and date of birth shouldn't be repeated inside every vital-sign record — instead, patient information lives in the `Patient` table and connects to vital signs, medications, and appointments through `PatientId`.

```text
Patients(Id PK, ApplicationUserId FK, FirstName, LastName, DateOfBirth, Gender, PhoneNumber, CreatedAt)
VitalSigns(Id PK, PatientId FK, HeartRate, SystolicBP, DiastolicBP, OxygenSaturation, RecordedAt)
Medications(Id PK, PatientId FK, Name, Dosage, Frequency, StartDate, EndDate)
Appointments(Id PK, PatientId FK, DoctorName, AppointmentDate, Reason, Status, Notes)
```

### 5. Mapping relationships and cardinality

- `ApplicationUser` → `Patient` — **one-to-zero-or-one**: an Identity user may have one patient profile, and the patient profile connects to one user
- `Patient` → `VitalSign` — **one-to-many**: one patient can have many vital-sign records
- `Patient` → `Medication` — **one-to-many**: one patient can have multiple medications
- `Patient` → `Appointment` — **one-to-many**: one patient can have multiple appointments

### 6. Finalizing the ERD

The ERD produced this week is the same one the Week 9 Definition of Done audit will check for — worth building carefully and keeping updated as the schema evolves across sprints, rather than treating it as a one-time Day 1 exercise. A schema diagram that falls out of sync with the actual database quickly becomes actively misleading rather than merely outdated.

```text
ApplicationUser
       │
       │ 1 ─── 0..1
       │
       ▼
    Patient
       │
       ├──── 1 ─── * ─── VitalSign
       │
       ├──── 1 ─── * ─── Medication
       │
       └──── 1 ─── * ─── Appointment
```

Built in draw.io, the ERD shows each entity, its important fields, its primary key, its foreign keys, and the relationships between entities — it becomes the blueprint for the database. Later, when the entities are implemented and EF Core is configured, the ERD should already answer exactly what relationships and constraints are needed.

## 📌 Complete Database Design

The database design for the Cardiac Patient Monitoring API was created before implementing the EF Core model. The purpose of this design was to clearly identify the data required by the system, define how the entities are connected, and make sure the database can support the project's future features without unnecessary duplication.

The final database contains five main entities:

1. `ApplicationUser`
2. `Patient`
3. `VitalSign`
4. `Medication`
5. `Appointment`

Each entity has a specific responsibility, while relationships connect the entities together.

### Entity 1 — ApplicationUser

`ApplicationUser` represents the authenticated user of the system. It extends ASP.NET Core Identity's `IdentityUser`, so authentication-related information such as username, email, password hash, and user ID is managed by Identity.

| Attribute | Type | Key / Constraint | Description |
|---|---|---|---|
| `Id` | `string` | PK | Unique Identity user identifier |
| `FullName` | `string` | Required | Full name of the user |
| `UserName` | `string` | Identity | Username used for authentication |
| `Email` | `string` | Identity | User's email address |
| `PasswordHash` | `string` | Identity | Securely stored password hash |
| `Patient` | `Patient?` | Navigation Property | Optional patient profile connected to the user |

```text
ApplicationUser
       │
       │ 1
       │
       │ 0..1
       ▼
    Patient
```

**Relationship:** One `ApplicationUser` can have zero or one `Patient` profile.

### Entity 2 — Patient

`Patient` represents the patient's medical profile and basic personal information. It is the central entity of the cardiac monitoring system because vital signs, medications, and appointments all belong to a patient.

| Attribute | Type | Key / Constraint | Description |
|---|---|---|---|
| `Id` | `int` | PK | Unique patient identifier |
| `ApplicationUserId` | `string?` | FK | Links the patient to an Identity user |
| `FirstName` | `string` | Required | Patient's first name |
| `LastName` | `string` | Required | Patient's last name |
| `DateOfBirth` | `DateTime` | Required | Patient's date of birth |
| `Gender` | `string` | Required | Patient's gender |
| `PhoneNumber` | `string` | Required | Patient's contact number |
| `CreatedAt` | `DateTime` | Required | Date the patient profile was created |
| `ApplicationUser` | `ApplicationUser?` | Navigation Property | Connected Identity account |
| `VitalSigns` | `ICollection<VitalSign>` | Navigation Property | Patient's vital-sign records |
| `Medications` | `ICollection<Medication>` | Navigation Property | Patient's medications |
| `Appointments` | `ICollection<Appointment>` | Navigation Property | Patient's appointments |

```text
Patient
  │
  ├──── 1 ─── * ─── VitalSign
  │
  ├──── 1 ─── * ─── Medication
  │
  └──── 1 ─── * ─── Appointment
```

**Patient → VitalSign:** one patient can have many vital-sign records.
**Patient → Medication:** one patient can have many medications.
**Patient → Appointment:** one patient can have many appointments.

### Entity 3 — VitalSign

`VitalSign` stores individual cardiac and health measurements recorded for a patient. Multiple readings can be recorded for the same patient over time.

| Attribute | Type | Key / Constraint | Description |
|---|---|---|---|
| `Id` | `int` | PK | Unique vital-sign record identifier |
| `PatientId` | `int` | FK | Identifies the patient |
| `HeartRate` | `int` | Required | Heart rate in beats per minute |
| `SystolicPressure` | `int` | Required | Upper blood-pressure value |
| `DiastolicPressure` | `int` | Required | Lower blood-pressure value |
| `OxygenSaturation` | `decimal` | Required | Blood oxygen saturation |
| `RecordedAt` | `DateTime` | Required | Time when the reading was recorded |
| `Notes` | `string?` | Optional | Additional information about the reading |
| `Patient` | `Patient` | Navigation Property | Patient who owns the reading |

```text
Patient 1 ─────────── * VitalSign

Patient
   │
   ├── VitalSign #1
   ├── VitalSign #2
   ├── VitalSign #3
   └── VitalSign #4
```

This allows the system to maintain a history of the patient's measurements instead of overwriting previous readings.

### Entity 4 — Medication

`Medication` represents medication assigned or prescribed to a patient.

| Attribute | Type | Key / Constraint | Description |
|---|---|---|---|
| `Id` | `int` | PK | Unique medication record identifier |
| `PatientId` | `int` | FK | Identifies the patient |
| `Name` | `string` | Required | Medication name |
| `Dosage` | `string` | Required | Prescribed dosage |
| `Frequency` | `string` | Required | How frequently the medication is taken |
| `StartDate` | `DateTime` | Required | Medication start date |
| `EndDate` | `DateTime?` | Optional | Medication end date |
| `Notes` | `string?` | Optional | Additional medication instructions |
| `Patient` | `Patient` | Navigation Property | Patient taking the medication |

```text
Patient 1 ─────────── * Medication

Patient
   │
   ├── Aspirin
   ├── Bisoprolol
   └── Atorvastatin
```

### Entity 5 — Appointment

`Appointment` represents a scheduled or completed appointment associated with a patient.

| Attribute | Type | Key / Constraint | Description |
|---|---|---|---|
| `Id` | `int` | PK | Unique appointment identifier |
| `PatientId` | `int` | FK | Identifies the patient |
| `AppointmentDate` | `DateTime` | Required | Date and time of appointment |
| `DoctorName` | `string` | Required | Doctor assigned to the appointment |
| `Reason` | `string` | Required | Reason for the appointment |
| `Status` | `string` | Required | Appointment status |
| `Notes` | `string?` | Optional | Additional appointment notes |
| `Patient` | `Patient` | Navigation Property | Patient associated with the appointment |

```text
Patient 1 ─────────── * Appointment
```

One patient can have multiple appointments.

### Entity relationship summary

```text
                    ApplicationUser
                          │
                          │ 1 : 0..1
                          │
                          ▼
                       Patient
                     /    |    \
                    /     |     \
                   /      |      \
                  ▼       ▼       ▼
            VitalSign  Medication  Appointment
             1 : Many    1 : Many     1 : Many
```

| Parent Entity | Child Entity | Relationship | Foreign Key |
|---|---|---|---|
| `ApplicationUser` | `Patient` | One-to-Zero-or-One | `Patient.ApplicationUserId` |
| `Patient` | `VitalSign` | One-to-Many | `VitalSign.PatientId` |
| `Patient` | `Medication` | One-to-Many | `Medication.PatientId` |
| `Patient` | `Appointment` | One-to-Many | `Appointment.PatientId` |

### Final ERD diagram

The complete ERD was designed using **draw.io** and represents the database blueprint used for the EF Core implementation.

> 🖼️ Insert the final draw.io ERD screenshot here.

```text
┌─────────────────────────┐
│    ApplicationUser      │
├─────────────────────────┤
│ PK Id : string          │
│ FullName : string       │
│ UserName : string       │
│ Email : string          │
│ PasswordHash : string   │
└────────────┬────────────┘
             │
             │ 1 : 0..1
             ▼
┌─────────────────────────┐
│        Patient          │
├─────────────────────────┤
│ PK Id : int             │
│ FK ApplicationUserId    │
│ FirstName : string      │
│ LastName : string       │
│ DateOfBirth : DateTime  │
│ Gender : string         │
│ PhoneNumber : string    │
│ CreatedAt : DateTime    │
└──────┬──────┬───────┬───┘
       │      │       │
     1 : *  1 : *    1 : *
       │      │       │
       ▼      ▼       ▼
┌──────────┐ ┌────────────┐ ┌─────────────┐
│VitalSign │ │ Medication │ │ Appointment │
├──────────┤ ├────────────┤ ├─────────────┤
│PK Id     │ │PK Id       │ │PK Id        │
│FK Patient│ │FK Patient  │ │FK Patient   │
│HeartRate │ │Name        │ │Date         │
│Systolic  │ │Dosage      │ │DoctorName   │
│Diastolic │ │Frequency   │ │Reason       │
│Oxygen    │ │StartDate   │ │Status       │
│RecordedAt│ │EndDate     │ │Notes        │
│Notes     │ │Notes       │ └─────────────┘
└──────────┘ └────────────┘
```

### Primary keys & foreign keys

A **Primary Key (PK)** uniquely identifies each record in a table — each patient has a unique `Id`. A **Foreign Key (FK)** connects one table to another. For example, a `VitalSign` with `PatientId = 1` belongs to the `Patient` with `Id = 1`:

```text
Patient.Id
     ▲
     │
     │ FK
     │
VitalSign.PatientId
```

The same principle applies to `Medication` and `Appointment`.

### Navigation properties

Navigation properties let EF Core navigate between related entities in C#. `VitalSign` contains:

```csharp
public Patient Patient { get; set; } = null!;
```

meaning this `VitalSign` belongs to a `Patient`. `Patient` contains:

```csharp
public ICollection<VitalSign> VitalSigns { get; set; }
    = new List<VitalSign>();
```

meaning this `Patient` can have multiple `VitalSign` records. Together, these represent `Patient 1 ─── * VitalSign`. The same pattern applies to medications and appointments.

### Database design decisions

**1. Patient information is stored once** — not repeated inside every vital-sign, medication, or appointment record. Instead, `VitalSigns`, `Medications`, and `Appointments` all connect back to a single `Patient`. This reduces duplication and follows normalization principles.

**2. Vital signs are historical records** — a new record is created for each measurement instead of updating the previous one:

```text
10:00 → HeartRate 72
12:00 → HeartRate 76
14:00 → HeartRate 81
16:00 → HeartRate 75
```

This lets the system analyze a patient's measurements over time.

**3. ApplicationUser and Patient are separated** — authentication data is handled by ASP.NET Core Identity, while patient-specific information lives in the `Patient` entity, keeping authentication concerns separate from medical-profile data.

**4. Optional information is nullable** — fields such as `EndDate`, `Notes`, and `ApplicationUserId` are nullable because they may not always have a value.

### 7. Sizing tasks realistically

A backlog task like "build the patient monitoring feature" is too large to size or track meaningfully. Breaking it into "implement Patient entity and migration," "implement paginated `GET /patients`," "implement `GET /patients/{id}`" gives each task a clear, checkable definition of done.

Sprint 1 task estimates:

| Task | Estimate |
|---|---|
| Design & Finalize Database Schema | 4 hours |
| Implement EF Core Data Model | 4 hours |
| Create & Apply Initial Database Migration | 3 hours |
| Implement Core Read Routes | 6 hours |
| Implement Patient Monitoring Write Operations | 6 hours |
| Validation & Error Handling | 4 hours |
| Test Sprint 1 Features | 4 hours |
| Document Sprint 1 | 2 hours |
| **Total planned work** | **33 hours** |

That leaves roughly 7 hours as a reasonable buffer for debugging, reviews, GitHub work, meetings, and other sprint activities within the 40-hour week.

> **Note to self:** a sprint with no written backlog tends to expand to fill the week with whatever feels most interesting moment to moment. Fifteen minutes of planning on Day 1, with tasks sized to about a day each, saves far more time than it costs across the week.

## 📌 What I built — hands-on lab

- [x] Wrote a one-sentence Sprint 1 goal and set up the Sprint Backlog board in Trello
- [x] Listed every entity the Cardiac Patient Monitoring API will need across its full professional baseline — not just Sprint 1's scope
- [x] Defined attributes, primary keys, and foreign keys for all five entities
- [x] Defined navigation properties for every relationship
- [x] Designed the complete database schema, applying normalization principles
- [x] Mapped relationships and cardinality between all entities
- [x] Diagrammed the finalized schema as an ERD in draw.io
- [x] Documented key database design decisions
- [x] Broke Sprint 1's specific scope into sized backlog tasks of roughly half a day to a day each

**Tools:** Trello · draw.io · Entity Framework Core (design phase)

## 📌 Day 1 final output

At the end of Day 1, the project has a complete database blueprint ready for implementation — not new API code.

```text
Sprint Goal
     ↓
Trello Backlog
     ↓
Entity Identification
     ↓
Attributes Definition
     ↓
Relationships
     ↓
Normalization
     ↓
ERD in draw.io
     ↓
Task Sizing
     ↓
Day 2 — EF Core Implementation
```

**Day 1 checklist**

- [x] Sprint Goal
- [x] Trello Sprint Backlog
- [x] Full Entity List
- [x] Entity Attributes
- [x] Primary Keys
- [x] Foreign Keys
- [x] Navigation Properties
- [x] Relationships & Cardinality
- [x] Normalized Schema
- [x] ERD designed in draw.io
- [x] Task Estimation
- [x] Day 2 implementation plan

> Day 1 is complete. The database blueprint is ready to become the EF Core model on Day 2.

## 📌 Why this is different from Week 5

```text
Week 5
Test and harden the existing project
        ↓
xUnit + Moq + WebApplicationFactory + Global Exception Middleware
        ↓
38/38 tests passing, stable foundation

Sprint 1, Day 1
Plan and design before implementing
        ↓
Sprint Goal + Trello Backlog + Full Entity List + Normalized Schema + ERD
        ↓
Blueprint ready for Day 2's EF Core implementation
```

Week 5 was about proving the existing code was solid. Sprint 1 starts a new phase — Phase 3 — where the database and routes for the full professional baseline get planned and built from a clean, deliberate blueprint rather than growing organically.

## 📌 Entities identified

```text
CardiacPatientMonitoring — Database Schema
│
├── ApplicationUser   (Identity account)
├── Patient           (profile, linked to ApplicationUser)
├── VitalSign         (heart rate, blood pressure, oxygen saturation)
├── Medication        (dosage, frequency, dates)
└── Appointment       (doctor, date, reason, status, notes)
```

## 📌 Sprint 1 backlog snapshot

| Task | Estimate | Status |
|---|---|---|
| Design & Finalize Database Schema | 4h | ✅ Done (Day 1) |
| Implement EF Core Data Model | 4h | ⏳ Day 2 |
| Create & Apply Initial Database Migration | 3h | ⏳ Day 2 |
| Implement Core Read Routes | 6h | ⏳ Upcoming |
| Implement Patient Monitoring Write Operations | 6h | ⏳ Upcoming |
| Validation & Error Handling | 4h | ⏳ Upcoming |
| Test Sprint 1 Features | 4h | ⏳ Upcoming |
| Document Sprint 1 | 2h | ⏳ Upcoming |

By the end of Day 1: a clear Sprint Goal, an organized Trello backlog, a complete list of entities, a normalized database design, a finalized draw.io ERD, and realistically sized Sprint 1 tasks — ready to turn into EF Core code and migrations on Day 2.

---

<div align="center">

**Day 1 — Complete ✅**

`Sprint 1` · `Sprint Planning` · `Database Schema Design` · `ERD` · `Trello Backlog`

*— end of Day 1*

</div>
