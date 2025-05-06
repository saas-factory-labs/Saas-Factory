# AppBlueprint.Infrastructure

## Components

- Database Contexts (EF Core code-first)
    - ApplicationDBContext
    - BaselineDBContext
    - B2BDBContext
    - B2CDBContext
- Repositories for each database table
- Data Seeding for each database table
- Data Migrations applied to ApplicationDBContext
- Caching using Redis
- Databases (PostgreSQL, SQL Server, NoSQL, Redis)
- File/Object storage, eg. Azure Blob storage/Cloudflare R2

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
