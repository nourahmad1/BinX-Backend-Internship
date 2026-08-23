
<div align="center">

# ❤️ Cardiac Patient Monitoring API

*Field notes from building a backend that treats a patient as the center of the system, not a row referenced by four unrelated tables.*

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity%20Framework-Core-512BD4)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC2927?logo=microsoftsqlserver&logoColor=white)
![JWT](https://img.shields.io/badge/Auth-JWT-black?logo=jsonwebtokens)
![FluentValidation](https://img.shields.io/badge/FluentValidation-Rules-orange)
![xUnit](https://img.shields.io/badge/Testing-xUnit%20%2B%20Moq-5C2D91)
![Postman](https://img.shields.io/badge/Tested%20with-Postman-FF6C37?logo=postman&logoColor=white)
![Status](https://img.shields.io/badge/status-complete-2ea44f)

**BinX Backend Internship | .NET 10**
`📄 Full testing report:` [`Cardiac_Patient_Monitoring_API_Validation_Testing_Report.docx`](./Cardiac_Patient_Monitoring_API_Validation_Testing_Report.pdf)

</div>

---

## 📌 In one sentence

This project is a backend API built around one idea — **one patient, one place for their medical information** — so that vitals, medications, and appointments aren't separate systems that happen to share a database, but three views onto the same patient, all reachable through the same authenticated, validated, tested pipeline.

## 📌 Objectives

- Design the database and API around the patient as the central entity, not as four disconnected resource sets
- Secure the API with ASP.NET Core Identity and JWT so medical resources are never publicly accessible
- Provide full CRUD for Patients, Vital Signs, Medications, and Appointments, with filtering and search where it makes sense
- Enforce relationships at the API level so records can never attach to a patient that doesn't exist
- Separate the database entities from the public API contract using DTOs
- Validate that incoming data doesn't just fit C#'s types, but actually makes sense for a medical record
- Return meaningful, predictable HTTP status codes for every success and failure case
- Cover the API with both manual Swagger/Postman testing and automated xUnit/Moq tests

## 📌 What I learned

### 1. A request passes through layers before it ever touches the database
It doesn't jump from Swagger straight to SQL Server. It moves through JWT authentication, the controller, a DTO, validation, a business check, EF Core, and finally back out through a response DTO:

```text
HTTP REQUEST → JWT AUTHENTICATION → CONTROLLER → DTO → VALIDATION
→ BUSINESS CHECK → EF CORE → SQL SERVER → RESPONSE DTO → HTTP RESPONSE
```

Keeping these as separate stages is what stops authentication, validation, and database access from collapsing into one large piece of code.

### 2. Authentication is the first gate, not a formality
Built with ASP.NET Core Identity and JWT:

```text
REGISTER → IDENTITY USER → LOGIN → VERIFY CREDENTIALS → GENERATE JWT → PROTECTED REQUESTS
```

A `current-user` endpoint lets the app resolve which user a token belongs to, and every medical controller is marked `[Authorize]` so nothing sensitive is reachable without a valid token:

```text
No token       → 401 Unauthorized
Invalid token  → 401 Unauthorized
Valid token    → Request continues
Forbidden      → 403 Forbidden
```

This was the point where I stopped just building endpoints and started thinking about how the API should actually behave.

### 3. The patient is the entity everything else hangs off
A patient record holds first/last name, date of birth, gender, phone number, and a created-at timestamp. The controller supports the full `CREATE → READ → UPDATE → DELETE` lifecycle, but the important part is *around* the data — read operations use DTO projection with `AsNoTracking()` so the API returns exactly what the client needs instead of exposing the raw entity.

### 4. ❤️ Vital signs are readings tied to a moment in time
Each record captures heart rate, systolic/diastolic pressure, oxygen saturation, a recorded-at timestamp, and notes:

```text
                    PATIENT
                       │
          ┌────────────┼────────────┐
          ▼            ▼            ▼
       Reading 1    Reading 2    Reading 3
```

The API supports getting all readings, one reading, all readings for a patient, and full create/update/delete — ordered by recording time so the most recent measurement comes first.

### 5. 💊 Medications describe ongoing treatment, not a single event
Name, dosage, frequency, start date, end date, and notes, with a patient able to hold many at once. I added search across name, dosage, and frequency, with results ordered by start date — because "what is this patient currently taking" is a more common question than "give me every medication row."

### 6. 📅 Appointments needed to be filterable, not just listable
Each appointment holds a date, doctor, reason, status, and notes. Filtering by patient, status, and doctor — using optional query parameters composed through EF Core — is what lets the backend answer real questions: *appointments for this patient*, *only scheduled ones*, *appointments with this doctor*.

### 7. Relationships are enforced at the API level, not just the database
Before creating a medication (or a vital sign, or an appointment), the API checks that the referenced patient actually exists:

```text
PatientId = 25 → Does patient 25 exist? → YES: CREATE   |   NO: ERROR
```

This keeps medical records from ever attaching to a patient that isn't there.

### 8. 📦 What the database stores and what the API accepts aren't the same thing
Every resource gets its own Create/Update/Response DTOs (`PatientCreateDto`, `PatientUpdateDto`, `PatientResponseDto`, and the same pattern for Medications, Vitals, and Appointments). DTOs give the API explicit control over incoming data, outgoing data, which fields are updateable, and where validation rules live — and they keep the database entity from becoming the public contract.

### 9. ✅ Validation isn't just "can C# store this?" — it's "does this make sense?"
A missing `DateTime` doesn't arrive as *missing*; C# quietly defaults it to `0001-01-01T00:00:00`. That's technically valid, but meaningless as a medication start date. Catching that meant validation had to go beyond required-field and max-length checks and start asking whether a value made sense for the application, not just whether it compiled.

### 10. 🛡️ HTTP status codes carry meaning, not just pass/fail
```text
             SUCCESS                              FAILURE
                │                                     │
      ┌─────────┼─────────┐              ┌────────────┼────────────┐
      ▼         ▼         ▼              ▼            ▼            ▼
     200       201       204            400          401          404
    OK       CREATED    DELETED       Invalid   Unauthenticated  Not Found
```
Plus `403 Forbidden` when the user is authenticated but not authorized. Predictable status codes are what make the API usable by a client that isn't guessing.

### 11. 🧪 Manual testing and automated testing catch different things
Manual testing in Swagger covered request bodies, authentication, validation, response bodies, status codes, invalid IDs, invalid data, and relationships. Automated tests (xUnit + `WebApplicationFactory` + Moq) turned the important cases into repeatable assertions instead of things I had to remember to re-check by hand:

```text
Request unknown patient → API runs → Assert expected result → 404 Not Found
```

### 12. 🐛 Not every bug is in the code
At one point the build failed because files like `Microsoft.Data.SqlClient.dll` and `Microsoft.AspNetCore.Identity.EntityFrameworkCore.dll` were locked by another process, and the build system kept retrying until it gave up. The fix was finding the process holding the files, stopping it, cleaning the build output, restoring packages, and rebuilding — a reminder that debugging is sometimes about the environment around the code, not the code itself.

## 📌 Project structure

```text
CardiacPatientMonitoring
│
├── Controllers
│   ├── AuthController
│   ├── PatientsController
│   ├── VitalSignsController
│   ├── MedicationsController
│   └── AppointmentsController
│
├── DTOs
│   ├── Patient
│   ├── VitalSign
│   ├── Medication
│   └── Appointment
│
├── Entities
│   ├── Patient
│   ├── VitalSign
│   ├── Medication
│   └── Appointment
│
├── Data
│   └── AppDbContext
│
├── Services
│
├── Middleware
│
└── Tests
```

## 📌 Technology stack

| Area                 | Technology                          |
| -------------------- | ------------------------------------ |
| Language              | C#                                   |
| Framework             | ASP.NET Core                         |
| Runtime               | .NET 10                              |
| Database              | SQL Server                           |
| ORM                   | Entity Framework Core                |
| Authentication        | ASP.NET Core Identity                |
| Security              | JWT                                  |
| Validation            | FluentValidation + Data Annotations  |
| Documentation         | Swagger                              |
| Testing               | xUnit                                |
| Integration Testing   | WebApplicationFactory                |
| Mocking               | Moq                                  |
| Database Versioning   | EF Core Migrations                   |

## 📌 What I built — hands-on lab

- [x] Designed the database around the patient as the central entity, with Vitals, Medications, and Appointments all relating back to it
- [x] Implemented registration, login, and JWT-based authentication with ASP.NET Core Identity
- [x] Added a current-user endpoint and locked down medical controllers with `[Authorize]`
- [x] Built full CRUD for Patients, Vital Signs, Medications, and Appointments
- [x] Added medication search (name, dosage, frequency) and appointment filtering (patient, status, doctor)
- [x] Enforced patient-existence checks before creating Vitals, Medications, or Appointments
- [x] Split every resource into Create/Update/Response DTOs to separate the database from the API contract
- [x] Added validation covering required fields, max lengths, phone format, and valid vital-sign ranges — including catching the `0001-01-01` default-date issue
- [x] Standardized HTTP responses (`200/201/204` for success, `400/401/403/404` for failure)
- [x] Tested manually through Swagger and automated key scenarios with xUnit, `WebApplicationFactory`, and Moq

## 📌 Testing summary

Full detailed evidence — including per-endpoint request/response pairs and screenshot placeholders — lives in the linked report. High-level coverage:

| Module                          | Cases | Result   |
| -------------------------------- | ----: | -------- |
| Register / Login                 |     9 | ✅ Pass   |
| JWT / Current User / Authorization |   6 | ✅ Pass   |
| Patients                         |     9 | ✅ Pass   |
| Vital Signs                      |    12 | ✅ Pass (1 fix)  |
| Medications                      |    13 | ✅ Pass (1 fix)  |
| Appointments                     |    15 | ✅ Pass   |
| **Total**                        | **65**| **65/65 Pass** |

Two issues were caught and fixed during testing: an invalid heart rate (`500`) initially returned `200 OK` instead of `400`, and a missing medication `StartDate` was silently defaulting to `0001-01-01T00:00:00` instead of being rejected. Both now correctly return `400 Bad Request`.

📄 **See [`Cardiac_Patient_Monitoring_API_Validation_Testing_Report.pdf`](./Cardiac_Patient_Monitoring_API_Validation_Testing_Report.pdf) for the full test report, including per-test-case evidence sections with screenshot placeholders.**

> **Note to self:** the biggest part of this project wasn't any one controller or technology — it was learning how the layers change each other. Authentication protects the request. DTOs control what data enters. Validation decides whether that data makes sense. Controllers handle the operation. EF Core talks to the database. Relationships keep the medical records honest. Testing verifies all of it actually behaves the way it's supposed to.

## 📌 Next steps

- Add cross-field business rules (e.g. appointment end time after start time) beyond single-property checks
- Extend automated xUnit/Moq coverage to match the full manual test matrix
- Add pagination to list endpoints (Patients, Vital Signs, Medications, Appointments) as data volume grows
- Revisit role-based authorization for finer-grained access (e.g. doctor vs. nurse vs. admin views)

---

<div align="center">

📄 **Full testing report:** [`Cardiac_Patient_Monitoring_API_Validation_Testing_Report.pdf`](./Cardiac_Patient_Monitoring_API_Validation_Testing_Report.pdf)

**Cardiac Patient Monitoring API** — a complete backend built during the BinX Backend Internship: authentication, database design, medical resources, validation, testing, and debugging.

*— end of project journal —*

</div>
