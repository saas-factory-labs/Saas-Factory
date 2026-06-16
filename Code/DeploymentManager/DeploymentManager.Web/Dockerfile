# Build context: repo root (Railway default; also works locally from repo root)
# docker build -f Code/DeploymentManager/DeploymentManager.Web/Dockerfile -t deploymentmanager-web .

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Install Node.js for TypeScript compilation (required by AppBlueprint.UiKit → Microsoft.TypeScript.MSBuild)
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Package management
COPY Code/DeploymentManager/Directory.Packages.props Code/DeploymentManager/
COPY Code/AppBlueprint/Directory.Packages.props Code/AppBlueprint/
COPY Code/AppBlueprint/NuGet.Config /root/.nuget/NuGet.Config

# Project files for layer-cached restore
COPY Code/DeploymentManager/DeploymentManager.Web/DeploymentManager.Web.csproj Code/DeploymentManager/DeploymentManager.Web/

# AppBlueprint direct dependencies
COPY Code/AppBlueprint/AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj Code/AppBlueprint/AppBlueprint.ServiceDefaults/
COPY Code/AppBlueprint/Shared-Modules/AppBlueprint.Application/AppBlueprint.Application.csproj Code/AppBlueprint/Shared-Modules/AppBlueprint.Application/
COPY Code/AppBlueprint/Shared-Modules/AppBlueprint.Contracts/AppBlueprint.Contracts.csproj Code/AppBlueprint/Shared-Modules/AppBlueprint.Contracts/
COPY Code/AppBlueprint/Shared-Modules/AppBlueprint.Domain/AppBlueprint.Domain.csproj Code/AppBlueprint/Shared-Modules/AppBlueprint.Domain/
COPY Code/AppBlueprint/Shared-Modules/AppBlueprint.SharedKernel/AppBlueprint.SharedKernel.csproj Code/AppBlueprint/Shared-Modules/AppBlueprint.SharedKernel/
COPY Code/AppBlueprint/Shared-Modules/AppBlueprint.UiKit/AppBlueprint.UiKit.csproj Code/AppBlueprint/Shared-Modules/AppBlueprint.UiKit/
COPY Code/AppBlueprint/Shared-Modules/AppBlueprint.AdminPortalKernel/AppBlueprint.AdminPortalKernel.csproj Code/AppBlueprint/Shared-Modules/AppBlueprint.AdminPortalKernel/

# Infrastructure facade and all sub-modules (transitive)
COPY Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure/AppBlueprint.Infrastructure.csproj Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure/
COPY Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Authentication/AppBlueprint.Infrastructure.Authentication.csproj Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Authentication/
COPY Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Compliance/AppBlueprint.Infrastructure.Compliance.csproj Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Compliance/
COPY Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Email/AppBlueprint.Infrastructure.Email.csproj Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Email/
COPY Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Notifications/AppBlueprint.Infrastructure.Notifications.csproj Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Notifications/
COPY Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Payments/AppBlueprint.Infrastructure.Payments.csproj Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Payments/
COPY Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Persistence/AppBlueprint.Infrastructure.Persistence.csproj Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Persistence/
COPY Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Realtime/AppBlueprint.Infrastructure.Realtime.csproj Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Realtime/
COPY Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Search/AppBlueprint.Infrastructure.Search.csproj Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Search/
COPY Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Storage/AppBlueprint.Infrastructure.Storage.csproj Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Storage/

RUN dotnet restore "Code/DeploymentManager/DeploymentManager.Web/DeploymentManager.Web.csproj"

COPY Code/DeploymentManager/ Code/DeploymentManager/
COPY Code/AppBlueprint/ Code/AppBlueprint/

WORKDIR /src/Code/DeploymentManager/DeploymentManager.Web
RUN dotnet build "DeploymentManager.Web.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

# Empty plugins dir — always present so the final stage COPY never fails.
RUN mkdir -p /app/plugins

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DeploymentManager.Web.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
USER $APP_UID
WORKDIR /app

COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish .

# Copy plugins dir from build stage (empty on Railway/git builds; contains DLLs for CI builds).
COPY --from=build --chown=$APP_UID:$APP_UID /app/plugins ./plugins/
ENV AdminPortal__PluginsPath=/app/plugins

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_GENERATE_ASPNET_CERTIFICATE=false \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "DeploymentManager.Web.dll"]
