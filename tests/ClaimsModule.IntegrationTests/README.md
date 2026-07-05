# ClaimsModule Integration Tests

These tests use `Microsoft.AspNetCore.Mvc.Testing` to run the API against the real SQL Server container from `docker-compose.yml`.

## Run

```bash
docker compose up -d
dotnet test ClaimsModule.sln
```

Docker must be running because the tests connect to SQL Server on `localhost,1433`. The test project creates a temporary database, applies EF Core migrations automatically, and drops the temporary database after the run.

## Coverage

- health endpoint
- seeded policy and cause-of-loss lookups
- FNOL claim creation, claim detail, claim list, and audit log
- claim status transitions and invalid transition handling
- reserve creation threshold behavior
- reserve approval/rejection authorization and validation rules
- Hangfire-backed GL posting and direct GL job idempotency
