<div align="center">

# Day 4 — Input Validation with FluentValidation

*Field notes from the day the API stopped trusting that a request even meant what it said, and started checking whether the values inside it made any sense.*

![.NET](https://img.shields.io/badge/ASP.NET%20Core-Validation-512BD4?logo=dotnet&logoColor=white)
![FluentValidation](https://img.shields.io/badge/FluentValidation-Rules-orange)
![Postman](https://img.shields.io/badge/Tested%20with-Postman-FF6C37?logo=postman&logoColor=white)
![Status](https://img.shields.io/badge/status-complete-2ea44f)

`⏱ 8 hours` · `📄 Full report: Day4_Validation_Testing_Report.pdf`

</div>

---

## 📌 Today in one sentence

Yesterday the API learned who was allowed to do what. Today it learned that "allowed" isn't the same as "sensible" — a request can carry a valid token, hit the right role check, and still be garbage, and `[Required]` alone was never going to catch a title that's 500 characters of whitespace.

## 📌 Learning objectives

- Compare DataAnnotations and FluentValidation and choose appropriately
- Write validators expressing real business rules, not just data types
- Return clear, structured validation error responses

## 📌 What I learned

### 1. DataAnnotations answers "is this the right shape?" — FluentValidation answers "does this make sense?"
`[Required]` and `[MaxLength(100)]` sit right on the model and handle the simple cases fast. They stop being enough the moment a rule needs to compare two properties, check against something outside the model, or express a genuine business constraint — a discount that has to fall between 0 and 100, an end date that has to land after a start date. FluentValidation moves that logic into its own class instead of bolting it onto the data model, and that separation is the whole reason this program treats it as the standard past the trivial cases.

### 2. A validator reads like the rule it's enforcing
```csharp
public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.Items).NotEmpty()
            .WithMessage("An order must contain at least one item.");
    }
}
```
The part that stuck: a rule that only checks "is this present" is doing half the job. `CustomerId must be greater than 0` catches real bugs that `CustomerId must not be null` walks straight past — a `0` or a `-1` is very much "present."

### 3. Registering the validator means invalid requests never reach the controller at all
Once FluentValidation's ASP.NET Core integration is wired into the pipeline, validation runs automatically during model binding — before the action body ever executes. A bad payload gets a `400` handed back on its own; the controller and service layers stay focused on business logic instead of re-checking nulls and ranges on every endpoint.

### 4. The error shape matters as much as the status code
A client can't do much with a bare "invalid request" string. `ValidationProblemDetails` — the format FluentValidation's integration produces automatically — names the field and the exact rule that failed, in a shape a client can parse and place next to the relevant form field:
```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": [
      "Title cannot exceed 200 characters."
    ]
  }
}
```

> **Note to self:** Day 3 was about the API knowing *who* you are. Day 4 is about it knowing *what you're actually sending it* — and both matter for the same reason: neither authentication nor a well-shaped model guarantees the request is trustworthy.

## 📌 DataAnnotations vs FluentValidation — the core distinction

| Approach | Best for |
|---|---|
| **DataAnnotations** | Simple, single-property checks (`[Required]`, `[MaxLength]`) attached directly to the model. |
| **FluentValidation** | Business rules — cross-property checks, conditional logic, anything beyond "is this field present and the right type." |

## 📌 What I built — hands-on lab

- [x] Installed FluentValidation and its ASP.NET Core integration package
- [x] Wrote `TaskCreateDtoValidator`, covering at least 3 real business rules
- [x] Wrote `TaskUpdateDtoValidator`
- [x] Registered the validators; confirmed invalid requests now return a structured `400` automatically
- [x] Tested each validation rule individually in Postman, confirming the specific error message returned

**Tools:** FluentValidation · FluentValidation.AspNetCore integration · ASP.NET Core Model Binding · `ValidationProblemDetails` · Postman

## 📌 Putting the validators through Postman

Day 3's tests proved the API could tell *who* was asking. Today's had to prove it could also tell *whether what they sent made sense* — before any of that reached controller logic.

| TC | Test Case | Endpoint | Rule Violated | Expected |
|---|---|---|---|---|
| TC-01 | Create Task — Empty Title | `POST /api/Tasks` | Title required | 400 |
| TC-02 | Create Task — Title Contains Only Spaces | `POST /api/Tasks` | Title not whitespace | 400 |
| TC-03 | Create Task — Title Exceeds Maximum Length | `POST /api/Tasks` | Title ≤ 200 chars | 400 |
| TC-04 | Create Task — Invalid UserId | `POST /api/Tasks` | UserId valid/exists | 400 |
| TC-05 | Update Task — Empty Title | `PUT /api/Tasks/{id}` | Title required | 400 |
| TC-06 | Update Task — Title Contains Only Spaces | `PUT /api/Tasks/{id}` | Title not whitespace | 400 |
| TC-07 | Update Task — Title Exceeds Maximum Length | `PUT /api/Tasks/{id}` | Title ≤ 200 chars | 400 |

**7/7 passed.** Every invalid payload was stopped at the door with a `400` and a structured, field-level message — nothing invalid slipped through to controller logic on a technicality.

## 📌 Next steps

- Add cross-field business rules (e.g. an end date that must be after a start date) beyond single-property checks
- Extend validators to cover any remaining Create/Update DTOs in the API
- Expand Postman coverage as more validators are added

---

<div align="center">

📄 **See [`Day4_Validation_Testing_Report.pdf`](./Day4_Validation_Testing_Report.pdf) for the full test report.**

*— end of Day 4*

</div>
