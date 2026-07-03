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

Phase 6 Hangfire GL posting simulation is completed.

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

Claim creation now goes through MediatR, FluentValidation, EF Core, and writes a `ClaimCreated` audit log entry. Claims can also be browsed through read-only list/detail endpoints and moved through controlled status transitions with audit logging. Reserves can be created for existing claims, with small reserves auto-approved and larger reserves marked pending approval. Pending reserves can be manually approved or rejected by supervisors or managers, with self-approval blocked. Approved reserves are posted to a simulated GL through Hangfire with idempotency protection. Authentication, real GL integration, and frontend code have intentionally not been implemented yet.

## Local Database

For local development, SQL Server can run through Docker:

```bash
docker compose up -d
dotnet ef database update --project src/ClaimsModule.Persistence --startup-project src/ClaimsModule.API
```

The Docker password in `docker-compose.yml` and `appsettings.Development.json` is fake local development configuration, not production credentials.

In Development, the local Hangfire dashboard is available at `/hangfire` without authentication for demo convenience only.
