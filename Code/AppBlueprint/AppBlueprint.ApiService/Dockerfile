# Base runtime image (ASP.NET Core for API) - Includes ICU for globalization
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

# Expose only HTTP port (non-privileged port)
EXPOSE 8080

# Build stage - Microsoft SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy package management files
COPY Directory.Packages.props ./
COPY NuGet.Config /root/.nuget/NuGet.Config

# Copy project files for API and all dependencies
COPY ["AppBlueprint.ApiService/AppBlueprint.ApiService.csproj", "AppBlueprint.ApiService/"]
COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
COPY ["AppBlueprint.TodoAppKernel/AppBlueprint.TodoAppKernel.csproj", "AppBlueprint.TodoAppKernel/"]
COPY ["Shared-Modules/AppBlueprint.Application/AppBlueprint.Application.csproj", "Shared-Modules/AppBlueprint.Application/"]
COPY ["Shared-Modules/AppBlueprint.Contracts/AppBlueprint.Contracts.csproj", "Shared-Modules/AppBlueprint.Contracts/"]
COPY ["Shared-Modules/AppBlueprint.Domain/AppBlueprint.Domain.csproj", "Shared-Modules/AppBlueprint.Domain/"]
COPY ["Shared-Modules/AppBlueprint.Infrastructure/AppBlueprint.Infrastructure.csproj", "Shared-Modules/AppBlueprint.Infrastructure/"]
COPY ["Shared-Modules/AppBlueprint.Presentation.ApiModule/AppBlueprint.Presentation.ApiModule.csproj", "Shared-Modules/AppBlueprint.Presentation.ApiModule/"]
COPY ["Shared-Modules/AppBlueprint.SharedKernel/AppBlueprint.SharedKernel.csproj", "Shared-Modules/AppBlueprint.SharedKernel/"]
COPY ["SeedTest/SeedTest.csproj", "SeedTest/"]

# Restore dependencies
RUN dotnet restore "AppBlueprint.ApiService/AppBlueprint.ApiService.csproj"

# Copy the rest of the source code
COPY . .

WORKDIR "/src/AppBlueprint.ApiService"

# Build the API service
RUN dotnet build "AppBlueprint.ApiService.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AppBlueprint.ApiService.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

# Final runtime image - Hardened distroless
FROM base AS final
USER $APP_UID
WORKDIR /app

# Copy published app with proper ownership
COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish .

# Security hardening environment variables
# ASPNETCORE_HTTP_PORTS is NOT set here - it is resolved at runtime from Railway's PORT variable below
ENV DOTNET_GENERATE_ASPNET_CERTIFICATE=false \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Note: Health checks are configured in railway.json / railway.toml

# Start the API service.
# Railway injects PORT at runtime. We use shell form to expand ${PORT:-8080} so the app
# always binds on the port Railway expects and the /health check succeeds.
ENTRYPOINT ["/bin/sh", "-c", "ASPNETCORE_HTTP_PORTS=${PORT:-8080} exec dotnet AppBlueprint.ApiService.dll"]