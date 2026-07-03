# Backend Demo Checklist

Use this checklist to run the backend MVP through Swagger or curl.

## Commands

```bash
dotnet --version
dotnet restore ClaimsModule.sln
dotnet build ClaimsModule.sln --no-restore
docker compose up -d
dotnet tool restore
dotnet tool run dotnet-ef database update --project src/ClaimsModule.Persistence --startup-project src/ClaimsModule.API
dotnet run --project src/ClaimsModule.API/ClaimsModule.API.csproj --urls http://localhost:5188
```

Open:

- Swagger: `http://localhost:5188/swagger`
- Hangfire dashboard in Development: `http://localhost:5188/hangfire`

## Seeded Demo Data

- Handler user: `bbbbbbbb-0000-0000-0000-000000000001`
- Supervisor user: `bbbbbbbb-0000-0000-0000-000000000002`
- Manager user: `bbbbbbbb-0000-0000-0000-000000000003`
- Auto policy: `11111111-1111-1111-1111-111111111111`
- Collision cause of loss: `aaaaaaaa-0000-0000-0000-000000000001`

## Endpoints To Test

- `GET /health` returns `OK`
- `GET /api/policies` returns seeded policies
- `GET /api/cause-of-loss-codes` returns seeded cause codes
- `POST /api/claims` returns `201 Created`
- `GET /api/claims` returns claims newest first
- `GET /api/claims/{id}` returns parties, risk objects, reserves, and audit log entries
- `PATCH /api/claims/{id}/status` returns `200 OK` for allowed transitions
- `POST /api/claims/{claimId}/reserves` with `5000 USD` returns `Approved`
- `POST /api/claims/{claimId}/reserves` with `15000 USD` returns `PendingApproval`
- `PATCH /api/claims/{claimId}/reserves/{reserveId}/approve` returns `200 OK` for supervisor or manager
- `PATCH /api/claims/{claimId}/reserves/{reserveId}/reject` returns `200 OK` for supervisor or manager

## Expected Demo Outputs

- Created claims start with status `Open`.
- `Open -> UnderInvestigation` succeeds.
- Invalid status transitions return `422 Unprocessable Entity`.
- Small reserves are auto-approved.
- Large reserves are pending until approved or rejected.
- Handler role returns `403 Forbidden` for reserve approval or rejection.
- Self-approval and self-rejection return `422 Unprocessable Entity`.
- Approved reserves eventually show `glPostedAtUtc` and `glPostingReference`.
- Approved reserves get a `ReserveGlPosted` audit entry after Hangfire runs.
- Rejected reserves do not get GL posting fields.
- Hangfire duplicate job execution should not duplicate `ReserveGlPosted` audit entries.

## Common Errors And Fixes

- Docker not running: start Docker Desktop, then run `docker compose up -d` again.
- SQL Server not ready yet: wait a few seconds and rerun the EF database update command.
- Port `1433` already used: stop the other SQL Server process or change the host port in `docker-compose.yml`.
- API port already used: stop the process on `5188` or run the API with a different `--urls` port.
- Hangfire dashboard missing: confirm the API is running in Development and open `/hangfire`.
- Invalid transition returns `422`: use an allowed transition such as `Open -> UnderInvestigation`.
- Handler role returns `403` for reserve approval: use supervisor or manager user IDs.
- Self-approval returns `422`: approve with a different supervisor or manager user than the reserve creator.
