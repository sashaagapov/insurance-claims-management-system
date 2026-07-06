# DICEUS Claims Module Assessment

This repository contains a planned backend-first MVP for a simplified Claims Management System in the insurance domain.


## Project Idea

The system will model a simplified claim lifecycle:

- A user creates a claim after a loss event.
- The first claim creation process is called FNOL, or First Notice of Loss.
- A claim can be linked to a simulated insurance policy.
- A claim contains loss details, claimant parties, risk objects, status, reserves, and audit log records.
- A reserve represents an expected financial cost of the claim.
- Small reserves are auto-approved.
- Larger reserves require supervisor or manager approval.
- A user must not approve their own reserve.
- Approved reserves trigger a Hangfire background job that simulates GL posting and writes an audit log entry.

## MVP Direction

This is a backend-first MVP. The first demo path should work through Swagger/OpenAPI before any frontend is added.

The backend is the main assessment focus because it shows:

- domain modeling
- API design
- validation
- persistence
- business rules
- background processing
- auditability
- explainable architecture

## Planned Stack

- ASP.NET Core Web API
- Clean Architecture
- EF Core
- SQL Server
- Docker-based SQL Server for local reproducibility
- MediatR
- FluentValidation
- Swagger/OpenAPI
- Hangfire
- Optional Angular frontend after the backend core is stable

## Planned Demo Path

1. Start the API.
2. Open Swagger.
3. Search seeded policies.
4. Create a claim through FNOL.
5. View claim details.
6. View audit log entries.
7. Transition claim status.
8. Create a reserve under 10,000 and show auto-approval.
9. Create a reserve over 10,000 and show pending approval.
10. Try self-approval and show the validation error.
11. Approve the reserve as supervisor or manager.
12. Show the Hangfire GL posting audit entry.



Future AI work should:

- read the planning documents before coding
- implement small vertical slices
- keep the code explainable for a beginner-level .NET developer
- document decisions and tradeoffs
- avoid overbuilding
- run build/tests when possible


## Current Status

Phase 7B automated backend integration tests are completed.

The repository now contains a .NET 9 Clean Architecture solution with separate Domain, Application, Persistence, Infrastructure, and API projects. The Domain project contains the initial claims-domain entities and enums. The Persistence project contains the EF Core DbContext, entity configurations, seed data, and the initial migration.

The API currently exposes:

- `GET /health`
- `GET /api/policies`
- `GET /api/cause-of-loss-codes`
- `GET /api/claims`
- `GET /api/claims/{id}`
- `POST /api/claims`
- `PATCH /api/claims/{id}/status`
- `POST /api/claims/{claimId}/reserves`
- `PATCH /api/claims/{claimId}/reserves/{reserveId}/approve`
- `PATCH /api/claims/{claimId}/reserves/{reserveId}/reject`

Claim creation now goes through MediatR, FluentValidation, EF Core, and writes a `ClaimCreated` audit log entry. Claims can also be browsed through read-only list/detail endpoints and moved through controlled status transitions with audit logging. Reserves can be created for existing claims, with small reserves auto-approved and larger reserves marked pending approval. Pending reserves can be manually approved or rejected by supervisors or managers, with self-approval blocked. Approved reserves are posted to a simulated GL through Hangfire with idempotency protection. The Angular frontend currently includes the app shell, mock user selector, shared API layer, claims list dashboard, FNOL create claim form, and read-only claim detail screen. Authentication, real GL integration, reserve actions UI, and status transition UI have intentionally not been implemented yet.

## Automated Tests

The solution includes automated backend integration tests in `tests/ClaimsModule.IntegrationTests`.

Run them with:

```bash
docker compose up -d
dotnet test ClaimsModule.sln
```

Docker is required because the tests use the SQL Server container from `docker-compose.yml`. The test project creates a temporary SQL Server database, applies EF Core migrations, runs the API through `Microsoft.AspNetCore.Mvc.Testing`, and drops the temporary database after the run.

The tests cover:

- `GET /health`
- seeded policy and cause-of-loss lookup endpoints
- FNOL claim creation, claim list, claim detail, and `ClaimCreated` audit entry
- valid and invalid claim status transitions
- reserve threshold behavior for approved and pending reserves
- handler/supervisor/manager approval and rejection rules
- Hangfire-backed GL posting fields and direct GL posting job idempotency

## Frontend Local Development

