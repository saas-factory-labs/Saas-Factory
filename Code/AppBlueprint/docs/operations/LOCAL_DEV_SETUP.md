# Local Development Setup

The AppHost uses cloud services (Railway PostgreSQL, Clerk) configured via Doppler. No local Docker containers are required.

## Prerequisites

1. .NET 10 SDK
2. Doppler CLI installed and authenticated

## Quick Start

```bash
cd Code/AppBlueprint/AppBlueprint.AppHost
doppler run -- dotnet run
```

Doppler injects the required secrets at runtime:

|Variable|Description|
|--------|-----------|
|`DATABASE_CONNECTIONSTRING`|Railway PostgreSQL connection string|
|`CLERK_PUBLISHABLE_KEY`|Clerk publishable key|
|`CLERK_SECRET_KEY`|Clerk secret key|
|`AUTHENTICATION_PROVIDER`|Set to `Clerk`|

## Run EF Core Migrations

With Doppler providing the connection string:

```bash
cd Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure
doppler run -- dotnet ef database update --startup-project ../../AppBlueprint.ApiService/AppBlueprint.ApiService.csproj
```
