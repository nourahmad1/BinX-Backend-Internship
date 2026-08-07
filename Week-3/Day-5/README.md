
# Task Tracker API — Day 5: Testing & Documenting the API with Postman

## Overview

Day 5 closes out Week 3 by turning the ad-hoc Postman requests from Day 4 into a **complete, organized, and documented test suite** for the Task Tracker API. The focus shifted from just calling endpoints to proving they behave correctly — on both the happy path and the error path — and packaging that proof into a portable, shareable collection plus a written report.

This day covered:

* Building a properly organized Postman collection
* Testing success **and** error paths for every endpoint
* Postman environments and variables (`{{baseUrl}}`)
* Producing a full testing report with evidence (linked in the GitHub repo)

---

# Learning Objectives — Status

| Objective | Status |
|---|---|
| Build a complete, organized Postman collection covering every endpoint | ✅ Done |
| Test both success and error paths systematically | ✅ Done |
| Use Postman environments and variables to keep the collection portable | ✅ Done |
| Document the API | ✅ Done |

---

# 1. Building the Postman Collection

All requests from Day 4 were reorganized into a single named collection, split into folders that mirror the API's resource structure:

```text
Task Tracker API Version2
├── Users
│   ├── TC-001 | Get All Users                      (GET)
│   ├── TC-002 | GET User By Id - Existing User      (GET)
│   ├── TC-003 | GET User By Id - Not Found          (GET)
│   ├── TC-004 | Create User - Valid Data            (POST)
│   ├── TC-005 | Create User - Invalid Data          (POST)
│   ├── TC-006 | Update User - Existing User         (PUT)
│   ├── TC-007 | Update User - Not Found             (PUT)
│   ├── TC-008 | Delete User - Existing User         (DEL)
│   └── TC-009 | Delete User - Not Found             (DEL)
│
└── Tasks
    ├── TC-010 | Get All Tasks                       (GET)
    ├── TC-011 | Get Task By Id - Existing Task       (GET)
    ├── TC-012 | Get Task By Id - Not Found          (GET)
    ├── TC-013 | Create Task - Valid Data            (POST)
    ├── TC-014 | Create Task - Invalid Data          (POST)
    ├── TC-015 | Update Task - Existing Task         (PUT)
    ├── TC-016 | Update Task - Not Found             (PUT)
    ├── TC-017 | Delete Task - Existing Task         (DEL)
    └── TC-018 | Delete Task - Not Found             (DEL)
```

> Request names follow the `TC-### | <Action> - <Scenario>` convention directly inside the collection, so each request's purpose is visible without opening it.

Every request is **saved**, not just tested ad hoc, so the whole collection can be re-run or shared with a teammate. This matches the collection format required by the Week 9 professional baseline (minimum one test per endpoint), so building this habit now avoids a scramble later.

---

# 2. Testing Success and Error Paths

Each endpoint has at least two requests:

* A **happy path** request — valid input, expected success response.
* A **failure path** request — invalid ID, missing/invalid required field, or a non-existent resource — expected error response.

| Scenario Type | Example | Expected Result |
|---|---|---|
| Happy path | `GET /api/users/1` | `200 OK` |
| Not found | `GET /api/users/99` | `404 Not Found` |
| Happy path | `POST /api/tasks` (valid body) | `201 Created` |
| Invalid input | `POST /api/tasks` (invalid `userId`) | `400 Bad Request` |

## Automated Test Scripts

Postman test scripts (JavaScript run automatically after each request) were added to assert the expected status code, turning manual click-through checks into repeatable, automated ones:

```javascript
// Postman test script example
pm.test("Status code is 201", () => {
    pm.response.to.have.status(201);
});

pm.test("Response has an id", () => {
    pm.expect(pm.response.json()).to.have.property("id");
});
```

At least 3 requests in the collection include assertions like these, so a full collection run reports pass/fail automatically instead of requiring a manual look at each response body.

---

# 3. Postman Environments and Variables

A Postman **environment** was created to store the API's base URL as a variable instead of hardcoding it into every request:

```text
{{baseUrl}}/api/users
{{baseUrl}}/api/tasks
```

All requests in both folders were updated to use `{{baseUrl}}`. Switching environments (local → staging → production) now only requires updating one variable value, instead of editing every request URL by hand. This becomes essential once the API is deployed in Week 9 and needs to be tested against a real, live URL alongside local development.

| Environment | `baseUrl` value |
|---|---|
| Local | `https://localhost:{port}` |
| Staging | *(to be added in Week 9)* |
| Production | *(to be added in Week 9)* |

---

# 4. Documenting the API

Beyond the Swagger UI generated automatically by ASP.NET Core, this README documents each endpoint, its purpose, required fields, and possible error responses — enough for a teammate (or a future version of the developer) to use the API without reading the source code.

## Users Endpoints

| Method | Endpoint | Description | Required Fields | Possible Errors |
|---|---|---|---|---|
| GET | `/api/users` | Get all users | — | — |
| GET | `/api/users/{id}` | Get user by ID | — | `404 Not Found` |
| POST | `/api/users` | Create user | Name, Email | `400 Bad Request` |
| PUT | `/api/users/{id}` | Update user | Name, Email | `400 Bad Request`, `404 Not Found` |
| DELETE | `/api/users/{id}` | Delete user | — | `404 Not Found` |

## Tasks Endpoints

| Method | Endpoint | Description | Required Fields | Possible Errors |
|---|---|---|---|---|
| GET | `/api/tasks` | Get all tasks | — | — |
| GET | `/api/tasks/{id}` | Get task by ID | — | `404 Not Found` |
| POST | `/api/tasks` | Create task | Title, UserId | `400 Bad Request` |
| PUT | `/api/tasks/{id}` | Update task | Title | `400 Bad Request`, `404 Not Found` |
| DELETE | `/api/tasks/{id}` | Delete task | — | `404 Not Found` |

---

# 5. Full Testing Report

All 18 test cases (9 for Users, 9 for Tasks) — including the happy path, error path, and full collection run through the Postman Collection Runner — are documented with screenshot evidence in the full testing report, available in this repository.

📄 **Full report:** [`API_Testing_Report_Postman.pdf`](./API_Testing_Report_Postman.pdf)

The report includes:

* Swagger documentation review
* `{{baseUrl}}` environment variable configuration
* Postman collection structure (Users folder + Tasks folder)
* Postman Collection Runner summary (full collection run)
* Test case tables for every endpoint (Users & Tasks) with expected results
* Test summary and conclusion

---

# Hands-On Lab — Completion Checklist

* [x] Organized all of Day 4's requests into a properly named Postman collection with folders per resource
* [x] Added at least one error-path request per endpoint (not found, invalid input) alongside the happy path
* [x] Added test scripts asserting the expected status code for at least 3 requests
* [x] Created a Postman environment with a `baseUrl` variable and updated all requests to use it

---

# Key Takeaway

A Postman collection that only ever tests happy paths gives a false sense of confidence. Error-path tests are what actually catch the bugs that show up when a real client sends something unexpected — which is why every endpoint above ships with at least one failure-path request, not just a success case.

---

# Tools Used

| Tool | Purpose |
|---|---|
| Postman | Building, testing, and organizing API requests |
| Postman Collection Runner | Running the full collection and reporting pass/fail |
| Postman Environments | Managing the `{{baseUrl}}` variable across environments |
| Swagger | Reviewing endpoints before testing |
| ASP.NET Core (.NET 10) | The API under test |

---


* Expand the collection to meet the Week 9 professional baseline (minimum one test per endpoint, already satisfied here).
* Ensure documentation passes the Week 9 Definition of Done audit.
