# Demo Script

This script describes the full review flow after all MVP phases are complete.
It covers both the backend Swagger demo path and the Angular frontend demo path.

---

## Before The Demo

Prepare:

- Docker running with SQL Server started (`docker compose up -d`)
- Database migrated (`dotnet tool run dotnet-ef database update ...`)
- API running locally at `http://localhost:5188`
- Angular frontend running locally at `http://localhost:4200`
- Browser open with two tabs: Swagger and the Angular app
- Seeded mock users available (Handler, Supervisor, Manager)

### Start Commands

Start the backend:

```bash
docker compose up -d
dotnet tool restore
dotnet tool run dotnet-ef database update --project src/ClaimsModule.Persistence --startup-project src/ClaimsModule.API
dotnet run --project src/ClaimsModule.API/ClaimsModule.API.csproj --urls http://localhost:5188
```

Start the frontend:

```bash
cd frontend/claims-module-web
npm start
```

Open:

- Angular app: `http://localhost:4200`
- Swagger: `http://localhost:5188/swagger`
- Hangfire: `http://localhost:5188/hangfire`

### Seeded Demo IDs

| Role | User ID |
|---|---|
| Handler | `bbbbbbbb-0000-0000-0000-000000000001` |
| Supervisor | `bbbbbbbb-0000-0000-0000-000000000002` |
| Manager | `bbbbbbbb-0000-0000-0000-000000000003` |

| Resource | ID |
|---|---|
| Auto policy | `11111111-1111-1111-1111-111111111111` |
| Collision cause of loss | `aaaaaaaa-0000-0000-0000-000000000001` |

---

## Part 1: Architecture Overview (2–3 minutes)

Before opening anything, briefly describe the project structure.

What to say:

> "This is a backend-first Claims Management System built with Clean Architecture.
> The backend uses ASP.NET Core Web API, EF Core, SQL Server, MediatR, FluentValidation,
> and Hangfire. The frontend is Angular with Angular Material.
> The primary demo surface is Swagger for the backend, with the Angular app showing
> the same workflow in a browser UI."

Point to the solution layout:

- `ClaimsModule.Domain` — entities, enums, no external dependencies
- `ClaimsModule.Application` — MediatR commands/queries, FluentValidation, interfaces
- `ClaimsModule.Persistence` — EF Core DbContext, configurations, migrations, seed data
- `ClaimsModule.Infrastructure` — Hangfire background jobs
- `ClaimsModule.API` — thin ASP.NET Core controllers, startup configuration

What to explain:

- Each layer depends only on inner layers. The Domain knows nothing about EF Core.
- Controllers are thin: they receive an HTTP request and dispatch a MediatR command or query.
- Business rules live in Application handlers, not controllers.
- The `Result<T>` pattern is used to avoid exception-driven control flow.

---

## Part 2: Backend Demo Through Swagger (8–10 minutes)

Open `http://localhost:5188/swagger`.

### Step 1: Look Up Policies And Cause-Of-Loss Codes

Run `GET /api/policies` and `GET /api/cause-of-loss-codes`.

What to explain:

> "These are seeded lookup tables. In a real system, policies would come from a
> policy administration system. Here they are seeded for demo purposes."

### Step 2: Create A Claim (FNOL)

Run `POST /api/claims` with the sample body from `README.md`.

```json
{
  "policyId": "11111111-1111-1111-1111-111111111111",
  "causeOfLossCodeId": "aaaaaaaa-0000-0000-0000-000000000001",
  "lossDate": "2026-07-06",
  "description": "Demo collision claim created through FNOL",
  "createdByUserId": "bbbbbbbb-0000-0000-0000-000000000001",
  "parties": [
    {
      "fullName": "Demo Claimant",
      "partyType": "Claimant",
      "email": "demo.claimant@example.test",
      "phone": "+380000000000",
      "notes": "Demo claimant for Swagger testing"
    }
  ],
  "riskObjects": [
    {
      "objectType": "Vehicle",
      "externalReference": "VIN-DEMO-001",
      "description": "Demo insured vehicle"
    }
  ]
}
```

