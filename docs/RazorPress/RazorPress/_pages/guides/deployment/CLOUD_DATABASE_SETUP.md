# Cloud Database Connection Setup

## Overview

The application now supports connecting to a cloud PostgreSQL database via environment variables. This allows you to easily switch between local development and cloud databases without modifying configuration files.

## Configuration Priority

The database connection string is resolved in the following order:

1. **Environment Variable**: `DATABASE_CONNECTION_STRING` (highest priority)
2. **appsettings.json**: `ConnectionStrings:appblueprintdb`
3. **appsettings.json**: `ConnectionStrings:postgres-server`
4. **appsettings.json**: `ConnectionStrings:DefaultConnection` (lowest priority)

## Setting Up Cloud Database Connection

### Option 1: Using Environment Variable (Recommended for Cloud)

Set the `DATABASE_CONNECTION_STRING` environment variable with your cloud database connection string.

#### Windows PowerShell:
```powershell
$env:DATABASE_CONNECTION_STRING="Host=your-cloud-host.com;Port=5432;Database=appblueprintdb;Username=your_user;Password=your_password;SSL Mode=Require"
```

#### Windows Command Prompt:
```cmd
set DATABASE_CONNECTION_STRING=Host=your-cloud-host.com;Port=5432;Database=appblueprintdb;Username=your_user;Password=your_password;SSL Mode=Require
```

#### Permanent Environment Variable (Windows):
1. Open System Properties → Environment Variables
2. Add a new User or System variable:
   - Name: `DATABASE_CONNECTION_STRING`
   - Value: `Host=your-cloud-host.com;Port=5432;Database=appblueprintdb;Username=your_user;Password=your_password;SSL Mode=Require`

### Option 2: Using appsettings.json

Add the connection string to `AppBlueprint.ApiService/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "appblueprintdb": "Host=your-cloud-host.com;Port=5432;Database=appblueprintdb;Username=your_user;Password=your_password;SSL Mode=Require"
  }
}
```

## Common Cloud Database Providers

### Azure Database for PostgreSQL
```
Host=your-server.postgres.database.azure.com;Port=5432;Database=appblueprintdb;Username=your_user@your-server;Password=your_password;SSL Mode=Require;Trust Server Certificate=true
```

### AWS RDS PostgreSQL
```
Host=your-instance.region.rds.amazonaws.com;Port=5432;Database=appblueprintdb;Username=your_user;Password=your_password;SSL Mode=Require
```

### Supabase
```
Host=db.your-project-ref.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=your_password;SSL Mode=Require
```

### Neon
```
Host=your-project.neon.tech;Port=5432;Database=neondb;Username=your_user;Password=your_password;SSL Mode=Require
```

### Railway
```
Host=containers-us-west-xxx.railway.app;Port=5432;Database=railway;Username=postgres;Password=your_password;SSL Mode=Require
```

## Running the Application with Cloud Database

### Development (without AppHost)

If running the API service directly without Aspire AppHost:

```powershell
# Set the environment variable
$env:DATABASE_CONNECTION_STRING="Host=your-cloud-host.com;Port=5432;Database=appblueprintdb;Username=your_user;Password=your_password;SSL Mode=Require"

# Navigate to the API service
cd Code\AppBlueprint\AppBlueprint.ApiService

# Run the application
dotnet run
```

### With Aspire AppHost

The AppHost uses Aspire's automatic connection string injection. To override with a cloud database:

1. Set the environment variable before running AppHost
2. Or modify `AppBlueprint.AppHost/Program.cs` to use an external connection string instead of the containerized PostgreSQL

## Security Best Practices

1. **Never commit credentials**: Do not commit connection strings with passwords to source control
2. **Use secrets management**: For production, use Azure Key Vault, AWS Secrets Manager, or similar
3. **Enable SSL**: Always use `SSL Mode=Require` for cloud databases
4. **Restrict access**: Configure firewall rules to only allow connections from your application's IP
5. **Use strong passwords**: Generate secure passwords for database users

## Troubleshooting

### Connection Issues

The application logs will show:
- Where the connection string was loaded from (Environment Variable vs Configuration)
- A masked version of the connection string (passwords are hidden)

Check the console output for:
```
[AddDbContext] Database Connection String Source: Environment Variable
[AddDbContext] Database Connection String: Host=xxx;Port=5432;Password=***;...
```

### Common Issues

1. **SSL/TLS errors**: Add `Trust Server Certificate=true` to your connection string
2. **Firewall blocking**: Ensure your cloud database allows connections from your IP
3. **Wrong credentials**: Verify username and password are correct
4. **Database doesn't exist**: Ensure the database name in the connection string matches your cloud database

## Verification

To verify the connection is working:

1. Start the application
2. Check the logs for the connection string source
3. Access the API at `http://localhost:8091/swagger`
4. Try an endpoint that queries the database

## Migration to Cloud Database

If you need to migrate your local database to the cloud:

1. Export your local database:
   ```bash
   pg_dump -h localhost -U postgres -d appblueprintdb > backup.sql
   ```

2. Import to cloud database:
   ```bash
   psql -h your-cloud-host.com -U your_user -d appblueprintdb < backup.sql
   ```

## Contact

For issues or questions, refer to the main documentation or create an issue in the repository.

