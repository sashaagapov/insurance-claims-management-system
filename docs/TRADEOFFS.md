# Tradeoffs

Use this document to explain what the MVP includes, what is simplified, what is intentionally out of scope, and why.

## Current Implementation

Current phase:

- Phase 0: repository/context setup

Implemented:

- Repository planning documents
- AI agent rules
- Backend-first MVP scope
- Implementation roadmap
- Architecture decision log
- AI workflow log template
- Demo script

Not implemented yet:

- .NET solution
- API
- database
- domain model
- frontend

Reason:


## Simplifications

Planned MVP simplifications:

- Mock users and roles instead of full authentication
- One fixed organization / tenant
- Swagger-first demo before frontend
- Simulated policies instead of integration with a real policy system
- Simulated GL posting instead of integration with a real finance system
- Basic reserve approval thresholds
- Minimal document handling, or no document handling, unless time allows

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
- automated integration tests
- CI/CD pipeline
- deployment scripts
- observability and structured logging

## Review Position

The MVP should be presented as a deliberate, scoped vertical slice. The goal is to show that the important business flow works and that tradeoffs were made consciously.
