# Tradeoffs

Use this document to explain what the MVP includes, what is simplified, what is intentionally out of scope, and why.

## Current Implementation

Current phase:

- Phase 8A: Angular scaffold only

Implemented:

- Repository planning documents
- AI agent rules
- Backend-first MVP scope
- Implementation roadmap
- Architecture decision log
- AI workflow log template
- Demo script
- .NET 9 Clean Architecture solution scaffold
- Initial domain entities and enums
- EF Core DbContext
- EF Core entity configurations
- Seed data for policies, cause-of-loss codes, and mock users
- SQL Server local development setup
- FNOL claim creation endpoint
- Minimal policy and cause-of-loss lookup endpoints for Swagger testing
- Claim creation audit log entry
- Read-only claim list and detail endpoints
- Controlled claim status transition endpoint
- Reserve creation endpoint
- Auto-approved small reserves and pending larger reserves
- Manual reserve approval and rejection endpoints
- Self-approval and self-rejection prevention
- Hangfire-backed simulated GL posting for approved reserves
- Idempotency protection for GL posting jobs
- README demo flow and backend demo checklist
- Automated backend integration tests for the main MVP flow
- Angular frontend scaffold with routing, Angular Material, an app shell, and placeholder home page

Not implemented yet:

- frontend business screens
- real external GL or accounting integration
- real authentication

Reason:

The project is being built phase by phase so each part remains explainable, reviewable, and realistic for the assessment.

## Simplifications

Planned MVP simplifications:

- Mock users and roles instead of full authentication
- One fixed organization / tenant
- Swagger-first demo before frontend
- Simulated policies instead of integration with a real policy system
- Simulated GL posting instead of integration with a real finance system
- Basic reserve approval thresholds
- Minimal document handling, or no document handling, unless time allows
- Local Docker SQL Server uses fake development credentials only
- Phase 3 includes only the lookup endpoints needed to test FNOL creation, not a full claim browsing API
- Phase 5A assigns reserve status by threshold but does not yet enforce manual approval, rejection, or self-approval rules
- Phase 5B uses seeded mock users and roles instead of a real authentication or authorization system
- Hangfire is used for local simulated background processing, not production scheduling configuration
- GL posting is simulated by writing local reserve fields and an audit log entry
- The Hangfire dashboard is enabled only in Development and is unauthenticated for local demo convenience
- GL posting idempotency checks `GlPostedAtUtc` so duplicate jobs do not create duplicate posting audit records
- Phase 7A improved demo documentation only
- Phase 7B uses the local Docker Compose SQL Server for integration tests instead of Testcontainers because SQL Server Testcontainers were unstable in the local Docker Desktop resource limits during this phase
- Integration tests create and drop a temporary SQL Server database per test run, but they still require Docker and port `1433`
- Phase 8A uses Angular 21 because Angular CLI 22 does not support the local Node 25 runtime; this keeps the scaffold on Angular 18+ while matching available local tooling

## Intentionally Out Of Scope

The MVP should not attempt:

- perfect Angular UI
- NgRx
- full real authentication
- complex multi-tenant architecture
- full Azure infrastructure
- full CI/CD pipeline
- complex document management
- full SLA monitoring polish
- extra features outside the main claim and reserve flow

## Why Backend-First

Backend-first is chosen because the assessment value is mostly in:

- business-rule implementation
- data modeling
- validation
- API behavior
- persistence
- audit logging
- background jobs
- architecture clarity

Swagger can demonstrate the core workflow without spending early time on frontend polish.

## What Would Be Improved With More Time

With more time, the project could add:

- real authentication and authorization
- better role and permission management
- richer claim lifecycle rules
- real policy system integration
- real GL or finance integration
- document upload and storage
- Angular UI with stronger UX
- CI/CD pipeline
- deployment scripts
- observability and structured logging

## Review Position

The MVP should be presented as a deliberate, scoped vertical slice. The goal is to show that the important business flow works and that tradeoffs were made consciously.
