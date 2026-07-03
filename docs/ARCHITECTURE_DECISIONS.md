# Architecture Decisions

This file records decisions that should guide implementation. Add new entries when important choices are made.

## Decision 1: Backend-First MVP

Status: Accepted

Decision:

The first working product should be a backend-first MVP demonstrated through Swagger/OpenAPI.

Reason:

The backend carries the most important assessment value: domain modeling, validation, persistence, business rules, auditability, and background processing.

Tradeoff:

The user interface may be basic or absent at first. This is acceptable because Swagger can demonstrate the full business flow.

## Decision 2: Clean Architecture Structure

Status: Accepted

Decision:

Use separate projects for Domain, Application, Persistence, Infrastructure, and API.

Reason:

This keeps responsibilities clear and makes it easier to explain where code belongs.

Tradeoff:

There is more project structure than a single Web API project, but the separation supports the assessment requirements and prevents controller-heavy code.

## Decision 3: Mock Authentication For MVP

Status: Planned

Decision:

Use mock users and roles for handler, supervisor, and manager instead of a full real authentication system.

Reason:

The MVP needs to demonstrate role-sensitive behavior such as reserve approval and self-approval prevention, but full authentication would take time away from the claims workflow.

Tradeoff:

This is not production-ready security. It is acceptable for a technical assessment MVP if clearly documented.

## Decision 4: SQL Server With Docker Preferred

Status: Planned

Decision:

Use SQL Server for persistence and prefer Docker for local reproducibility.

Reason:

SQL Server matches the expected .NET stack and Docker makes local setup easier to repeat.

Tradeoff:

Local setup requires Docker. If Docker becomes a blocker, a documented fallback may be needed.

## Decision 5: Hangfire For GL Posting Simulation

Status: Planned

Decision:

Use Hangfire to simulate asynchronous GL posting after reserve approval.

Reason:

This demonstrates background processing and creates a useful audit trail after approval.

Tradeoff:

The job does not integrate with a real finance system. It only simulates posting for the MVP.

## Decision 6: Keep Business Rules Explicit

Status: Accepted

Decision:

Important rules should be implemented in simple, visible code rather than hidden behind generic abstractions.

Reason:

The code must be explainable by a beginner-level .NET developer during review.

Tradeoff:

The code may be less abstract than a senior enterprise system, but it will be easier to understand and defend.

## Decision 7: Protect Audit History With Restrictive Delete Behavior

Status: Accepted

Decision:

Use restrictive delete behavior for audit log relationships and other relationships where automatic cascading could remove important history.

Reason:

Audit entries are intended to be append-only evidence of important actions. Future application code should add new audit entries instead of editing or deleting history.

Tradeoff:

Some delete operations may require explicit cleanup or may be blocked by the database. This is acceptable for the MVP because claim auditability is more important than convenient deletion.