The Angular frontend lives in `frontend/claims-module-web`.

Run it locally with:

```bash
cd frontend/claims-module-web
npm install
npm start
```

Open `http://localhost:4200`.

The frontend API base URL is configured through Angular environment files and currently points to `http://localhost:5188`. The current shell includes shared API services, global API error handling, a mock user selector for the seeded Handler, Supervisor, and Manager users, a claims list dashboard, an FNOL create claim form, and a read-only claim detail screen. Reserve and status action workflows are intentionally reserved for later approved phases.

Frontend demo path:

1. Open `http://localhost:4200/claims`.
2. Use filters to review existing claims.
3. Click `Log New Claim`.
4. Select a seeded policy and cause of loss.
5. Complete party and risk object details.
6. Submit the claim.
7. Confirm the success message and redirect to the claim detail screen.
8. Review Overview, Parties, Risk Objects, Reserves, and Audit Log tabs.
9. Return to `/claims` and confirm the new claim appears in the list.

## Local Database

For local development, SQL Server can run through Docker:

```bash
docker compose up -d
dotnet tool restore
dotnet tool run dotnet-ef database update --project src/ClaimsModule.Persistence --startup-project src/ClaimsModule.API
```

The Docker password in `docker-compose.yml` and `appsettings.Development.json` is fake local development configuration, not production credentials.

In Development, the local Hangfire dashboard is available at `/hangfire` without authentication for demo convenience only.

## Demo Flow

Start the local backend:

```bash
docker compose up -d
dotnet tool restore
dotnet tool run dotnet-ef database update --project src/ClaimsModule.Persistence --startup-project src/ClaimsModule.API
dotnet run --project src/ClaimsModule.API/ClaimsModule.API.csproj --urls http://localhost:5188
```

Open:

- Swagger: `http://localhost:5188/swagger`
- Hangfire dashboard in Development: `http://localhost:5188/hangfire`

Seeded demo IDs:

- Handler user: `bbbbbbbb-0000-0000-0000-000000000001`
- Supervisor user: `bbbbbbbb-0000-0000-0000-000000000002`
- Manager user: `bbbbbbbb-0000-0000-0000-000000000003`
- Auto policy: `11111111-1111-1111-1111-111111111111`
- Collision cause of loss: `aaaaaaaa-0000-0000-0000-000000000001`

Recommended Swagger path:

1. `GET /api/policies`
2. `GET /api/cause-of-loss-codes`
3. `POST /api/claims`
4. `GET /api/claims/{id}`
5. `PATCH /api/claims/{id}/status`
6. `POST /api/claims/{claimId}/reserves` with `5000 USD`; expect `Approved`, then `ReserveGlPosted`
7. `POST /api/claims/{claimId}/reserves` with `15000 USD`; expect `PendingApproval`
8. `PATCH /api/claims/{claimId}/reserves/{reserveId}/approve` as supervisor or manager; expect `Approved`, then `ReserveGlPosted`
9. Create another `15000 USD` reserve and reject it as manager; expect `Rejected` and no GL posting fields
10. Try invalid/self/forbidden cases:
    - invalid claim transition returns `422`
    - handler reserve approval returns `403`
    - self-approval returns `422`

### Sample Requests

Create a claim:

```json
{
  "policyId": "11111111-1111-1111-1111-111111111111",
  "causeOfLossCodeId": "aaaaaaaa-0000-0000-0000-000000000001",
  "lossDate": "2026-07-02",
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

Change claim status from `Open` to `UnderInvestigation`:

```json
{
  "newStatus": "UnderInvestigation",
  "actorUserId": "bbbbbbbb-0000-0000-0000-000000000001"
}
```

Create a reserve:

```json
{
  "amount": 5000,
  "currency": "USD",
  "createdByUserId": "bbbbbbbb-0000-0000-0000-000000000001"
}
```

Approve a pending reserve:

```json
{
  "actorUserId": "bbbbbbbb-0000-0000-0000-000000000002"
}
```

Reject a pending reserve:

```json
{
  "actorUserId": "bbbbbbbb-0000-0000-0000-000000000003",
  "reason": "Insufficient supporting information for this reserve amount."
}
```

After a GL posting job runs, `GET /api/claims/{id}` should show `glPostedAtUtc`, `glPostingReference`, and a `ReserveGlPosted` audit entry for approved reserves.