What to explain:

> "FNOL stands for First Notice of Loss. It is the first step in the claim lifecycle.
> The request goes through a MediatR command, a FluentValidation validator, and then
> the handler persists the claim and writes a `ClaimCreated` audit log entry.
> The claim starts with status `Open`."

Note the returned `claimId` and `claimNumber` for the next steps.

### Step 3: View Claim Detail

Run `GET /api/claims/{id}` with the returned claim ID.

What to explain:

> "The detail response includes the claim header, all parties, all risk objects,
> all reserves, and the full audit log. The audit log is append-only.
> You can already see the `ClaimCreated` entry."

### Step 4: View Audit Log

Point to the `auditLogEntries` array in the response.

What to explain:

> "Every important action writes an audit entry with a timestamp and the actor user ID.
> This is how the system maintains traceability. Audit entries are never deleted."

### Step 5: Transition Claim Status

Run `PATCH /api/claims/{id}/status`:

```json
{
  "newStatus": "UnderInvestigation",
  "actorUserId": "bbbbbbbb-0000-0000-0000-000000000001"
}
```

What to explain:

> "Status transitions are controlled. Only explicitly allowed pairs succeed.
> Any other transition returns `422 Unprocessable Entity`.
> The handler checks a transition table, writes a `ClaimStatusChanged` audit entry,
> and saves."

Then try an invalid transition to demonstrate the 422 response:

```json
{
  "newStatus": "Closed",
  "actorUserId": "bbbbbbbb-0000-0000-0000-000000000001"
}
```

### Step 6: Create A Small Reserve (Auto-Approved)

Run `POST /api/claims/{claimId}/reserves`:

```json
{
  "amount": 5000,
  "currency": "USD",
  "createdByUserId": "bbbbbbbb-0000-0000-0000-000000000001"
}
```

What to explain:

> "Reserves below 10,000 USD are automatically approved. This is a simplified
> business rule for the MVP. The response already shows `status: Approved`.
> A Hangfire job is enqueued in the background to simulate GL posting."

### Step 7: Create A Large Reserve (Pending Approval)

Run `POST /api/claims/{claimId}/reserves`:

```json
{
  "amount": 15000,
  "currency": "USD",
  "createdByUserId": "bbbbbbbb-0000-0000-0000-000000000001"
}
```

Note the returned `reserveId`.

What to explain:

> "Reserves above 10,000 USD require manual approval by a supervisor or manager.
> The status is `PendingApproval`. This represents the segregation-of-duties rule."

### Step 8: Try Self-Approval (Blocked)

Run `PATCH /api/claims/{claimId}/reserves/{reserveId}/approve`:

```json
{
  "actorUserId": "bbbbbbbb-0000-0000-0000-000000000001"
}
```

Expected: `422 Unprocessable Entity` — "Users cannot approve their own reserve."

What to explain:

> "Self-approval is explicitly blocked. The handler checks whether the approving
> user is the same as the reserve creator and rejects the request."

### Step 9: Try Handler Approval (Forbidden)

Try to approve with the Handler role:

```json
{
  "actorUserId": "bbbbbbbb-0000-0000-0000-000000000001"
}
```

(Same user, so 422 self-approval. To see the 403, create a new reserve with a
Supervisor user ID and then try to approve it with the Handler.)

What to explain:

> "Only Supervisors and Managers can approve or reject pending reserves.
> Handlers receive 403 Forbidden."

### Step 10: Approve As Supervisor

Run `PATCH /api/claims/{claimId}/reserves/{reserveId}/approve`:

```json
{
  "actorUserId": "bbbbbbbb-0000-0000-0000-000000000002"
}
```

What to explain:

> "The reserve is now Approved. A Hangfire background job is enqueued to simulate
> posting to a General Ledger system. The job is idempotent: if it runs twice,
> only one `ReserveGlPosted` audit entry is created."

### Step 11: Show Hangfire Dashboard

Open `http://localhost:5188/hangfire`.

What to explain:

