# Source Structure

This folder contains the backend Clean Architecture scaffold for the Claims Module MVP.

## Projects

- `ClaimsModule.Domain`: entities, enums, value objects, domain events, and domain behavior later.
- `ClaimsModule.Application`: MediatR commands/queries, DTOs, validators, and interfaces later.
- `ClaimsModule.Persistence`: EF Core DbContext, configurations, migrations, and seed data later.
- `ClaimsModule.Infrastructure`: Hangfire jobs, storage, and external service implementations later.
- `ClaimsModule.API`: ASP.NET Core startup, controllers, middleware, Swagger, and HTTP endpoints.

## Current Phase

Phase 1 only creates the scaffold. It intentionally does not include claim logic, reserve logic, EF entities, migrations, authentication, or frontend code.
