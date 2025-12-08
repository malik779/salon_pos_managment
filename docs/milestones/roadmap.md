## Delivery Roadmap

The program is organized into iterative milestones. Each phase ends with a PR that includes code, tests, infra assets, and documentation updates.

### Phase 0 – Foundations (Week 1)
- Finalize architecture (this document), coding standards, branching strategy.
- Bootstrap shared `.NET` solution (`services/SalonPlatform.sln`) with common libraries (contracts, MediatR behaviors, Redis helpers).
- Set up GitHub Actions templates, Docker base images, and pre-commit hooks.
- Exit criteria: CI green on hello-world services, docs approved.

### Phase 1 – Identity, Branch, Shared Libs (Weeks 2-3)
- Implement Identity service (OAuth flows, RBAC, device registration) with SQL Server + Redis.
- Implement Branch service with branch metadata APIs and events.
- Deliver shared OpenAPI generator + TypeScript/Flutter clients.
- Provide seed data scripts and local Docker Compose with SQL/Redis/RabbitMQ/Identity/Branch.
- Tests: unit (xUnit), integration (token issuance, branch CRUD), contract tests for API clients.
- Exit criteria: Angular app authenticates against Identity; services emit/consume events.

### Phase 2 – Catalog & Client CRM (Weeks 3-4)
- Build Catalog service (services/products/packages, stock events) and Client service (CRM, memberships, consents).
- Implement migration + seeding pipelines, attach contract tests.
- Extend Angular NgRx slices for catalog & client entities.
- Exit criteria: admin UI manages catalog items, API clients auto-generated in CI.

### Phase 3 – Booking Service & Notifications (Weeks 5-6)
- Implement booking command/query handlers (slot calc, idempotency, conflict detection).
- Integrate Redis locks, publish appointment events, trigger Notification service.
- Create Angular calendar UI + Cypress tests; Flutter booking creation for front desk.
- Exit criteria: End-to-end booking flow (API + UI) with SMS/email reminders.

### Phase 4 – POS, Payments, Staff Commission (Weeks 7-9)
- Deliver POS service (invoice lifecycle, Z-read) and Payments adapters (Stripe sandbox + mock gateway).
- Wire Staff service for schedules and commission calculations.
- Implement offline-ready POS view in Flutter (draft invoices, sync push/pull).
- Tests: invoice/payment integration tests, commission calculation unit tests, POS Cypress suite.
- Exit criteria: Service+product cart checkout, payment capture, and commission ledger.

### Phase 5 – Reporting, Sync Hardening, Audit (Weeks 10-11)
- Reporting service builds aggregates for dashboards; Angular reports module.
- Sync service handles device queue reconciliation, conflict resolution policies.
- Audit service ingests envelopes from MediatR pipeline.
- Exit criteria: Offline device successfully syncs bookings/invoices; audit queries export CSV.

### Phase 6 – Hardening & Operations (Weeks 12-13)
- Run performance, load, and failover drills; implement scaling & backup runbooks.
- Add observability dashboards, alerting rules, security scans (SAST/DAST).
- Finalize documentation (runbooks, architecture diagrams, OpenAPI, onboarding guides).
- Exit criteria: `run-locally.sh` boots representative stack, CI/CD promotion to staging green, pen-test gaps addressed.

### Ongoing Activities
- **Security & Compliance**: periodic secret rotation drills, dependency scans.
- **Data Quality**: nightly validation jobs, reconciliation reports.
- **Release Cadence**: bi-weekly releases with feature flags and blue/green deploys.
