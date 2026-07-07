# Source Structure

This folder contains the .NET 9 Clean Architecture backend for the Claims Module MVP.

## Projects

- `ClaimsModule.Domain`: entities, enums, and domain model.
- `ClaimsModule.Application`: MediatR commands/queries, DTOs, validators, interfaces, and business rules.
- `ClaimsModule.Persistence`: EF Core DbContext, SQL Server configuration, migrations, and seed data.
- `ClaimsModule.Infrastructure`: Hangfire GL posting simulation and infrastructure services.
- `ClaimsModule.API`: ASP.NET Core startup, controllers, middleware, Swagger, and HTTP endpoints.

## Current Implementation

Implemented scope includes FNOL claim creation, claim list/detail, claim status transitions, reserve creation/approval/rejection, Hangfire-backed GL posting simulation, audit logging, an Angular frontend MVP, and backend integration tests.
