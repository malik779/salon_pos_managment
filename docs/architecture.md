# Salon POS, Booking & Staff Management Platform

## 1. Vision & Scope
The platform targets multi-branch salons that require unified booking, POS, staff management, CRM, and compliance tooling. It is designed as a microservice-oriented monorepo that supports horizontal scale, modular deployments, and extensibility for regional payment providers, notification channels, and offline-first mobile behavior.

Key traits:
- **Multi-branch aware**: branch-scoped RBAC, branch-aware scheduling, localized receipts, timezone-aware reporting.
- **Omnichannel booking**: internal calendar + embeddable widget + public API, synced with mobile devices.
- **Enterprise-grade POS**: mixed carts, multiple tenders, discounts, Z-read, Bluetooth receipts, offline queues.
- **Staff productivity**: scheduling, commission rules, attendance, performance dashboards.
- **Client engagement**: CRM, memberships, consents, workflows for reminders & follow-ups.
- **Secure & compliant**: OAuth2/OIDC, audit trails, GDPR tooling, secure secret management.

## 2. Monorepo Layout
```
/services
  /Identity
  /Branch
  /Catalog
  /Booking
  /POS
  /Payments
  /Staff
  /Client
  /Notifications
  /Reports
  /Sync
  /Audit
/frontend
/mobile
/infra
/scripts
/docs
```
Shared assets:
- **/docs**: architectural decision records, API contracts, onboarding guides.
- **/infra**: Dockerfiles, docker-compose, Helm charts, GitHub Actions templates, k8s manifests.
- **/scripts**: developer tooling, `run-locally.sh`, data seeding helpers.

## 3. Technology Stack
- **Backend**: .NET 9, ASP.NET Core Web API, MediatR (CQRS), EF Core, SQL Server (provider-agnostic), StackExchange.Redis, AutoMapper, FluentValidation, Serilog, OpenTelemetry.
- **Async messaging**: RabbitMQ (events, sagas). Redis used for caching, locking, offline sync staging.
- **Frontend**: Angular 20+, RxJS, NgRx signal stores, Angular Material + Tailwind, Jest unit tests, Cypress E2E.
- **Mobile**: Flutter (for cross-platform speed, Bluetooth plugin support), uses same backend APIs. SQLite for offline queue, native channels for hardware.
- **CI/CD**: GitHub Actions building Docker images per service; Helm charts for Kubernetes deploy; Docker Compose for local dev.

## 4. Cross-Cutting Architecture
- **API Gateway (future)**: optional reverse proxy (YARP) for routing and auth offload.
- **Authentication**: Identity service issues OAuth2/OIDC tokens (Duende IdentityServer). Frontend/mobile obtains tokens and refresh tokens; device registration flows for POS terminals.
- **RBAC**: Branch-scoped claims, role hierarchy (Owner > Manager > Cashier > Therapist). Policies enforced via ASP.NET authorization handlers.
- **Observability**: Serilog sinks to Seq/Elastic; OpenTelemetry traces to Jaeger/Tempo; Prometheus metrics via exporters.
- **Caching**: Redis per service for short-lived caches (catalog, availability). Cache invalidation via MediatR pipeline behavior and events.
- **MediatR Pipeline Behaviors**:
  - `ValidationBehavior`: runs FluentValidation.
  - `TransactionBehavior`: wraps commands in EF Core transactions per request.
  - `AuditBehavior`: publishes audit events to Audit service.
  - `CacheInvalidationBehavior`: clears dependent cache entries.
  - `RetryPolicyBehavior`: Polly-based retries for transient messaging failures.

## 5. Service Design Details
Each service is an independent ASP.NET Core Web API with its own database schema (per-service DB). Communication is primarily async events via RabbitMQ plus REST APIs for synchronous flows.

