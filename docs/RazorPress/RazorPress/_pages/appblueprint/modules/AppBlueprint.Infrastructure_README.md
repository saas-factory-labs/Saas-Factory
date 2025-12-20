---
title: AppBlueprint Infrastructure
---

# AppBlueprint.Infrastructure

- Data Context (EF Core code-first)
- Repositories
- Data Seeding
- Data Migrations
- Caching
- Databases (PostgreSQL, SQL Server, NoSQL, Redis)
- File/Object storage, eg. Azure Blob storage

External systems
Databases
Messaging
Email providers
Storage services
Identity
System clock

EF Core
DbContext
Entity configurations
Repositories
Optimistic concurrency
Publishing Domain events

dotnet-ef dbcontext info --context "ApplicationDBContext"
dotnet ef migrations add InitialMigration --context ApplicationDBContext
dotnet ef database update --context ApplicationDBContext --verbose
 
