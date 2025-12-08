# Salon Suite Platform (Reference Backend)

This repository provides a reference implementation for the multi-service salon management platform described in the global instructions. It focuses on production-ready foundations rather than exhaustive business logic so you can evolve bounded contexts independently.

## Tech Stack

- **Runtime**: .NET 8, ASP.NET Core minimal APIs
- **Patterns**: CQRS with MediatR, layered Domain/Application/Infrastructure per service
- **Data**: EF Core (SQL Server provider), Redis cache abstraction, MassTransit + RabbitMQ for integration events
- **Cross-cutting**: FluentValidation, AutoMapper hooks, Serilog + OpenAPI support, Prometheus middleware stub
- **Testing**: Solution builds cleanly (`dotnet build`). Add xUnit projects alongside services as features grow.

## Solution Layout

```
/workspace
├── Directory.Build.props / Directory.Packages.props   # Centralized builds + packages
├── docker-compose.infrastructure.yml                  # Local SQL Server + Redis + RabbitMQ
├── src
│   ├── BuildingBlocks                                 # Shared abstractions & infra helpers
│   ├── Shared/SalonSuite.Contracts                    # DTOs & integration events
│   └── Services
│       ├── IdentityAccess
│       ├── BranchConfiguration
│       ├── Catalog
│       ├── BookingCalendar
│       ├── PosTransactions
│       ├── PaymentsAdapter
│       ├── StaffPayroll
│       ├── ClientManagement
│       ├── Notification
│       ├── ReportingAnalytics
│       ├── SyncOffline
│       └── AuditCompliance
└── SalonSuite.sln
```

Each service follows the same structure:

```
<ServiceName>
  ├── <Service>.Domain            # Aggregates, value objects, domain errors
  ├── <Service>.Application       # CQRS requests, validators, MediatR handlers
  ├── <Service>.Infrastructure    # DbContext, EF configurations, DI extensions
  └── <Service>.Api               # Minimal API host wiring the module
```

## Building Blocks Recap

- `BuildingBlocks.Domain`: base entity/value-object abstractions, domain events, `Result` wrapper.
- `BuildingBlocks.Application`: CQRS interfaces, pipeline behaviors (validation, logging, transactions, auditing, caching contracts).
- `BuildingBlocks.Infrastructure`: EF Core base context, Redis cache service, Serilog bootstrap, date/time provider, unit of work.
- `BuildingBlocks.Messaging`: Integration event base types plus MassTransit + RabbitMQ wiring helpers.

## Implemented Service Capabilities

| Service | Sample Command | Sample Query |
|---------|----------------|--------------|
| IdentityAccess | `POST /api/v1/users` (Create user) | `GET /api/v1/users?pageNumber=1&pageSize=50` |
| BranchConfiguration | `POST /api/v1/branches` | `GET /api/v1/branches?branchId=<guid>` |
| Catalog | `POST /api/v1/catalog/services` | `GET /api/v1/catalog/services?pageNumber=1&pageSize=50` |
| BookingCalendar | `POST /api/v1/bookings` | `GET /api/v1/bookings?branchId=...&rangeStart=...&rangeEnd=...` |
| PosTransactions | `POST /api/v1/pos/invoices` | `GET /api/v1/pos/invoices?invoiceId=...` |
| PaymentsAdapter | `POST /api/v1/payments` | `GET /api/v1/payments?paymentId=...` |
| StaffPayroll | `POST /api/v1/staff` | `GET /api/v1/staff/performance?staffId=...` |
| ClientManagement | `POST /api/v1/clients` | `GET /api/v1/clients?clientId=...` |
| Notification | `POST /api/v1/notifications` | `GET /api/v1/notifications?notificationId=...` |
| ReportingAnalytics | `POST /api/v1/reports/daily-sales` | `GET /api/v1/reports/daily-sales?branchId=...` |
| SyncOffline | `POST /api/v1/sync/devices` | `GET /api/v1/sync/devices?deviceId=...` |
| AuditCompliance | `POST /api/v1/audit` | `GET /api/v1/audit?category=...&fromUtc=...&toUtc=...` |

Each API host registers JWT auth, Swagger, Serilog request logging, Prometheus middleware stub, and a consistent DI pipeline (`Add<Service>Infrastructure`, `AddInfrastructureBuildingBlocks`, `AddMessaging`).

## Local Development

1. **Install prerequisites**
   - .NET 8 SDK (`dotnet --version` should be 8.0.x).
   - Docker Desktop / compatible engine for infrastructure containers.

2. **Boot shared infrastructure**

   ```bash
   docker compose -f docker-compose.infrastructure.yml up -d
   ```

   - SQL Server exposed on `localhost:14333` (`sa/Your_password123`).
   - Redis on `localhost:6380`.
   - RabbitMQ on `localhost:5673` (management UI `http://localhost:15673`).

3. **Restore & build everything**

   ```bash
   dotnet build
   ```

4. **Run an individual service** (example for Catalog):

   ```bash
   dotnet run --project src/Services/Catalog/Catalog.Api/Catalog.Api.csproj
   ```

   Swagger UI available at `https://localhost:5001/swagger` (port varies per service). Repeat for any service you need to debug.

## Extending the Platform

- Add new commands/queries inside the respective `<Service>.Application` folder. Validators automatically flow through the MediatR pipeline.
- Surface events by emitting `IntegrationEvent` records and publishing via MassTransit in the infrastructure layer.
- Keep all persistence concerns inside `<Service>.Infrastructure`. If your application handlers require more expressive data access, expose repositories or query services from that layer.
- Share reusable DTOs or contracts through `SalonSuite.Contracts`.

## Testing Hooks

- Each service compiles independently; add xUnit projects under `<Service>.Tests` referencing domain + application layers.
- For integration tests, spin up Docker dependencies before executing test suites.

## Next Steps

- Wire up authentication/authorization policies once the identity provider is available.
- Add background workers for async workflows (e.g., Notification dispatch, Reporting projections).
- Introduce automated migrations (EF Core tools) per service in their infrastructure projects.

Feel free to open issues or extend services with richer domain logic, but keep the established structure so the platform remains consistent.
