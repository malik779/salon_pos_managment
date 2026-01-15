## Architecture Overview

The platform adopts a domain-oriented microservice architecture: each bounded-context service encapsulates its own persistence, domain model, and API surface while collaborating via events on RabbitMQ and cache coordination through Redis. CQRS and MediatR orchestrate command/query separation within each .NET service.

### High-Level Runtime View
1. **Clients**: Angular admin PWA, Flutter POS/mobile, public booking widget, and third-party partners (OpenAPI + OAuth scopes).
2. **Gateway**: Azure Application Gateway/NGINX Ingress terminating TLS, forwarding to backend services via service mesh (mTLS enforced).
3. **Services**: Independently deployable ASP.NET Core containers, each with EF Core migrations targeting a shared SQL Server cluster (per-schema or per-database per service).
4. **Async Fabric**: RabbitMQ exchanges for domain events, Redis for caching, distributed locks, and offline sync metadata.
5. **Observability**: OpenTelemetry exporters, Serilog to Seq/Elastic, Prometheus metrics, Grafana dashboards, Alertmanager hooks.

### Service Responsibilities

| Service | Core Responsibilities | Persistence | Key APIs | Events (Pub/Sub) | Tests & Notes |
| --- | --- | --- | --- | --- | --- |
| **Identity** | Auth (OIDC/OAuth2), RBAC, device registration, token issuance, branch-scoped claims | SQL: Users, Roles, Claims, Devices; Redis for session tokens | `/api/v1/auth/token`, `/auth/refresh`, `/users`, `/roles`, `/devices/register` | Publishes `UserCreated`, `UserRoleChanged`; Subscribes to `BranchCreated` | Unit tests for token flows, integration tests for device registration |
| **Branch** | Branch metadata, hours, receipt templates, availability rules | SQL: Branches, BranchSettings | `/branches`, `/branches/{id}/settings` | Publishes `BranchCreated`, `BranchUpdated`; Subscribes to `DayClosed` | Contract tests with Booking & POS |
| **Catalog** | Services, products, packages, pricing overrides, stock snapshots | SQL: Services, Products, SKUs, PricingOverrides | `/catalog/services`, `/catalog/products`, `/catalog/stock` | Publishes `ProductStockChanged`; Subscribes to `InvoiceCreated` | Inventory integration tests, seed data scripts |
| **Booking** | Appointment lifecycle, availability engine, cancellations, notifications | SQL: Appointments, AppointmentItems, BookingSources; Redis for slot locks | `/bookings`, `/calendar`, `/bookings/{id}/status` | Publishes `AppointmentCreated/Cancelled/Rescheduled`; Subscribes to `ClientCreated`, `StaffScheduleChanged` | Simulation tests for double-booking prevention |
| **POS** | Cart, invoice creation, Z-read, petty cash, invoice read models | SQL: Invoices, InvoiceLines, TillSessions | `/invoices`, `/invoices/{id}`, `/closereads` | Publishes `InvoiceCreated`, `DayClosed`; Subscribes to `PaymentCaptured`, `StockLevelChanged` | Integration tests for invoice lifecycle and daily close |
| **Payments** | Gateway abstraction, payment intents, refunds, settlement webhooks | SQL: Payments, Refunds, GatewayTokens | `/payments`, `/payments/refund`, `/payments/webhook` | Publishes `PaymentCaptured`, `PaymentRefunded`; Subscribes to `InvoiceCreated` | Webhook signature verification tests |
| **Staff** | Staff profiles, schedules, attendance, commission rules | SQL: Staff, Shifts, AttendanceRecords, CommissionRules | `/staff`, `/staff/{id}/commissions`, `/attendance/checkin` | Publishes `StaffScheduleChanged`, `CommissionCalculated`; Subscribes to `InvoicePaid` | Time clock integration tests |
| **Client** | CRM data, visits, memberships, consents, packages | SQL: Clients, ClientConsents, Memberships, PackageBalances | `/clients`, `/clients/{id}/visits`, `/clients/{id}/packages` | Publishes `ClientCreated`, `MembershipUpdated`; Subscribes to `InvoiceCreated`, `AppointmentCreated` | GDPR export tests |
| **Notifications** | Template engine, channels (SMS, email, push), retries | SQL: NotificationQueue, Templates, DeliveryStatus; Redis delay queues | `/notifications/send`, `/templates` | Publishes `NotificationSent`; Subscribes to `AppointmentCreated`, `InvoicePaid` | Load tests for fan-out |
| **Reports** | Aggregated KPIs, dashboards, precomputed read models | SQL DW schema (star) + OLAP cubes | `/reports/kpis`, `/reports/commissions` | Subscribes to `InvoicePaid`, `DayClosed`, `CommissionCalculated` | Data consistency tests |
| **Sync** | Device provisioning, offline queue ingestion, conflict resolution | SQL: DeviceSyncQueue, DeviceStates; Redis for sequence tokens | `/sync/register`, `/sync/push`, `/sync/pull` | Publishes `SyncOperationApplied`; Subscribes to all critical events for delta feeds | Offline simulation tests |
| **Audit** | Centralized audit log ingestion, search, export | SQL/Elastic: AuditEvents, ExportJobs | `/audit/events`, `/audit/export` | Subscribes to audit envelopes from pipeline behavior | Tamper-proofing checks |

