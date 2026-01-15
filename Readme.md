Add me
# Salon POS Monorepo

Salon POS is a polyglot monorepo that hosts every deployable for the platform:

- `services/*` – backend bounded-contexts (Identity, Branch, Catalog, Booking, POS, Payments, Staff, Client, Notifications, Reports, Sync, Audit) implemented with ASP.NET Core, EF Core, MediatR CQRS, and RabbitMQ messaging via the shared BuildingBlocks package.
- `frontend/` – Angular web SPA.
- `mobile/` – Flutter client for offline-first POS.
- `docs/` – architecture and decision records.
- `run-locally.sh` / `docker-compose.dev.yml` – spin up the full stack with SQL Server, Redis, RabbitMQ, and representative services.

> **Important:** The Codespace/CI environment used for this repository does **not** provide the `dotnet` CLI. The backend source compiles in a normal .NET 9 SDK but you cannot run `dotnet build` inside the hosted environment. Use Docker-based workflows (below) or run the solution on your workstation.

---

## 1. Prerequisites

| Tool | Purpose |
| --- | --- |
| Docker + Docker Compose v2 | Run the local stack (`run-locally.sh`) |
| Node.js 20 + pnpm | Develop the Angular front-end |
| Flutter 3.x | Develop the mobile client |
| .NET 9 SDK | Build/run backend services outside containers |

Optional: RedisInsight, RabbitMQ management UI, or Azure Data Studio for debugging containers.

---

## 2. Running the whole platform locally

```bash
./run-locally.sh
```

This script wraps `docker compose -f docker-compose.dev.yml up --build`, starting:

- SQL Server, Redis, RabbitMQ
- Identity + Booking + POS services (extend `docker-compose.dev.yml` to add the rest)
- Any additional tooling defined in the compose file

> Logs stream in the same terminal. Press `Ctrl+C` to stop everything; Docker removes the containers but leaves named volumes with persisted data.

### Customizing the stack

```
PROJECT_NAME=my-salon COMPOSE_FILE=docker-compose.light.yml ./run-locally.sh
```

- Use `PROJECT_NAME` to isolate containers/volumes per developer.
- Provide an alternate compose file to run only a subset of services.

---

## 3. Backend services

Each service under `services/<Context>` follows the same layout:

```
src/
  Api/             -> Minimal API endpoints, authentication/authorization glue
  Application/     -> CQRS handlers, validators, MediatR commands/queries
  Domain/          -> Aggregate roots and EF-friendly entities
  Infrastructure/  -> EF Core DbContext, repositories, messaging adapters
Program.cs         -> Wires ServiceDefaults + BuildingBlocks extensions
```

Shared packages:

- `services/Shared/ServiceDefaults` – Serilog, health checks, Swagger, default middleware.
- `services/Shared/BuildingBlocks` – MediatR pipeline behaviors, EF helpers, RabbitMQ publisher/consumer infrastructure, retry policies.
- `services/Shared/Contracts` – integration-event contracts (e.g., `InvoicePaidIntegrationEvent`).

### Local configuration

1. Add `appsettings.Development.json` per service with a `ConnectionStrings` section (SQL Server) and `RabbitMQ` credentials. The defaults use SQL Server + RabbitMQ hosts from `docker-compose.dev.yml`.
2. When no connection string is present, services fall back to EF Core InMemory (handy for unit tests, but **not** a persistent store).
3. Services publish/consume events through RabbitMQ topic exchange `salon-pos`.

### Manual service launch (outside Docker)

```bash
cd services/Branch
dotnet restore
dotnet run --project BranchService.csproj
```

Remember to export the same environment variables you pass through Docker (connection strings, RabbitMQ, ASPNETCORE_URLS, etc.).

---

## 4. Frontend (Angular)

```
cd frontend
pnpm install
pnpm start          # serves at http://localhost:4200
pnpm test           # Jest unit tests
pnpm run e2e        # Cypress end-to-end (expects docker stack running)
```

Key config files: `angular.json`, `tsconfig*.json`, `.eslintrc.json`, `cypress.config.ts`.

---

## 5. Mobile (Flutter)

```
cd mobile
flutter pub get
flutter run
```

Use `lib/env.dart` to point at your local backend (Identity + Booking + POS + Sync). The offline queue lives in `lib/core/storage/offline_queue.dart`.

---

## 6. Common tasks

| Task | Command |
| --- | --- |
| Lint frontend | `pnpm lint` |
| Format frontend | `pnpm format` |
| Run Cypress booking flow | `pnpm run e2e -- --spec cypress/e2e/booking.cy.ts` |
| Smoke-test mobile | `flutter test test/smoke_test.dart` |
| Seed databases | Extend `docker-compose.dev.yml` with seed jobs or run SQL scripts manually |

---

## 7. Troubleshooting

- **SQL Server container fails to start**: ensure you allocate ≥ 4GB RAM to Docker Desktop and that port `1433` is available.
- **RabbitMQ connection errors**: the shared BuildingBlocks expect the host `rabbitmq` (Docker service name). Override via `RabbitMQ__HostName` env var.
- **dotnet CLI missing**: run services inside Docker until you install the .NET 9 SDK locally.
- **Front-end CORS**: `ServiceDefaults` registers a permissive `default` CORS policy for dev; make sure services call `UseServiceDefaults()` before mapping endpoints.

---

## 8. Contributing

1. Branch from `main` (e.g., `cursor/refactor-salon-pos-monorepo-*`).
2. Keep backend changes within the service boundary; reuse shared BuildingBlocks or add new primitives there.
3. Update docs in `docs/architecture.md` for architectural decisions.
4. Run lint/tests relevant to your change (frontend, mobile, or targeted backend tests).
5. Submit PR with summary + test evidence.

Happy hacking!