### 5.1 Identity & Access Service (services/Identity)
- **Responsibilities**: user lifecycle, role assignments, branch scope mapping, OAuth2/OIDC tokens, refresh tokens, device registration, biometric flagging.
- **DB Tables**: `Users`, `Roles`, `UserRoles`, `UserClaims`, `Devices`, `DeviceSecrets`.
- **APIs**: `/auth/token`, `/auth/refresh`, `/users`, `/roles`, `/devices/register`, `/devices/revoke`.
- **Events**: `UserCreated`, `UserUpdated`, `UserRoleChanged`, `DeviceRegistered`.
- **Tests**: token issuance, RBAC policy enforcement, device revocation.

### 5.2 Branch & Config Service (services/Branch)
- **Responsibilities**: branch metadata, tax configs, timezones, receipt templates, booking windows, public booking settings.
- **DB Tables**: `Branches`, `BranchSettings`, `BusinessHours`, `ReceiptTemplates`.
- **APIs**: `/branches`, `/branches/{id}`, `/branches/{id}/settings`, `/branches/{id}/business-hours`.
- **Events**: `BranchCreated`, `BranchUpdated`, `ReceiptTemplateChanged`.
- **Tests**: branch CRUD, timezone conversions, template overrides.

### 5.3 Catalog Service (services/Catalog)
- **Responsibilities**: services, products, packages, SKUs, pricing tiers, inventory snapshots, supplier references.
- **DB Tables**: `Services`, `ServicePricing`, `Products`, `SKUs`, `Packages`, `PricingOverrides`, `InventoryTransactions`.
- **APIs**: `/catalog/services`, `/catalog/products`, `/catalog/packages`, `/catalog/stock`, `/catalog/inventory/adjustments`.
- **Events**: `ProductStockChanged`, `ServiceUpdated`, `PackageBalanceChanged`.
- **Tests**: pricing tier calculations, inventory adjustments, SKU merges.

### 5.4 Booking & Calendar Service (services/Booking)
- **Responsibilities**: appointment lifecycle, availability engine, idempotent booking creation, recurring rules, notifications triggers, distributed locking.
- **DB Tables**: `Appointments`, `AppointmentItems`, `BookingSources`, `AvailabilityOverrides`, `Waitlists`.
- **APIs**:
  - `POST /bookings` (requires `Idempotency-Key` header).
  - `GET /calendar?branchId=&from=&to=` multi-view (day/week/month) support.
  - `PATCH /bookings/{id}/status` (confirm/cancel/reschedule).
  - `POST /bookings/{id}/notes` for staff notes.
- **Events**: `AppointmentCreated`, `AppointmentCancelled`, `AppointmentRescheduled`, `AvailabilityUpdated`.
- **Concurrency**: Redis distributed locks per branch-staff-time slot; idempotency tokens stored in Redis cluster.
- **Tests**: overlapping booking prevention, recurring schedule expansion, event emission.

### 5.5 POS & Transactions Service (services/POS)
- **Responsibilities**: carts, invoices, payment orchestration, refunds, Z-read close, petty cash, tax handling.
- **DB Tables**: `Invoices`, `InvoiceLines`, `Payments` (shadow table), `Refunds`, `TillSessions`, `DiscountsApplied`, `GiftCards`.
- **APIs**:
  - `POST /invoices` (draft vs finalize flows).
  - `POST /payments` (idempotent, references invoice or deposit).
  - `POST /invoices/{id}/refund`.
  - `POST /closereads` (daily Z-read close with branch + till session).
  - `POST /tills/{id}/cash-movements` for petty cash.
- **Events**: `InvoiceCreated`, `InvoiceFinalized`, `InvoicePaid`, `PaymentRefunded`, `DayClosed`.
- **Read Models**: aggregated `DailyTotals` per branch/day, `OutstandingInvoices`.
- **Tests**: mixed cart totals, Z-read calculations, petty cash reconciliation.

### 5.6 Payments Gateway Adapter Service (services/Payments)
- **Responsibilities**: abstract payment gateway operations, tokenize, handle webhooks, offline reconciliation, adapters (Stripe, Adyen, local e-wallets).
- **DB Tables**: `PaymentIntents`, `PaymentSources`, `WebhookEvents`, `ReconciliationBatches`.
- **APIs**: `POST /payments/charge`, `POST /payments/webhook`, `POST /payments/tokenize`, `GET /payments/{id}`.
- **Events**: `PaymentAuthorized`, `PaymentCaptured`, `PaymentFailed`.
- **Tests**: HMAC validation, adapter contract tests, webhook retries.

