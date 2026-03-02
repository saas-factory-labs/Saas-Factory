# Local Development Setup

Run PostgreSQL and Logto locally using Docker so you don't need cloud services during development.

## Prerequisites

1. Docker Desktop installed and running
2. .NET 10 SDK

## Quick Start

```bash
cd Code/AppBlueprint
docker compose -f docker-compose.local.yml up -d
```

This starts:

| Service    | URL                      | Credentials                                    |
|------------|--------------------------|------------------------------------------------|
| PostgreSQL | localhost:5432           | User: appblueprint / Pass: localdev123 / DB: appblueprint |
| Logto API  | http://localhost:3001    | (configure via Admin Console)                  |
| Logto Admin| http://localhost:3002    | (create admin account on first visit)          |

## Configure the App to Use Local Services

Set these environment variables before running the AppHost:

```
DATABASE_CONNECTIONSTRING=Host=localhost;Port=5432;Database=appblueprint;Username=appblueprint;Password=localdev123
LOGTO_ENDPOINT=http://localhost:3001
AUTHENTICATION_PROVIDER=Logto
```

Or for Firebase:

```
AUTHENTICATION_PROVIDER=Firebase
Authentication__Firebase__ApiKey=your-firebase-api-key
```

## First Time Logto Setup

1. Open http://localhost:3002 in your browser
2. Create an admin account
3. Create a new "Traditional Web" application
4. Copy the App ID and App Secret
5. Set LOGTO_APPID and LOGTO_APPSECRET environment variables

## Run EF Core Migration

After the database is running, apply the migration:

```bash
cd Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure
dotnet ef migrations add AddUserExternalIdentities --startup-project ../../AppBlueprint.ApiService/AppBlueprint.ApiService.csproj
dotnet ef database update --startup-project ../../AppBlueprint.ApiService/AppBlueprint.ApiService.csproj
```

## Stop Services

```bash
docker compose -f docker-compose.local.yml down
```

To also remove the database data:

```bash
docker compose -f docker-compose.local.yml down -v
```
