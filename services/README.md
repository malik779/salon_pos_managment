# Salon POS Microservices Solution

This solution contains all microservices for the Salon POS Management system, properly configured for Visual Studio development.

## Solution Structure

### Shared Projects
- **ServiceDefaults** - Common service configuration and defaults
- **BuildingBlocks** - Shared building blocks (messaging, caching, validation, etc.)
- **Contracts** - Shared contracts and DTOs

### Microservices
All services are configured to run on specific ports:

| Service | Port | Description |
|---------|------|-------------|
| Identity | 5001 | User authentication and authorization |
| Booking | 5002 | Appointment booking management |
| Branch | 5003 | Branch/location management |
| Catalog | 5004 | Product and service catalog |
| POS | 5005 | Point of sale operations |
| Payments | 5006 | Payment processing |
| Staff | 5007 | Staff management |
| Client | 5008 | Client/customer management |
| Notifications | 5009 | Notification service |
| Reports | 5010 | Reporting service |
| Sync | 5011 | Data synchronization |
| Audit | 5012 | Audit logging |

## Prerequisites

Before running the services in Visual Studio, ensure you have:

1. **SQL Server** running locally on port `14333` (or update connection strings)
   - Default connection: `Server=localhost,14333;Database=app;User ID=sa;Password=P@ssword12345!;TrustServerCertificate=True;`
   - Or use Docker: `docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=P@ssword12345!" -p 14333:1433 -d mcr.microsoft.com/mssql/server:2022-latest`

2. **Redis** running locally on port `6379`
   - Or use Docker: `docker run -d -p 6379:6379 redis:7-alpine`

3. **RabbitMQ** running locally
   - Or use Docker: `docker run -d -p 5672:5672 -p 15672:15672 -e RABBITMQ_DEFAULT_USER=salon -e RABBITMQ_DEFAULT_PASS=salonpass rabbitmq:3-management`

## Running Services in Visual Studio

### Option 1: Run Individual Service
1. Open `SalonPOSMicroservices.sln` in Visual Studio
2. Right-click on any service project (e.g., `IdentityService`)
3. Select "Set as Startup Project"
4. Press F5 or click Run

### Option 2: Run Multiple Services
1. Right-click on the solution
2. Select "Properties"
3. Go to "Startup Project"
4. Select "Multiple startup projects"
5. Set each service to "Start" or "Start without debugging"
6. Click OK and press F5

### Option 3: Use Docker Compose (Recommended for Full Stack)
```powershell
docker compose -f ../docker-compose.dev.yml up
```

## Configuration

Each service has:
- **launchSettings.json** - Configured with proper ports and environment variables
- **appsettings.Development.json** - Local development configuration

### Environment Variables
All services use the following environment variables (configured in launchSettings.json):
- `ASPNETCORE_ENVIRONMENT=Development`
- `ConnectionStrings__Default` - SQL Server connection
- `Redis__ConnectionString` - Redis connection
- `RabbitMQ__HostName` - RabbitMQ host
- `RabbitMQ__UserName` - RabbitMQ username
- `RabbitMQ__Password` - RabbitMQ password

## Accessing Services

Once running, each service exposes:
- **API**: `http://localhost:{port}`
- **Swagger UI**: `http://localhost:{port}/swagger`

Example:
- Identity Service: http://localhost:5001/swagger
- Booking Service: http://localhost:5002/swagger

## Building the Solution

### Build All Projects
```powershell
dotnet build SalonPOSMicroservices.sln
```

### Build Specific Project
```powershell
dotnet build Identity/IdentityService.csproj
```

### Restore NuGet Packages
```powershell
dotnet restore SalonPOSMicroservices.sln
```

## Project Dependencies

All microservices depend on:
- `ServiceDefaults` - Common service configuration
- `BuildingBlocks` - Shared building blocks

The solution automatically handles these dependencies through project references.

## Troubleshooting

### Port Already in Use
If you get a port conflict error:
1. Check which process is using the port: `netstat -ano | findstr :5001`
2. Either stop that process or change the port in `Properties/launchSettings.json`

### Database Connection Issues
- Ensure SQL Server is running and accessible
- Verify the connection string in `appsettings.Development.json`
- Check that the database exists or will be created on first run

### RabbitMQ Connection Issues
- Ensure RabbitMQ is running
- Verify credentials match in `appsettings.Development.json`
- Check RabbitMQ management UI at http://localhost:15672

## Next Steps

1. Open the solution in Visual Studio
2. Set up the required infrastructure (SQL Server, Redis, RabbitMQ)
3. Set a service as the startup project
4. Press F5 to run and debug

