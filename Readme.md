## Salon/SPA Platform Monorepo

This repository hosts the multi-channel Salon/SPA management platform that powers bookings, POS, staff operations, and analytics across web and mobile channels.

### Tech Stack Guardrails
- **Backend services**: .NET 10, ASP.NET Core, MediatR (CQRS), EF Core, SQL Server, Redis, RabbitMQ, Serilog, FluentValidation, AutoMapper.
- **Frontend admin PWA**: Angular 20+, NgRx, Angular Material/Tailwind, generated API clients.
- **Mobile apps**: Flutter (shared business/UI widgets, offline SQLite, Bluetooth printing integration layer).
- **Observability & Ops**: OpenTelemetry, Seq/Grafana, GitHub Actions CI, Docker + Helm for deployments.

### Repository Layout
- `services/*`: microservices by bounded contexts (Identity, Branch, Catalog, Booking, POS, Payments, Staff, Client, Notifications, Reports, Sync, Audit).
- `frontend/`: Angular admin/PWA workspace.
- `mobile/`: Flutter workspace for POS/front-desk experience.
- `infra/`: Docker Compose, Helm charts, GitHub Actions, runbooks.
- `scripts/`: automation (seeders, codegen, tooling).
- `docs/`: architecture, milestones, diagrams, API references.

### Current State
Documentation scaffolding and directory layout are in place. Next milestones:
1. Finalize high-level architecture & service contracts (see `docs/architecture/overview.md`).
2. Define phased delivery roadmap with testing/compliance checkpoints (see `docs/milestones/roadmap.md`).
3. Bootstrap Identity + Branch services and shared libraries, then expand outward per roadmap.

### Contributing
1. Ensure .NET 10 SDK, Node 20+, Flutter, Docker, and Azure Data Studio/SQL tooling are installed.
2. Follow the coding standards in `docs/architecture/overview.md` (CQRS, idempotency, validation, audit).
3. All changes require unit/integration tests plus updated OpenAPI schemas and docs.
4. Use semantic commits, create PRs with self-review notes, and keep services backward compatible.
