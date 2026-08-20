<div align="center">

# Day 5 — Securing the API: Rate Limiting, CORS & Security Headers

*Field notes from the day the API stopped worrying only about who's asking and what they're asking for, and started worrying about how often, from where, and over what kind of connection.*

![.NET](https://img.shields.io/badge/ASP.NET%20Core-Security%20Hardening-512BD4?logo=dotnet&logoColor=white)
![Rate Limiting](https://img.shields.io/badge/Middleware-Rate%20Limiting-orange)
![CORS](https://img.shields.io/badge/Browser-CORS-blue)
![HTTPS](https://img.shields.io/badge/Transport-HTTPS%20%2F%20HSTS-2ea44f)
![Status](https://img.shields.io/badge/status-complete-2ea44f)

`⏱ 8 hours` · `🔎 No Postman testing today — configuration & review day`

</div>

---

## 📌 Today in one sentence

Days 2–4 built up who you are, what you're allowed to do, and whether what you sent makes sense. Day 5 stepped back from the request itself and hardened everything *around* it — how many requests are too many, which origins get to ask at all, whether the connection carrying the request can be trusted, and whether a query can ever be tricked into running someone else's SQL.

## 📌 Learning objectives

- Configure rate limiting to protect against brute-force and denial-of-service patterns
- Configure CORS correctly for the API's real intended consumers
- Apply security headers and explain how EF Core prevents SQL injection by default

## 📌 What I learned

### 1. Rate limiting protects the endpoints that get hit hardest when something's wrong
Capping how many requests a client can make in a given window doesn't just guard against generic abuse — it specifically slows down brute-force login attempts, where the whole attack depends on firing many attempts quickly. The login endpoint earns a stricter limit than the rest of the API for exactly that reason: repeated rapid hits there are one of the clearest signs an attack is in progress, in a way that repeated hits on a read-only endpoint usually aren't.

### 2. CORS is about which *websites* get to call the API from a browser — not about locking out real clients
The permissive version — allow any origin — is the kind of thing that's fine for a first local test and dangerous the moment it ships, because it lets any site's script call the API using a logged-in user's own session. A named policy that only allows the known frontend origin is the actual production answer:
```csharp
builder.Services.AddCors(options =>
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("https://myapp.com")
              .AllowAnyHeader()
              .AllowAnyMethod()));
```
The part worth remembering: CORS is a browser-enforced rule, not a server-side wall — it stops a *browser* from letting a foreign page's script read the response, not a direct API call from something like Postman or a server. That's exactly why it has to be paired with the other layers, not treated as a substitute for them.

### 3. HTTPS, HSTS, and security headers each close a different, specific hole
- `UseHttpsRedirection()` forces every request onto an encrypted connection instead of allowing plain HTTP.
- HSTS goes a step further — once a browser has seen it, that browser won't even *try* HTTP for the domain again, closing the gap where the very first request could otherwise be intercepted.
- `X-Content-Type-Options` stops the browser from guessing a file's type as something more dangerous than what the server declared (content-type sniffing).
- `X-Frame-Options` stops the page from being loaded inside someone else's frame (clickjacking).
- `Referrer-Policy` limits what leaks to the next site when a user navigates away.
- `Content-Security-Policy` restricts what scripts and content the page is even allowed to execute or load.

None of these take more than a few lines to configure — that's what made the framing land: they're baseline hardening, not optional extras, and a surprising share of real incidents trace back to exactly these few lines being skipped.

### 4. EF Core is safe from SQL injection by default — but "by default" has one specific exception
Ordinary LINQ — `Where()`, `AnyAsync()`, `FirstOrDefaultAsync()`, `FindAsync()` — is safe because EF Core parameterizes every value automatically; user input is never concatenated straight into a SQL string, which is the actual mechanism that makes injection possible in the first place. The one place that protection can be undone is raw SQL built with string interpolation, e.g. `FromSqlRaw($"...{userInput}...")`. `FromSqlInterpolated()` or explicit parameters keep the same safety raw SQL would otherwise throw away.

> **Note to self:** Authentication answers *who*. Authorization answers *what they can do*. Validation answers *whether their input makes sense*. Today's layer answers something different again — *how much, from where, and over what kind of connection* — and it's the layer that's easiest to forget because none of it shows up as a feature; it only shows up as an incident when it's missing.

## 📌 The four hardening layers — what each one stops

| Layer | Stops |
|---|---|
| **Rate Limiting** | Brute-force login attempts and simple denial-of-service patterns |
| **CORS (named policy)** | Untrusted browser-based origins calling the API on a user's behalf |
| **HTTPS / HSTS** | Interception or downgrade of traffic to an unencrypted connection |
| **Security Headers** | Content-type sniffing, clickjacking, referrer leakage, unwanted script execution |
| **EF Core parameterization** | SQL injection via LINQ — bypassed only by unparameterized raw SQL |

## 📌 What I built — hands-on lab

- [x] Configured rate limiting with a stricter limit on the login endpoint than on general endpoints
- [x] Configured a named CORS policy (`AllowFrontend`) allowing only the API's specific frontend origin
- [x] Confirmed a disallowed origin is rejected by the CORS policy
- [x] Enabled HTTPS redirection and HSTS in the middleware pipeline
- [x] Added `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy`, and `Content-Security-Policy` headers
- [x] Reviewed the codebase for raw SQL queries; confirmed none use unparameterized string interpolation

**Tools:** ASP.NET Core Rate Limiting Middleware · CORS Middleware · `UseHttpsRedirection()` · HSTS · Security Header Middleware · Entity Framework Core (LINQ parameterization, `FromSqlInterpolated`)

## 📌 A note on testing today

Days 2–4 each closed with a Postman pass confirming status codes and response shapes against a running endpoint. Today's work is different in kind — it's middleware and pipeline configuration (rate-limit windows, an allowed-origins list, header values, HSTS settings) plus a manual code review for raw SQL usage, rather than behavior exposed through request/response pairs. That's why there's no Postman test table in today's report: the verification here was configuration review and manual origin/rate-limit checks, not endpoint-by-endpoint API calls.

## 📌 Next steps

- Add automated tests (or a small Postman collection) that exercise the rate limiter's actual threshold and the CORS-rejection path, so this layer gets the same regression coverage as Days 2–4
- Move HSTS and any environment-specific CORS origins into per-environment configuration before deploying beyond local development
- Revisit the Content-Security-Policy once the frontend's real script/style sources are finalized, rather than leaving it at a starter baseline

---

<div align="center">

*— end of Day 5 —*

</div>