### Shared Platform Components
- **`services/common` library**: Contracts, base MediatR behaviors (Validation, Transaction, Audit, CacheInvalidation, Retry), idempotency helpers, tracing utilities.
- **Event Envelope**: `{ eventId, type, occurredAtUtc, correlationId, source, payload }` serialized as JSON with schema registry (JSON Schema).
- **Configuration**: `appsettings.{Environment}.json` + environment overrides, secrets pulled from Vault/Azure Key Vault.

### Data & Idempotency
- All write APIs require `Idempotency-Key` header; middleware stores hash in Redis with TTL to guarantee at-least-once safety.
- EF Core migrations stored per service under `Migrations/` with automated seeding scripts triggered via `/scripts/seed.ps1`.
- Cross-service references use UUIDs; no cross-schema foreign keys to preserve autonomy.

### Messaging & Integration
- RabbitMQ exchanges: `booking.events`, `pos.events`, `payments.events`, `sync.events`, `audit.events`.
- Retry strategy: publish-confirm, outbox pattern via `OutboxMessages` table processed by background worker.
- Contract tests (Pact) ensure Angular/mobile clients stay aligned with OpenAPI definitions generated during CI.

### Pipeline Behaviors (MediatR)
1. **ValidationBehavior** → FluentValidation ensures commands/queries meet invariants.
2. **TransactionBehavior** → Wraps EF Core DbContext transaction when command mutates state.
3. **AuditBehavior** → Emits audit envelope to Audit service asynchronously.
4. **CacheInvalidationBehavior** → Clears Redis keys derived from metadata on command attributes.
5. **RetryPolicyBehavior** → Polly-based transient fault handling for SQL/RabbitMQ interactions.

### API Standards
- Versioned routes `/api/v1/...`, JSON:API-inspired responses with `data`, `meta`, `links`.
- Problem details (`application/problem+json`) for errors, consistent error codes documented in OpenAPI.
- Authentication via OAuth client credentials (services) or auth code with PKCE (frontends); tokens include branch scope claim to limit access.
- Generated TypeScript/Flutter API clients stored under `frontend/src/app/core/api` and `mobile/lib/api`.

### Offline & Sync Strategy
- Flutter app maintains local SQLite store with operation queue; each operation carries `{opId, idempotencyKey, entityType, payload}`.
- `/sync/push` accepts batches, validates signatures, writes to `DeviceSyncQueue`, and replays via internal command bus.
- Conflict policy examples:
  - **Bookings**: server rejects conflicting slots and returns `409` with alternate suggestions generated by availability engine.
  - **Invoices**: duplicates flagged; Payments service initiates refund automatically if double charge detected.
- Background sync triggered every 5 minutes or on connectivity changes; manual "Sync Now" button surfaces last successful timestamp.

### Observability, Security & Compliance
- **Observability**: trace IDs propagated via `traceparent` header; dashboards for booking lead time, POS throughput, sync backlog.
- **Security**: TLS 1.3, HSTS, strict CSP on frontend, gateway-side rate limiting for public booking APIs, periodic pen-tests (OWASP ASVS alignment).
- **Compliance**: GDPR tooling (export/delete), encrypted PII at rest via SQL Server column encryption, audit trail immutability through append-only log.

### DevEx & CI/CD
- GitHub Actions pipeline stages: `lint` → `unit` → `integration` → `contract-tests` → `build-images` → `security-scan` → `deploy-dev`.
- `run-locally.sh` (coming soon) will orchestrate Docker Compose with SQL Server, Redis, RabbitMQ, Identity, and sample services + seed data.
- Coding standards enforce SOLID, vertical slice architecture, and reuse-first mindset (shared API clients, validation components, UI primitives).

### Next Documentation Deliverables
1. Sequence diagrams for booking confirmation and POS payment/posting.
2. Runbooks: backup/restore, scaling, incident response, and daily close procedure.
3. Detailed data dictionary with column metadata and indexing strategy.
