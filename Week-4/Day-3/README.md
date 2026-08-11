<div align="center">

# Day 3 — Protecting Routes with Authorization & Role-Based Access Control

*Field notes from the day the API stopped trusting anyone who merely showed up with a valid token, and started asking what they were actually allowed to do.*

![.NET](https://img.shields.io/badge/ASP.NET%20Core-Authorization-512BD4?logo=dotnet&logoColor=white)
![Identity](https://img.shields.io/badge/ASP.NET%20Core-Identity-512BD4?logo=dotnet&logoColor=white)
![RBAC](https://img.shields.io/badge/Access%20Control-Role%20Based-blue)
![Postman](https://img.shields.io/badge/Tested%20with-Postman-FF6C37?logo=postman&logoColor=white)
![Status](https://img.shields.io/badge/status-complete-2ea44f)

`⏱ 8 hours` · `📄 Full report: Day3_Authorization_API_Testing_Report.pdf`

</div>

---

## 📌 Today in one sentence

Yesterday a valid JWT was enough to get in the door. Today the API learned that being *authenticated* and being *allowed* are two different questions — `[Authorize]` locked the door, roles decided who gets which room, and a first named policy hinted at a future where permissions, not job titles, do the deciding.

## 📌 Learning objectives

- Apply `[Authorize]` to protect endpoints from unauthenticated access
- Implement role-based access control with at least two roles
- Understand claims-based and policy-based authorization for finer-grained control

## 📌 What I learned

### 1. `[Authorize]` doesn't check anything itself — it just refuses to let unchecked requests through
Dropping `[Authorize]` on `TasksController` means no token, an expired token, or a bad signature never even reaches the action body — the middleware set up on Day 2 rejects it with `401 Unauthorized` first. `[AllowAnonymous]` is the escape hatch: it lets one action (a health check, say) opt back out of an otherwise-locked-down controller.

### 2. Roles are seeded, not requested
`User` and `Admin` exist as Identity roles created via `RoleManager<IdentityRole>` at startup, and the Admin account itself is seeded through `UserManager<User>` with credentials pulled from configuration — not from public registration. The part that surprised me: there's no "become admin" button anywhere, by design. If self-registration could ever produce an Admin, the role would be worthless as a security boundary.

### 3. Restricting an endpoint to a role is one attribute — but it changes the status code, not just the outcome
```csharp
[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id) { ... }
```
A User-role token hitting this doesn't get `401` — it gets `403 Forbidden`. That distinction is the whole point of today: `401` means "I don't know who you are," `403` means "I know exactly who you are, and the answer is no."

### 4. Policy-based authorization: the first step away from hardcoding role names everywhere
Instead of scattering `[Authorize(Roles = "Admin")]` across every controller that needs it, a named policy centralizes the rule:
```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageTasks", policy =>
    {
        policy.RequireClaim("Permission", "CanManageTasks");
    });
});
```
It isn't wired end-to-end yet — the `Permission` claim still needs to be attached to issued JWTs before `[Authorize(Policy = "CanManageTasks")]` means anything on an endpoint. But the shape of it is already more flexible than roles: a policy can check *any* claim, not just role membership.

### 5. Middleware order is not a formality
```csharp
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```
Authentication has to answer "who is this?" before authorization can answer "what can they do?" Get the order wrong and a protected endpoint stops failing cleanly — a missing token starts blowing up as a `500` instead of politely returning `401`, which is usually the first sign this line got moved.

> **Note to self:** `401` and `403` aren't interchangeable "no" responses — they're different failures at different stages, and mixing them up in a real API leaks information about *why* someone was denied, which is exactly the kind of detail you don't want to leak by accident.

## 📌 401 vs 403 — the core distinction

| Status | Meaning |
|---|---|
| **401 Unauthorized** | The request has no valid authentication (no/invalid/expired JWT). |
| **403 Forbidden** | The caller is authenticated but lacks the required role/permission. |

```
No JWT → Authentication fails → 401

Valid User JWT → Authenticated → Admin role required → not Admin → 403

Valid Admin JWT → Authenticated → Admin role present → 204 (or 200)
```

## 📌 What I built — hands-on lab

- [x] Added `[Authorize]` to the Week 3 CRUD controller; confirmed no-token requests return `401`
- [x] Created `User` and `Admin` roles; assigned them to two test users via `UserManager`
- [x] Restricted the Delete endpoint to `Admin` only; confirmed a User-role token gets `403`
- [x] Defined one named authorization policy beyond a simple role check; applied it to one endpoint
- [x] Set up a Postman environment that captures the login token and reuses it for protected requests

**Tools:** ASP.NET Core · ASP.NET Core Identity (`RoleManager`, `UserManager`) · JWT Bearer Authentication · Role-Based & Policy-Based Authorization · Entity Framework Core · SQL Server / LocalDB · Postman · Swagger

## 📌 Putting the endpoints through Postman

Day 2's tests proved a token could be issued and checked. Today's had to prove something more layered — that a valid token isn't automatically a valid *permission*, and that the API tells the difference between "I don't know you" and "I know you, and no."

| TC | Test Case | Endpoint | Auth | Expected |
|---|---|---|---|---|
| TC01 | Protected route without token | `GET /api/Tasks` | No Token | 401 |
| TC02 | Protected route with invalid token | `GET /api/Tasks` | Invalid JWT | 401 |
| TC03 | Protected route with User token | `GET /api/Tasks` | User JWT | 200 |
| TC04 | Protected route with Admin token | `GET /api/Tasks` | Admin JWT | 200 |
| TC05 | Admin endpoint without token | `DELETE /api/Users/{id}` | No Token | 401 |
| TC06 | Admin endpoint with User token | `DELETE /api/Users/{id}` | User JWT | 403 |
| TC07 | Admin endpoint with Admin token | `DELETE /api/Users/{id}` | Admin JWT | 204 |
| TC08 | Policy endpoint without token | Policy endpoint | No Token | 401 |
| TC09 | Policy endpoint with User token | Policy endpoint | User JWT | 403 |
| TC10 | Policy endpoint with authorized Admin | Policy endpoint | Admin JWT + Permission | Success |

**10/10 passed.** Every combination of "who are you" and "what are you allowed to do" landed on the status code it should have — nothing leaked through on a technicality, and nothing correctly-permitted got blocked by accident.

## 📌 Next steps

- Attach the `Permission` claim to issued JWTs so the `CanManageTasks` policy can be exercised end-to-end
- Add further granular policies (e.g. per-tenant, per-resource-owner checks)
- Expand Postman coverage as more policy-protected endpoints are added

---

<div align="center">

📄 **See [`Day3_Authorization_API_Testing_Report.pdf`](./Day3_Authorization_API_Testing_Report.pdf
) for the full test report.**

*— end of Day 3*

</div>