### 5.7 Staff & Commission Service (services/Staff)
- **Responsibilities**: staff profiles, shift rosters, attendance, commission engine, leave management.
- **DB Tables**: `Staff`, `StaffBranches`, `Shifts`, `AttendanceRecords`, `CommissionRules`, `Commissions`, `Timesheets`.
- **APIs**: `/staff`, `/staff/{id}`, `/staff/{id}/commissions`, `/attendance/checkin`, `/attendance/checkout`, `/shifts`.
- **Events**: `StaffCreated`, `ShiftPublished`, `AttendanceLogged`, `CommissionCalculated`.
- **Tests**: commission tiers, overtime calculations, attendance edge cases.

### 5.8 Client Management Service (services/Client)
- **Responsibilities**: client profiles, visits history, memberships, packages, consents, notes, GDPR exports.
- **DB Tables**: `Clients`, `ClientContacts`, `ClientVisits`, `ClientConsents`, `Memberships`, `PackageBalances`, `ClientNotes`.
- **APIs**: `/clients`, `/clients/{id}`, `/clients/{id}/visits`, `/clients/{id}/packages`, `/clients/{id}/consents`, `/clients/{id}/gdpr/export`.
- **Events**: `ClientCreated`, `ClientUpdated`, `ConsentCaptured`, `MembershipStatusChanged`.
- **Tests**: consent versioning, package redemption, GDPR anonymization.

### 5.9 Notification Service (services/Notifications)
- **Responsibilities**: message templates, provider adapters (Twilio, SendGrid, WhatsApp), queue & retry, reminder scheduling.
- **DB Tables**: `NotificationQueue`, `NotificationTemplates`, `DeliveryStatus`, `ProviderCredentials`.
- **APIs**: `/notifications/send`, `/notifications/bulk`, `/templates`, `/providers`.
- **Events**: `NotificationDispatched`, `NotificationFailed`, `ReminderDue`.
- **Tests**: template rendering, retry policies, rate limit handling.

### 5.10 Reporting & Analytics Service (services/Reports)
- **Responsibilities**: pre-computed KPIs, staff performance dashboards, membership usage, Z-read history.
- **DB Tables**: star-schema style `FactSales`, `FactBookings`, `FactStaffPerformance`, `DimBranch`, `DimDate`, `DimStaff`.
- **APIs**: `/reports/kpis`, `/reports/commissions`, `/reports/branches/{id}/daily`, `/reports/export`.
- **Events Consumed**: almost all domain events for ETL ingestion; publishes `ReportReady` if long-running.
- **Tests**: aggregation correctness, filtering, permission gating.

### 5.11 Sync / Offline Service (services/Sync)
- **Responsibilities**: device registration, sync tokens, offline queue intake, conflict resolution policies, background replay.
- **DB Tables**: `DeviceStates`, `DeviceSyncQueue`, `SyncSequences`, `OfflineConflicts`.
- **APIs**: `/sync/register`, `/sync/push`, `/sync/pull`, `/sync/conflicts/{id}`.
- **Events**: `DeviceSyncCompleted`, `OfflineConflictDetected`.
- **Behavior**:
  1. Device registers (identity-provided token + hardware fingerprint) → receives sync token (TTL).
  2. Mobile stores operations locally with idempotency key.
  3. `/sync/push` accepts batch + last sequence; service assigns server sequences and relays to target services via commands/events.
  4. `/sync/pull` streams changes using per-device watermark + delta feed from Redis/SQL.

### 5.12 Audit & Compliance Service (services/Audit)
- **Responsibilities**: centralized audit log storage, GDPR export/anonymize, retention policies, tamper-proof log (e.g., append-only table + hash chain).
- **DB Tables**: `AuditEvents`, `DataAccessLogs`, `ComplianceExports`.
- **APIs**: `/audit/search`, `/audit/events/{id}`, `/audit/exports`, `/audit/purge`.
- **Events**: consumes `AuditEvent` from all services; publishes `ExportReady` for asynchronous GDPR export completion.
- **Tests**: tamper proofing, retention cleanup, export filtering.

