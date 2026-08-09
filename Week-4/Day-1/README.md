<div align="center">

# Day 1 — ASP.NET Core Identity & User Registration

*Field notes from the day I finally stopped being scared of auth.*

![.NET](https://img.shields.io/badge/ASP.NET%20Core-Identity-512BD4?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity%20Framework-Core-512BD4?logo=dotnet&logoColor=white)
![Postman](https://img.shields.io/badge/Tested%20with-Postman-FF6C37?logo=postman&logoColor=white)
![Status](https://img.shields.io/badge/status-complete-2ea44f)

`⏱ 8 hours` · `📄 Full report: Postman_API_Testing_Report.docx`

</div>

---

## 📌 Today in one sentence

Authentication — specifically, *not* building it myself. ASP.NET Core Identity handles the parts of a login system that are easiest to get wrong, and today was about understanding what it gives you for free, wiring it into a real project, and proving it actually works.

## 🎯 Learning objectives

- Explain what ASP.NET Core Identity provides out of the box
- Set up Identity with Entity Framework Core
- Implement a user registration endpoint

## 🧠 What I learned

### 1. It's a complete membership system, not just a users table
Storage, password hashing, roles, account confirmation — all of it ships out of the box, sitting on top of Entity Framework Core. The real value isn't convenience, it's that this code has already been picked apart by Microsoft and the entire .NET community. Rolling your own version means reinventing something security-critical, badly, alone.

### 2. Wiring it up is mostly one inheritance change
Extend the existing `DbContext` to inherit from `IdentityDbContext` and the full Identity schema — Users, Roles, UserRoles, and a few supporting tables — comes along for the ride. One migration later, it's sitting next to whatever tables were already there.

```csharp
public class AppDbContext : IdentityDbContext
{
    public DbSet<Order> Orders => Set<Order>();
}

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();
```

### 3. Registration is mostly plumbing, not logic
`UserManager.CreateAsync` does the actual work — hashing the password, saving the user — in one call. Writing the endpoint is really about validating what comes in and turning the result back into the right response: success, or a clear list of what went wrong.

### 4. The hashing story is more thoughtful than I expected
Identity hashes passwords with **PBKDF2** by default — deliberately slow, and salted per user, so a leaked database can't be cracked with a rainbow table in one pass. Two users with the same password end up with completely different stored hashes.

> **Note to self:** Don't ever write custom password hashing. Not "don't unless you have a good reason" — just don't. Identity's version has been battle-tested by a much bigger crowd than will ever review my code.

## 🛠️ What I built — hands-on lab

- [x] Added the Identity NuGet packages and extended the `DbContext` to inherit from `IdentityDbContext`
- [x] Ran a migration to add the Identity schema to the database, then applied it
- [x] Registered Identity services in `Program.cs` with `IdentityUser` and `IdentityRole`
- [x] Implemented a registration endpoint using `UserManager.CreateAsync`, with meaningful errors for bad input
- [x] Tested registration in Postman — once with a valid request, once with a deliberately weak password

**Tools:** ASP.NET Core Identity · Entity Framework Core · Postman

## 🧪 Putting the endpoint through Postman

With the endpoint built, the last step was proving it actually behaves — both when everything is correct and when it isn't.

| Test | Request | Result | What happened |
|---|---|---|---|
| **Register – Valid User** | `POST /api/Auth/register` | ✅ `200 OK` | Sent a valid username, email, and a strong password. The user was created successfully through `UserManager.CreateAsync`. |
| **Register – Weak Password** | `POST /api/Auth/register` | ❌ `400 Bad Request` | Same endpoint, same shape of request, but with a deliberately weak password. Identity rejected it and returned clear validation errors. |

Both requests live in one Postman collection:

```
Task Tracker API - Week 4 Day 1
└── Authentication
    ├── Register - Valid User
    └── Register - Weak Password
```

Between the two, both sides of registration are covered: a real user getting created, and a bad password getting caught before it ever reaches the database.

---

<div align="center">

📄 **See [`Postman_API_Testing_Report.docx`](./Postman_API_Testing_Report.docx) for the full test report, including screenshots.**

*— end of Day 1*

</div>
