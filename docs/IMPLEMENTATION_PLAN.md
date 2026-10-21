# Implementation Plan

## Phase 0: Repository/Context Setup

Goal: Prepare the repository scope and implementation plan.

Expected output:

- planning documents
- agent rules
- MVP boundaries
- AI workflow template
- demo script

Main files or areas affected:

- `README.md`
- `docs/`

What I must understand and explain:

- why implementation has not started yet
- why the project is backend-first
- what future agents must read before coding
- what is intentionally out of scope

Suggested commit message:

`docs: initialize assessment context`

## Phase 1: Backend Solution Scaffold

Goal: Create the .NET solution and Clean Architecture project structure.

Expected output:

- solution file
- API project
- Domain project
- Application project
- Persistence project
- Infrastructure project
- project references
- basic Swagger-enabled API startup

Main files or areas affected:

- `ClaimsModule.sln`
- `src/ClaimsModule.API/`
- `src/ClaimsModule.Domain/`
- `src/ClaimsModule.Application/`
- `src/ClaimsModule.Persistence/`
- `src/ClaimsModule.Infrastructure/`

What I must understand and explain:

- why each project exists
- dependency direction between projects
- why controllers should stay thin
- how a request will move through the backend

Suggested commit message:

`scaffold: create clean architecture solution`

## Phase 2: Domain Model And EF Core Database

Goal: Add core domain entities and database persistence.

Expected output:

- entities for claims, policies, reserves, parties, risk objects, and audit logs
- DbContext
- EF Core configuration
- initial migration
- SQL Server connection setup
- seed data for policies and cause-of-loss codes

Main files or areas affected:

- `ClaimsModule.Domain/Entities/`
- `ClaimsModule.Persistence/`
- migration files
- API configuration
- Docker compose if used

What I must understand and explain:

- main entity relationships
- why audit logs are append-only
- how seed data supports the demo
- how EF Core maps entities to SQL Server tables

Suggested commit message:

`feat: add domain model and persistence`

## Phase 3: Claim/FNOL Creation

Goal: Implement the first complete vertical slice for creating a claim.

Expected output:

- FNOL/create claim endpoint
- MediatR command
- FluentValidation validator
- command handler
- persistence logic
- audit log entry for claim creation
- Swagger test path

Main files or areas affected:

- API controller
- Application commands and validators
- Persistence DbContext usage
- Domain claim behavior

What I must understand and explain:

- what FNOL means
- required claim input
- how validation differs from business rules
- how audit logging happens for claim creation

Suggested commit message:

`feat: add claim creation flow`

## Phase 4: Claim Queries And Status Transitions

Goal: Add claim list, claim detail, and simple status transition behavior.

Expected output:

- claim list endpoint
- claim detail endpoint
- status transition endpoint
- validation of allowed transitions
- audit log entries for status changes

Main files or areas affected:

- API controllers
- Application queries
- Application commands
- Domain status logic
- audit log persistence

What I must understand and explain:

- supported claim statuses
- allowed and rejected transitions
- why status logic should not live directly in the controller

Suggested commit message:

`feat: add claim queries and status transitions`

## Phase 5: Reserve Creation And Approval Workflow

Goal: Implement reserve creation, auto-approval for small reserves, manual approval for larger reserves, rejection, and self-approval prevention.

Expected output:

- create reserve endpoint
- approve reserve endpoint
- reject reserve endpoint
- auto-approval below threshold
- pending approval above threshold
- role checks for supervisor or manager
- self-approval validation
- audit log entries

Main files or areas affected:

- reserve commands
- reserve validators
- reserve handlers
- reserve entity/domain methods
- audit log behavior

What I must understand and explain:

- reserve approval threshold
- why self-approval is blocked
- difference between auto-approved and pending reserves
- how roles are mocked for the MVP

Suggested commit message:

`feat: add reserve approval workflow`

## Phase 6: Hangfire GL Posting And Audit Log

Goal: Simulate GL posting after reserve approval and ensure the job is idempotent.

Expected output:

- Hangfire configuration
- background job for GL posting
- idempotency check
- audit log entry after posting
- visible job behavior in local demo

Main files or areas affected:

- Infrastructure background jobs
- API startup configuration
- Persistence records used for idempotency
- audit log entries

What I must understand and explain:

- why GL posting is asynchronous
- what idempotency means
- how duplicate posting is prevented
- where Hangfire fits in the architecture

Suggested commit message:

`infra: add Hangfire GL posting simulation`

## Phase 7: Swagger-First Demo And Optional Basic Frontend

Goal: Make the backend demo smooth through Swagger and optionally add minimal Angular screens if time allows.

Expected output:

- clear Swagger endpoints
- sample request bodies
- optional simple Angular screens
- demo data that supports the full flow

Main files or areas affected:

- API endpoint metadata
- seed data
- optional frontend folder

What I must understand and explain:

- how to demo the complete flow
- which parts are backend MVP and which parts are optional UI polish

Suggested commit message:

`docs: prepare swagger-first demo`

or, if frontend is added:

`feat: add basic claims frontend`

## Phase 8: Documentation, Cleanup, And Tradeoffs

Goal: Make the project review-ready.

Expected output:

- updated README
- updated tradeoffs
- architecture notes
- known limitations
- cleanup of generated files
- build/test verification notes

Main files or areas affected:

- `README.md`
- `docs/TRADEOFFS.md`
- `docs/ARCHITECTURE_DECISIONS.md`
- source comments where useful

What I must understand and explain:

- what was intentionally simplified
- what would be improved with more time
- why the final scope is defensible

Suggested commit message:

`docs: add final tradeoffs and review notes`

## Phase 9: Live Review Preparation

Goal: Prepare for explaining and demonstrating the project live.

Expected output:

- final demo script
- checklist of commands
- expected API responses
- explanation notes for architecture and business rules

Main files or areas affected:

- `docs/DEMO_SCRIPT.md`
- `README.md`
- final review notes

What I must understand and explain:

- how to run the app
- how each endpoint supports the business flow
- where major rules are implemented
- which tradeoffs were made and why

Suggested commit message:

`docs: prepare live review script`
