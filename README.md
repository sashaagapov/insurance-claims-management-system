# Insurance Claims Management System

An insurance claims demo with a .NET 9 API, SQL Server persistence, Hangfire jobs, and an Angular interface. It follows a claim from first notice of loss (FNOL) through reserve approval and a simulated general-ledger posting.

## See the result

The [backend integration tests](tests/ClaimsModule.IntegrationTests/BackendMvpFlowTests.cs) check this example flow against the API and a temporary SQL Server database. The table summarizes their assertions; generated IDs and timestamps vary by run.

| Action | Outcome asserted in tests |
|---|---|
| Create a claim using a seeded policy | HTTP `201`; claim starts `Open` with a `ClaimCreated` audit entry. |
| Move the claim to `UnderInvestigation` | HTTP `200`; an invalid next transition returns `422`. |
| Add a 5,000 USD reserve | `Approved` immediately. |
| Add a 15,000 USD reserve | `PendingApproval`; handler approval returns `403`, self-approval returns `422`, supervisor approval succeeds. |
| Run GL posting twice for an approved reserve | Posting reference remains the same and there is one `ReserveGlPosted` audit entry. |

The [sample FNOL request](#sample-requests) and [demo script](docs/DEMO_SCRIPT.md) show how to reproduce the flow. This repository has no deployed demo; the Angular app runs locally.

## Explore the project

| Feature | Implementation | Evidence or walkthrough |
|---|---|---|
| Policy and cause-of-loss lookup | [Policies API](src/ClaimsModule.API/Controllers/PoliciesController.cs) · [cause-of-loss API](src/ClaimsModule.API/Controllers/CauseOfLossCodesController.cs) | [Seed data](src/ClaimsModule.Persistence/Seed/SeedData.cs) · [lookup tests](tests/ClaimsModule.IntegrationTests/BackendMvpFlowTests.cs) |
| FNOL and claim list/detail | [Claims API](src/ClaimsModule.API/Controllers/ClaimsController.cs) · [claim creation](src/ClaimsModule.Application/Claims/CreateClaim/CreateClaimCommandHandler.cs) | [Integration flow](tests/ClaimsModule.IntegrationTests/BackendMvpFlowTests.cs) · [FNOL screen](frontend/claims-module-web/src/app/features/claims/fnol-form/fnol-form.html) |
| Status transitions and audit | [Status handler](src/ClaimsModule.Application/Claims/UpdateClaimStatus/UpdateClaimStatusCommandHandler.cs) · [audit entry](src/ClaimsModule.Domain/Entities/AuditLogEntry.cs) | [Integration flow](tests/ClaimsModule.IntegrationTests/BackendMvpFlowTests.cs) · [claim detail screen](frontend/claims-module-web/src/app/features/claims/claim-detail/claim-detail.html) |
| Reserve threshold and approvals | [Reserve creation](src/ClaimsModule.Application/Reserves/CreateReserve/CreateReserveCommandHandler.cs) · [approval](src/ClaimsModule.Application/Reserves/ApproveReserve/ApproveReserveCommandHandler.cs) · [rejection](src/ClaimsModule.Application/Reserves/RejectReserve/RejectReserveCommandHandler.cs) | [Reserve tests](tests/ClaimsModule.IntegrationTests/BackendMvpFlowTests.cs) |
| GL posting | [Hangfire job](src/ClaimsModule.Infrastructure/BackgroundJobs/ReserveGlPostingJob.cs) | [Idempotency test](tests/ClaimsModule.IntegrationTests/BackendMvpFlowTests.cs) |
| Seeded data and database | [Seed data](src/ClaimsModule.Persistence/Seed/SeedData.cs) · [migrations](src/ClaimsModule.Persistence/Migrations/) | [Seeded demo IDs](#demo-flow) |
| Angular app and mock user selector | [Routes](frontend/claims-module-web/src/app/app.routes.ts) · [user context](frontend/claims-module-web/src/app/core/services/auth-context.service.ts) | [Frontend guide](frontend/claims-module-web/README.md) · [demo path](#frontend-local-development) |

For architecture and scope, see [architecture decisions](docs/ARCHITECTURE_DECISIONS.md), [MVP scope](docs/MVP_SCOPE.md), [tradeoffs](docs/TRADEOFFS.md), and the [assessment gap report](docs/ASSESSMENT_GAP_REPORT.md). For a guided review, use the [demo script](docs/DEMO_SCRIPT.md) and [demo checklist](docs/DEMO_CHECKLIST.md). Jump to [local setup](#demo-flow) or [automated tests](#automated-tests).

## Run locally

Install the .NET SDK version pinned in [global.json](global.json), Node.js 20+, and Docker Desktop. From the repository root, start SQL Server, apply migrations, and launch the API:

```bash
docker compose up -d
dotnet tool restore
dotnet tool run dotnet-ef database update --project src/ClaimsModule.Persistence --startup-project src/ClaimsModule.API
dotnet run --project src/ClaimsModule.API/ClaimsModule.API.csproj --urls http://localhost:5188
```

In another terminal, run `cd frontend/claims-module-web`, then `npm install` and `npm start`. Open the [Angular app](http://localhost:4200) or [Swagger UI](http://localhost:5188/swagger). Backend tests use the running SQL Server container: `dotnet test ClaimsModule.sln`. See the [demo script](docs/DEMO_SCRIPT.md) for the complete sequence.

## API endpoints

The backend exposes:

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

The frontend API base URL is configured through Angular environment files and currently points to `http://localhost:5188`. The current shell includes shared API services, global API error handling, a mock user selector for the seeded Handler, Supervisor, and Manager users, a claims list dashboard, an FNOL create claim form, a claim detail screen, reserve actions, and a minimal `Open -> UnderInvestigation` claim status action.

Frontend demo path:

1. Open `http://localhost:4200/claims`.
2. Use filters to review existing claims.
3. Click `Log New Claim`.
4. Select a seeded policy and cause of loss.
5. Complete party and risk object details.
6. Submit the claim.
7. Confirm the success message and redirect to the claim detail screen.
8. Review Overview, Parties, Risk Objects, Reserves, and Audit Log tabs.
9. Add a small reserve and confirm it is auto-approved.
10. Add a large reserve and confirm it is pending approval.
11. Switch demo user to Supervisor or Manager and approve or reject the pending reserve.
12. Use the status action to move an Open claim to Under Investigation.
13. Return to `/claims` and confirm the new claim appears in the list.

## Known Simplifications

- Users and roles are selected through a frontend mock user selector, not real authentication.
- The frontend is local-only and is not deployed.
- Claim list filters are client-side.
- FNOL supports existing seeded policies and cause-of-loss codes only.
- The frontend exposes only the supported `Open -> UnderInvestigation` status action.
- Reserve actions use the existing MVP threshold rules and seeded mock roles.
- Document upload, Azure deployment, and real GL integration remain out of scope. A [GitHub Actions workflow](.github/workflows/ci.yml) is configured to build the backend and frontend and run backend tests on pushes and pull requests to `main`.

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
