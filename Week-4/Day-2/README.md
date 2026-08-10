<div align="center">

# Day 2 — JWT Authentication & Token Issuance

*Field notes from the day the login endpoint stopped just saying "yes" or "no" and started handing back proof.*

![.NET](https://img.shields.io/badge/ASP.NET%20Core-JWT-512BD4?logo=dotnet&logoColor=white)
![Identity](https://img.shields.io/badge/ASP.NET%20Core-Identity-512BD4?logo=dotnet&logoColor=white)
![Postman](https://img.shields.io/badge/Tested%20with-Postman-FF6C37?logo=postman&logoColor=white)
![Status](https://img.shields.io/badge/status-complete-2ea44f)

`⏱ 8 hours` · `📄 Full report: Postman_Login_JWT_Testing_Report.pdf`

</div>

---

## 📌 Today in one sentence

Yesterday Identity proved a user is who they say they are. Today that proof got turned into something the client can actually carry around — a signed JWT — and the API learned to check that proof on every request instead of asking for a password again.

## 📌 Learning objectives

- Explain a JWT's structure and what claims represent
- Implement a login endpoint that issues a JWT on successful authentication
- Configure JWT bearer authentication middleware to validate incoming tokens

## 📌 What I learned

### 1. A JWT is three dot-separated parts, and only one of them is secret-ish
Header, payload, signature. The header says which algorithm signed it, the payload holds claims — statements about the user like their ID or roles — and the signature proves nothing was tampered with after issuance. The part that surprised me: the payload isn't encrypted, just signed. Anyone can paste a token into jwt.io and read it. So the claims are for *identifying* the user, never for hiding anything from them.

### 2. Issuing a token is just: verify, then sign
Login still starts exactly like Identity taught it to — `SignInManager.CheckPasswordSignInAsync` checks the credentials. The new part is what happens on success: instead of returning "ok", the endpoint builds a `JwtSecurityToken` out of a few claims, signs it with a secret key, and hands it back as a string.

```csharp
var claims = new[]
{
    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
    new Claim(ClaimTypes.Email, user.Email!),
};

var token = new JwtSecurityToken(
    issuer: config["Jwt:Issuer"],
    claims: claims,
    expires: DateTime.UtcNow.AddHours(1),
    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
```

From here on, the client attaches this token to every request instead of resending a password — the token *is* the login.

### 3. `[Authorize]` only works because the middleware does the checking first
Registering JWT bearer authentication in `Program.cs` tells the app what a "valid" token even means for this API: expected issuer, expected audience, the key to verify the signature against, and whether to enforce expiry. Once that's wired up, any endpoint marked `[Authorize]` never even sees a request until the token has already been validated — the controller code doesn't do any of that checking itself.

### 4. Short-lived tokens are a feature, not a limitation
A stolen token is only dangerous for as long as it's valid, so access tokens are deliberately short — 15 minutes to a few hours. Refresh tokens exist to soften that: a longer-lived, more carefully stored token used only to get a new access token without forcing the user to log in again. Full refresh-token support was flagged as a stretch task rather than something to build today, so it's parked for later, not skipped by accident.

> **Note to self:** the signing key is a secret exactly like a database password — never committed in plaintext. Local dev keeps it in a gitignored `appsettings.Development.json`; production keeps it in the host's secrets manager, not the repo.

## 📌 What I built — hands-on lab

- [x] Implemented `POST /api/Auth/login`, verifying credentials with `SignInManager` and returning `401` for a wrong password or a non-existent email
- [x] On successful login, built and returned a signed JWT containing the user's ID (`sub`) and email as claims, plus issuer, audience, and a short expiry
- [x] Configured JWT bearer authentication in `Program.cs` — issuer, audience, signing key, and lifetime validation all wired into `TokenValidationParameters`
- [x] Protected `GET /api/Tasks` with `[Authorize]` and confirmed it rejects requests with no token
- [x] Decoded the issued token at jwt.io and confirmed `sub`, `email`, `iss`, `aud`, and `exp` all matched what the code set
- [x] Set a 15-minute expiry and confirmed an expired token is rejected by the protected endpoint

**Tools:** ASP.NET Core Identity · System.IdentityModel.Tokens.Jwt · Postman · jwt.io · PowerShell

## 📌 Putting the endpoint through Postman

Yesterday's tests proved a user could be created. Today's had to prove something more layered — that login hands out a real token, that a bad login never gets one, and that the token is actually being checked, not just politely ignored.

| Test | Request | Result | What happened |
|---|---|---|---|
| **Login – Valid Credentials** | `POST /api/Auth/login` | ✅ `200 OK` | Correct email and password. `SignInManager` confirmed the credentials and a signed JWT came back with `sub`, `email`, `iss`, `aud`, and `exp` claims. |
| **Login – Wrong Password** | `POST /api/Auth/login` | ❌ `401 Unauthorized` | Right email, wrong password. Same generic "Invalid email or password" message as a non-existent email — no hint about which part was wrong. |
| **Login – Non-Existing Email** | `POST /api/Auth/login` | ❌ `401 Unauthorized` | An email that isn't registered at all. Identical response to the wrong-password case, so the API never leaks whether an account exists. |
| **Get Tasks – No Auth** | `GET /api/Tasks` | ❌ `401 Unauthorized` | No Bearer token attached. `[Authorize]` never let the request reach the controller. |
| **Get Tasks – Valid JWT** | `GET /api/Tasks` | ✅ `200 OK` | The token from the valid login, sent as a Bearer token. Signature, issuer, audience, and expiry all checked out, and the task list came back. |
| **Get Tasks – Expired JWT** | `GET /api/Tasks` | ❌ `401 Unauthorized` | An older token past its `exp` timestamp. Everything else about it was still valid — only the expiry check failed, and that was enough to block it. |

Both flows live in one Postman collection:

```
Task Tracker API v2
├── Authentication
    ├── Login - Valid Credentials
    ├── Login - Wrong Password
    └── Login - Non-Existing Email
    ├── Get Tasks - No Auth
    ├── Get Tasks - Valid JWT
    └── Get Tasks - Expired JWT
```

Between the six, both halves of the story are covered: a real login producing a token that actually works, and every way a request can fail to prove itself — wrong password, unknown email, missing token, expired token — getting caught before it touches anything protected.

---

<div align="center">

📄 **See [`Postman_Login_JWT_Testing_Report.pdf`](./Postman_Login_JWT_Testing_Report.pdf) for the full test report, including screenshots.**

*— end of Day 2*

</div>
