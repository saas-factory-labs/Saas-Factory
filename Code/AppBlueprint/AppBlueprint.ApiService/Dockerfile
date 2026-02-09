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
COPY Code/AppBlueprint/Directory.Packages.props ./
COPY Code/AppBlueprint/NuGet.Config /root/.nuget/NuGet.Config

# Copy project files for API and all dependencies
COPY ["Code/AppBlueprint/AppBlueprint.ApiService/AppBlueprint.ApiService.csproj", "AppBlueprint.ApiService/"]
COPY ["Code/AppBlueprint/AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
COPY ["Code/AppBlueprint/AppBlueprint.TodoAppKernel/AppBlueprint.TodoAppKernel.csproj", "AppBlueprint.TodoAppKernel/"]
COPY ["Code/AppBlueprint/Shared-Modules/AppBlueprint.Application/AppBlueprint.Application.csproj", "Shared-Modules/AppBlueprint.Application/"]
COPY ["Code/AppBlueprint/Shared-Modules/AppBlueprint.Contracts/AppBlueprint.Contracts.csproj", "Shared-Modules/AppBlueprint.Contracts/"]
COPY ["Code/AppBlueprint/Shared-Modules/AppBlueprint.Domain/AppBlueprint.Domain.csproj", "Shared-Modules/AppBlueprint.Domain/"]
COPY ["Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure/AppBlueprint.Infrastructure.csproj", "Shared-Modules/AppBlueprint.Infrastructure/"]
COPY ["Code/AppBlueprint/Shared-Modules/AppBlueprint.Presentation.ApiModule/AppBlueprint.Presentation.ApiModule.csproj", "Shared-Modules/AppBlueprint.Presentation.ApiModule/"]
COPY ["Code/AppBlueprint/Shared-Modules/AppBlueprint.SharedKernel/AppBlueprint.SharedKernel.csproj", "Shared-Modules/AppBlueprint.SharedKernel/"]

# Restore dependencies
RUN dotnet restore "AppBlueprint.ApiService/AppBlueprint.ApiService.csproj"

# Copy the rest of the source code
COPY Code/AppBlueprint/ .

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
# Railway injects PORT dynamically - if not set, default to 8080
ENV PORT=8080 \
    DOTNET_GENERATE_ASPNET_CERTIFICATE=false \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Note: Health checks should be configured in orchestration layer (Kubernetes/Docker Compose)
# Chiseled images don't include curl/wget for minimal attack surface

# Start the API service - Railway will override PORT at runtime
CMD dotnet AppBlueprint.ApiService.dll --urls "http://+:${PORT}"