> "Hangfire manages background job processing. In a real system this would be used
> for things like notifications, integrations, or scheduled SLA checks.
> Here it simulates GL posting after reserve approval.
> The dashboard is available only in Development and is unauthenticated for demo convenience."

### Step 12: Show GL Posting Result

Run `GET /api/claims/{id}` again after a few seconds.

Point to the approved reserve — it now shows `glPostedAtUtc` and `glPostingReference`.
The audit log now contains a `ReserveGlPosted` entry.

What to explain:

> "The Hangfire job wrote the GL posting reference and timestamp directly on the reserve,
> and added an audit entry. This is the complete approve-and-post flow."

---

## Part 3: Angular Frontend Demo (5–8 minutes)

Open `http://localhost:4200`.

### Step 1: Claims List

Navigate to `http://localhost:4200/claims`.

What to show:

- The list loads real claims from `GET /api/claims`.
- The status column shows the current claim status.
- Filters above the list let you narrow the view by status or search text.
- The list updates reactively as you type.

What to explain:

> "The Angular frontend talks directly to the .NET API.
> There is no mock data in the frontend — every screen shows live backend data."

### Step 2: View A Claim Detail

Click the view/detail button on any existing claim.

What to show:

- The detail loads from `GET /api/claims/{id}`.
- Tabs: **Overview**, **Parties**, **Risk Objects**, **Reserves**, **Audit Log**.
- The Overview tab shows the claim header, status, policy, and loss details.
- The Parties tab shows all linked parties.
- The Risk Objects tab shows all linked risk objects.
- The Reserves tab shows all reserves with their status and GL fields.
- The Audit Log tab shows the full append-only action history.

### Step 3: Create A New Claim (FNOL Form)

Click **Log New Claim** in the navigation.

What to show:

- Policy dropdown populated from `GET /api/policies`.
- Cause of loss dropdown populated from `GET /api/cause-of-loss-codes`.
- Loss date picker.
- Description field.
- Add party button — fill in Full Name, type, email, phone.
- Add risk object button — fill in type, reference, description.
- Submit creates the claim via `POST /api/claims`.
- A success snackbar appears briefly.
- The page redirects to `/claims/{newClaimId}`.

What to explain:

> "The FNOL form validates required fields before submitting.
> After the claim is created the user is redirected to the claim detail screen
> so they can immediately start working on the new claim."

### Step 4: Add A Small Reserve

On the claim detail Reserves tab, add a reserve of 5,000 USD.

What to show:

- The reserve appears in the list with status **Approved**.
- No approval action needed.

What to explain:

> "Small reserves are auto-approved immediately.
> The Hangfire GL posting job runs in the background after a few seconds."

### Step 5: Add A Large Reserve

Add a reserve of 15,000 USD.

What to show:

- The reserve appears with status **PendingApproval**.
- As the current Handler user, no approve or reject buttons appear.

What to explain:

> "Large reserves wait for approval from a Supervisor or Manager.
> The Handler role cannot see the approval controls — this enforces the segregation rule."

### Step 6: Switch Demo User And Approve

Click the **demo user selector** in the top bar and switch to **Supervisor**.

What to show:

- The user context updates to Supervisor.
- The pending reserve now shows **Approve** and **Reject** buttons.
- Click Approve.
- The reserve status updates to **Approved**.
- After a few seconds, refresh to see the GL posting fields.

What to explain:

> "The user selector is a mock authentication stand-in for the demo.
> In a real system this would be a JWT token or session. The frontend sends
> the selected user ID in a request header and the backend resolves the role
> from the mock users table."

### Step 7: Status Transition

On an Open claim, use the **Move to Under Investigation** action.

What to show:

- The status updates from Open to Under Investigation.
- The Audit Log tab gains a new `ClaimStatusChanged` entry.

What to explain:

> "The frontend exposes only the `Open → Under Investigation` transition in the UI.
> The backend supports a wider set of transitions and enforces them at the API level.
> The frontend restriction is an intentional scope decision for the MVP."

---

## Part 4: Architecture Talking Points

Use these when the reviewer asks about technical choices.

### Clean Architecture