## 6. Data Model Highlights (Canonical)
Core normalized schemas (simplified):
- `Branches(BranchId PK, Name, Timezone, Address, DefaultTax)`
- `Users(UserId PK, Email, FullName, PasswordHash, IsActive)`
- `Roles(RoleId PK, Name)`
- `Staff(StaffId PK, UserId FK, DefaultBranchId, Role)`
- `Services(ServiceId PK, Name, DurationMinutes, BasePrice, Category)`
- `Products(ProductId PK, SKU, Name, Price, InventoryQty, TaxCode)`
- `Clients(ClientId PK, FirstName, LastName, Phone, Email, ConsentJson)`
- `Appointments(AppointmentId PK, BranchId, ClientId, StaffId, StartUtc, EndUtc, Status, Source, CreatedBy)`
- `Invoices(InvoiceId PK, BranchId, ClientId, Amount, Tax, Discount, Total, Status, CreatedAt)`
- `InvoiceLines(InvoiceLineId PK, InvoiceId FK, ItemType, ItemRefId, Qty, UnitPrice, DiscountApplied)`
- `Payments(PaymentId PK, InvoiceId FK, Method, ProviderRef, Amount, Status, CreatedAt)`
- `Commissions(CommissionId PK, StaffId FK, InvoiceLineId FK, CommissionType, Amount, Tier)`

Relationships enforced via EF Core with schema-per-service, while Reporting service uses consolidated read model star schema.

## 7. CQRS + MediatR Patterns
Sample commands & queries (per bounded context):
- **Commands**: `CreateBookingCommand`, `RescheduleBookingCommand`, `CompleteInvoiceCommand`, `CreatePaymentIntentCommand`, `CloseDayCommand`, `RegisterDeviceSyncCommand`, `CaptureConsentCommand`.
- **Queries**: `GetCalendarQuery`, `GetInvoiceQuery`, `GetStaffPerformanceQuery`, `GetClientProfileQuery`, `ListBranchConfigsQuery`.

Pipeline behaviors (applied in each service):
1. `ValidationBehavior<TRequest,TResponse>` – executes FluentValidation validators before handlers.
2. `TransactionBehavior<TRequest,TResponse>` – opens EF Core transaction for commands touching DB.
3. `AuditBehavior<TRequest,TResponse>` – generates audit DTO forwarded to Audit service.
4. `CacheInvalidationBehavior<TRequest,TResponse>` – invalidates Redis keys tied to aggregates.
5. `RetryPolicyBehavior<TRequest,TResponse>` – Polly policy for transient errors (e.g., RabbitMQ publish).

## 8. Offline & Sync Strategy (Mobile)
1. **Device registration**: device submits `deviceId`, `publicKey`, branch & role hints → Identity issues token + Sync service gives sync token with TTL.
2. **Local DB**: Flutter app uses SQLite with `OperationsQueue`, `PendingReceipts`, `SyncMetadata` tables.
3. **Operation queue**: every offline action stores payload + client-generated idempotency key; On reconnection, app batches operations.
4. **Sync push**: `/sync/push` accepts batch, ensures idempotency, dispatches to respective services via commands. Response includes server sequence numbers + conflicts.
5. **Conflict resolution**:
   - Bookings: server authoritative; returns conflict with alternate slots or forced reschedule suggestions.
   - Invoices: duplicates flagged, automatic refund/void flows triggered.
6. **Background sync**: triggered by connectivity gain, manual refresh, or scheduled intervals. UI surfaces per-operation status + global offline banner.
7. **Security**: tokens stored in secure keystore, optional biometric gating for high-privilege actions, offline access limited via policy (e.g., max 24h offline token).

