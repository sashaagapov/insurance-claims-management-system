# MVP Scope

## Goal

Build a backend-first vertical slice for a simplified Claims Management System.

Swagger/OpenAPI is acceptable as the first complete demo surface. A basic Angular frontend is optional and should only be considered after the backend core is stable.

## Must-Have Backend Scope

- Clean Architecture structure
- ASP.NET Core Web API
- EF Core
- SQL Server
- Docker-based SQL Server setup where practical
- MediatR commands and queries
- FluentValidation command validation
- Swagger/OpenAPI
- Seeded policies
- Seeded cause-of-loss codes
- Mock users and roles: handler, supervisor, manager
- Claim creation / FNOL
- Policy lookup
- Claim list
- Claim detail
- Claim status transitions
- Reserve creation
- Reserve approval / rejection
- Self-approval prevention
- Audit log
- Hangfire GL posting simulation
- Idempotency protection for GL posting job
- Documentation for architecture, scope, AI workflow, and tradeoffs

## Optional Or Simplified Scope

- Basic Angular frontend after backend works
- Basic frontend screens:
  - claims list
  - create claim form
  - claim detail
  - reserves
  - audit log
- Mock authentication instead of real authentication
- One fixed organization / tenant
- Local storage fallback instead of Azure Blob if document storage is attempted
- Basic deployment only if time allows
- Basic CI/CD only if time allows

## Out Of Scope For MVP

- Perfect Angular UI
- NgRx
- Full real authentication system
- Complex multi-tenant architecture
- Full Azure infrastructure
- Full CI/CD pipeline
- Complex document management
- Full SLA monitoring polish
- Extra features not required for the main flow
- Any implementation that makes the code too complex for an intern-level developer to explain

## Backend-First Rationale

The backend is the main evidence for the assessment. It demonstrates domain modeling, validation, persistence, business rules, auditability, background jobs, and clear architecture.

A frontend can help the demo later, but it should not distract from the core backend flow.

## Definition Of A Successful MVP

The MVP is successful if a reviewer can follow this flow through Swagger:

1. Find a policy.
2. Create a claim.
3. View the claim.
4. Change claim status.
5. Create reserves with different approval outcomes.
6. Prevent self-approval.
7. Approve a reserve as a supervisor or manager.
8. See audit log entries.
9. See simulated GL posting after approval.