> "The solution is split into five projects that follow the dependency rule:
> outer layers depend on inner layers, never the reverse.
> The Domain has no external dependencies. The Application layer defines interfaces
> that the outer layers implement. This makes the business rules easy to find and test
> independently of the database or framework."

### MediatR / CQRS

> "Every HTTP request dispatches a MediatR command or query. The controller itself
> contains almost no logic. This keeps request handling focused: one handler per use case,
> one validator per command. The ValidationBehavior pipeline runs FluentValidation
> automatically before any handler executes."

### FluentValidation

> "All input validation is in explicit validator classes. FluentValidation rules are easy
> to read and extend. Business rule violations — such as self-approval or an invalid
> status transition — are returned from the handler as domain errors, not validation errors."

### EF Core + SQL Server

> "EF Core maps entities through Fluent API configuration files, one per entity.
> This keeps the configuration explicit and avoids attribute-cluttered entity classes.
> Seed data uses stable GUIDs so demos and tests are reproducible.
> The audit log uses restrictive delete behavior so history cannot be accidentally removed."

### Hangfire GL Posting Simulation

> "When a reserve is approved, a Hangfire fire-and-forget job is enqueued.
> The job simulates posting to a GL system by writing a reference number and timestamp
> on the reserve and adding an audit entry. The job is idempotent: a `GlPostedAtUtc`
> check prevents duplicate audit entries if the job runs more than once."

### Audit Log

> "Every important business action writes an immutable audit log entry with the action name,
> actor user ID, timestamp, and a description. The entries are never updated or deleted.
> This gives the system full traceability for claim and reserve history."

### Mock Users Instead Of Real Auth

> "The MVP uses three seeded mock users: Handler, Supervisor, and Manager.
> The frontend sends the selected user's ID in a request header.
> The backend resolves the role from the mock users table.
> This is enough to demonstrate role-sensitive behavior — reserve approval rules,
> self-approval prevention, and role restrictions — without the complexity of a full
> authentication system."

---

## Part 5: How To Explain Missing Items

Use these responses when the reviewer asks about items not implemented.

### Azure Deployment Not Implemented

> "Deployment was deliberately kept out of scope. The assessment value is in the
> backend design and business-rule implementation, not infrastructure configuration.
> The local Docker setup with SQL Server is enough to run and verify the complete
> business flow. Deployment would be a straightforward next step using Azure App Service
> or Azure Container Apps with environment-specific configuration."

### CI/CD Not Implemented

> "There is no automated CI/CD pipeline. Build and test commands are documented in
> `README.md` and verified manually before each commit. Adding a GitHub Actions workflow
> for `dotnet test` and `npm run build` would be a low-risk next step."

### Document Upload Not Implemented

> "Document upload was listed as optional scope in `docs/MVP_SCOPE.md` from the start.
> The backend has no document storage infrastructure. Adding it would require an
> upload endpoint, a storage provider (Azure Blob or local file), and a document
> listing endpoint. It is a meaningful feature but was deprioritized to keep the
> core claim and reserve flow complete and correct."

### SLA Monitoring Job Not Implemented

> "SLA monitoring was listed as 'full SLA monitoring polish' in the out-of-scope list.
> A basic version could be a Hangfire recurring job that checks for claims open beyond
> a threshold and writes an audit or notification entry. It was not implemented to keep
> the scope focused on the primary claim and reserve workflow."

### Real Authentication Not Implemented

> "Real authentication — JWT, ASP.NET Core Identity, or an external identity provider —
> was explicitly out of scope for this MVP. The mock user selector demonstrates all the
> role-sensitive behavior the assessment requires. Replacing it with real auth would be
> a well-defined upgrade path without changing any business rules."

---

## Closing Notes

End the demo by showing:

- `docs/TRADEOFFS.md` — what was simplified and why
- `docs/ARCHITECTURE_DECISIONS.md` — key design choices with reasons

Key message:

> "The missing items are documented conscious tradeoffs, not hidden gaps.
> The core claim and reserve workflow is complete, tested, and explainable.
> The backend integration tests cover the full MVP flow automatically."