## 9. Frontend (Angular) Architecture
- **Structure**: `src/app/{core,auth,dashboard,branches,catalog,booking,pos,clients,staff,reports,settings}`.
- **State management**: NgRx signal store + entity adapters for normalized data; feature slices per domain service. Effects call typed API clients generated from OpenAPI specs.
- **Core services**: `ApiHttpInterceptor` attaches OAuth token, `IdempotencyInterceptor` attaches `Idempotency-Key` (uuid), `ErrorHandler` maps backend errors to UX.
- **Real-time**: SignalR hub connection for appointment updates, invoice payments, sync notifications.
- **Testing**: Jest for unit tests, Cypress for booking/POS flows. Storybook (optional) for UI components.
- **Offline awareness**: global service listens to Sync notifications for device status; POS module exposes offline cart UI.

## 10. Mobile (Flutter) Architecture
- **Modules**: `core`, `auth`, `booking`, `pos`, `clients`, `staff`, `sync`, `printing`.
- **State**: Riverpod or Bloc with hydrated storage; offline queue stored in `sqflite` DB.
- **Printing**: platform channel wrappers for Bluetooth ESC/POS printers, fallback to PDF for network printers.
- **Security**: tokens stored via `flutter_secure_storage`, biometric unlock for cashier check-ins.
- **Testing**: Flutter unit tests + widget tests for booking and POS screens; integration tests for offline sync flows.

## 11. Notifications & Reporting Pipelines
- Booking service publishes `AppointmentCreated` → Notification service schedules reminder (SMS/email) using templates that pull branch-specific content.
- POS service emits `InvoicePaid` → Notification service sends digital receipt with PDF link (hosted S3/blob) + triggers Reporting service aggregator update.
- Staff service emits `CommissionCalculated` → Reporting service updates dashboards, Notification service optionally pushes summary to managers.

## 12. Security & Compliance Checklist
- Enforce TLS + HSTS, secure cookies, SPA uses PKCE.
- Rate-limit public booking endpoints; WAF rules for injection, path traversal, etc.
- Secrets in Vault/Azure KeyVault; `appsettings.json` uses placeholder environment variables.
- Audit every write command; provide GDPR export + deletion flows.
- Logging scrubs PII where unnecessary; encryption at rest for sensitive tables (consents, payment tokens).

## 13. DevOps & Operations
- **Docker**: multi-stage builds for each service; base image `mcr.microsoft.com/dotnet/aspnet:9.0`.
- **docker-compose.dev.yml**: composes SQL Server, Redis, RabbitMQ, Identity service, and sample Booking/POS services for integration testing.
- **GitHub Actions**: workflow per service building/pushing Docker image, running unit tests, publishing coverage.
- **Kubernetes**: `infra/k8s` holds Helm charts; secrets referenced via External Secrets operator; horizontal pod autoscaler config per service.
- **Database migrations**: EF Core migrations per service + shared seeding scripts in `/scripts/migrations`.
- **Observability stack**: Prometheus + Grafana dashboards, Loki or Elastic for logs, Tempo/Jaeger for traces.
- **Backups**: automated SQL backups to object storage (Azure Blob/S3), retention policies documented.

## 14. Testing Strategy
- **Backend**: xUnit per service with unit + integration tests (testcontainers for SQL/Redis). Contract tests for event schemas.
- **Frontend**: Jest + Testing Library for components; Cypress running against docker-compose stack for booking/POS flows.
- **Mobile**: Flutter unit/widget tests; integration tests hitting QA backend; offline sync tests simulate network loss.
- **Performance**: k6 scripts for POS peak hours, booking concurrency, sync throughput.
- **Security**: automated SAST/DAST, dependency scanning, OWASP Zap in CI.

## 15. Next Steps
1. Flesh out individual service templates (ASP.NET Core project scaffolding, MediatR setup, tests).
2. Define shared packages (e.g., `BuildingBlocks` containing abstractions for messaging, auditing, exceptions).
3. Generate OpenAPI specs per service, feed Angular/Flutter API clients.
4. Implement data seeding scripts and sample fixtures for demo branches, staff, catalog items.
5. Build CI pipelines, Helm charts, and infrastructure automation (Terraform optional).
