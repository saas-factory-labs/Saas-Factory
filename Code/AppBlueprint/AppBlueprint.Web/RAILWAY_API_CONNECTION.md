# Railway Cloud API Connection Configuration

## Overview
The Web project has been configured to connect to the API service using environment variables, enabling deployment to Railway Cloud where services may have different URLs than localhost.

## Changes Made

### 1. Environment Variable Support (Program.cs)
The API base URL is now determined with the following priority:
1. **Environment Variable**: `API_BASE_URL` (highest priority - used in Railway)
2. **Configuration Setting**: `ApiBaseUrl` from appsettings.json
3. **Default Fallback**: `http://localhost:8091` (for local development)

**Code Location**: `Program.cs` lines ~216-225
```csharp
string apiBaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") 
    ?? builder.Configuration["ApiBaseUrl"] 
    ?? "http://localhost:8091";
```

### 2. Configuration File (appsettings.json)
Added `ApiBaseUrl` setting for local development default:
```json
"ApiBaseUrl": "http://localhost:8091"
```

### 3. Services Updated
Both API client services now use the configurable base URL:
- **IRequestAdapter** (Kiota-based API client)
- **TodoService HttpClient** (direct HTTP calls)

## Database Access Verification

### ✅ No Direct Database References
The Web project has been verified to have:
- **No database packages**: No Npgsql, PostgreSQL, or EntityFramework packages in AppBlueprint.Web.csproj
- **No DbContext usage**: All data access goes through API HTTP calls
- **Infrastructure reference**: Only uses `AppBlueprint.Infrastructure.Authorization` namespace for authentication (ITokenStorageService, IUserAuthenticationProvider)

### Project References
```xml
<ProjectReference Include="../AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj" />
<ProjectReference Include="..\Shared-Modules\AppBlueprint.Api.Client.Sdk\AppBlueprint.Api.Client.Sdk.csproj" />
<ProjectReference Include="..\Shared-Modules\AppBlueprint.Application\AppBlueprint.Application.csproj" />
<ProjectReference Include="..\Shared-Modules\AppBlueprint.Domain\AppBlueprint.Domain.csproj" />
<ProjectReference Include="..\Shared-Modules\AppBlueprint.Infrastructure\AppBlueprint.Infrastructure.csproj" />
<ProjectReference Include="..\Shared-Modules\AppBlueprint.SharedKernel\AppBlueprint.SharedKernel.csproj" />
<ProjectReference Include="..\Shared-Modules\AppBlueprint.UiKit\AppBlueprint.UiKit.csproj" />
<ProjectReference Include="..\AppBlueprint.TodoAppKernel\AppBlueprint.TodoAppKernel.csproj" />
```

**Note**: While Infrastructure is referenced, it's ONLY used for authorization interfaces, not database access.

## Railway Deployment Configuration

### Environment Variables to Set in Railway

#### Web Service
```bash
API_BASE_URL=https://your-api-service.railway.app
```
Or use Railway's internal service URL:
```bash
API_BASE_URL=http://api-service:8091
```

#### API Service
Ensure the API service exposes port 8091 or configure accordingly.

### Dockerfile Configuration
The Web Dockerfile already:
- Exposes port 80
- Sets `ASPNETCORE_URLS=http://+:80`
- Does not require database connections

### HTTPS/TLS Configuration
**Important**: The Web application is configured to:
- **Production Mode**: Only listens on HTTP port 80 (HTTPS is disabled)
- **Development Mode**: Listens on both HTTP (80) and HTTPS (443)

Railway handles TLS termination at the edge/load balancer level, so the application container only needs to serve HTTP traffic. This prevents the "Unable to configure HTTPS endpoint" error in Railway deployments.

## Local Development

### Default Behavior
When running locally without environment variables:
- Uses `http://localhost:8091` from appsettings.json
- Works with Aspire AppHost orchestration
- No changes needed to existing local setup

### Testing with Custom API URL
Set environment variable before running:
```powershell
$env:API_BASE_URL="http://custom-api-url:8091"
dotnet run
```

## Verification Checklist

- [x] No database packages in Web.csproj
- [x] No DbContext usage in Web project
- [x] API base URL uses environment variable
- [x] Configuration file has sensible defaults
- [x] Both API clients use configurable URL
- [x] Dockerfile exposes correct port
- [x] Infrastructure reference only for authorization
- [x] Build completes without errors

## Architecture Notes

### Clean Separation
```
Web Project (Blazor UI)
    ↓ HTTP Calls Only
API Project (REST API)
    ↓ EF Core
Database (PostgreSQL)
```

The Web project:
- **Only** communicates via HTTP to the API
- **Never** accesses the database directly
- Uses authorization abstractions for token management
- Is stateless and horizontally scalable

### Service Communication
- **Local Development**: Uses localhost URLs with specific ports
- **Railway Deployment**: Uses environment variable `API_BASE_URL`
- **Authentication**: Passes JWT tokens via HTTP Authorization headers
- **Tenant Isolation**: Passes tenant-id via HTTP headers

## Troubleshooting

### Web can't connect to API
1. Check `API_BASE_URL` environment variable is set correctly
2. Verify API service is running and accessible
3. Check Railway service URLs and internal networking
4. Review logs for connection errors

### Authentication Issues
1. Verify Logto configuration in environment variables
2. Check JWT token is being stored and retrieved
3. Ensure API validates tokens correctly
4. Review CORS settings if applicable

## Related Files
- `Program.cs` - API client registration and configuration
- `appsettings.json` - Default configuration values
- `Services/TodoService.cs` - Example HTTP-based data service
- `Services/AuthenticationDelegatingHandler.cs` - Adds auth headers to API calls
- `Dockerfile` - Container configuration for deployment

