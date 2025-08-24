# AppBlueprint.Infrastructure

[![NuGet Version](https://img.shields.io/nuget/v/SaaS-Factory.AppBlueprint.Infrastructure)](https://www.nuget.org/packages/SaaS-Factory.AppBlueprint.Infrastructure)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Overview

The AppBlueprint.Infrastructure package provides the infrastructure layer implementation for the AppBlueprint template. This package contains database repositories, external service integrations, and other infrastructure concerns following Clean Architecture and Domain-Driven Design principles.

## Features

### Database & Persistence
- **Entity Framework Core** integration with PostgreSQL
- **Multiple Database Contexts**:
  - ApplicationDBContext
  - BaselineDBContext  
  - B2BDBContext
  - B2CDBContext
- **Repository implementations** for domain aggregates
- **Data seeding** for each database table
- **Database migrations** applied to ApplicationDBContext
- **Optimistic concurrency** control
- **Domain event publishing**
- **Health checks** for database connectivity

### External Service Integrations
- **Email services** via Resend
- **Payment processing** via Stripe
- **Cloud storage** via AWS S3/Azure Blob/Cloudflare R2
- **Authentication** and authorization infrastructure
- **Identity providers** integration
- **Messaging systems** integration

### Caching & Performance
- **Redis caching** implementation
- **Distributed caching** abstractions
- **Performance monitoring**

### Infrastructure Services
- **Reverse proxy** configuration with YARP
- **Data protection** and security services
- **Configuration management** with user secrets support
- **GraphQL client** implementations via Kiota
- **System clock** abstractions

### Health Monitoring
- PostgreSQL health checks
- Redis health checks
- URI endpoint health checks

## Installation

```bash
dotnet add package SaaS-Factory.AppBlueprint.Infrastructure
```

## Dependencies

This package includes the following key dependencies:

- **Microsoft.EntityFrameworkCore** - ORM for database operations
- **Npgsql.EntityFrameworkCore.PostgreSQL** - PostgreSQL provider
- **Microsoft.AspNetCore.Authorization** - Authorization framework
- **Stripe.net** - Stripe payment integration
- **AWSSDK.S3** - AWS S3 storage integration
- **Resend** - Email service integration
- **Yarp.ReverseProxy** - Reverse proxy functionality

## Usage

### Database Context Registration

```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
```

### Repository Pattern

```csharp
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

### Health Checks

```csharp
services.AddHealthChecks()
    .AddNpgSql(connectionString)
    .AddRedis(redisConnectionString);
```

## Configuration

The package supports various configuration options through `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AppBlueprint;..."
  },
  "Stripe": {
    "PublishableKey": "pk_test_...",
    "SecretKey": "sk_test_..."
  },
  "AWS": {
    "Region": "us-west-2", 
    "S3BucketName": "your-bucket"
  }
}
```

## Architecture

This infrastructure layer implements:

- **Repository Pattern** for data access abstraction
- **Unit of Work Pattern** for transaction management
- **Factory Pattern** for service creation
- **Dependency Injection** for loose coupling
- **Health Check Pattern** for monitoring
- **Domain Event Pattern** for cross-cutting concerns

## Components

### Database Contexts (EF Core code-first)
- ApplicationDBContext
- BaselineDBContext
- B2BDBContext
- B2CDBContext

### External Systems Integration
- Databases (PostgreSQL, SQL Server, NoSQL, Redis)
- Messaging systems
- Email providers
- Storage services (Azure Blob storage/Cloudflare R2)
- Identity providers
- System clock abstractions

### Entity Framework Core Features
- DbContext configurations
- Entity configurations
- Repository implementations
- Optimistic concurrency control
- Domain event publishing

## Contributing

This package is part of the SaaS Factory Labs AppBlueprint template. Contributions are welcome!

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/saas-factory-labs/Saas-Factory/blob/main/LICENSE) file for details.

## Support

- 📚 [Documentation](https://github.com/saas-factory-labs/Saas-Factory)
- 🐛 [Issues](https://github.com/saas-factory-labs/Saas-Factory/issues)
- 💬 [Discussions](https://github.com/saas-factory-labs/Saas-Factory/discussions)

## Links

- [Source Code](https://github.com/saas-factory-labs/Saas-Factory)
- [SaaS Factory Labs](https://github.com/saas-factory-labs)
- [NuGet Package](https://www.nuget.org/packages/SaaS-Factory.AppBlueprint.Infrastructure)
