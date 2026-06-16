# Build context: Code/ (parent of both DeploymentManager/ and AppBlueprint/)
# docker build -f DeploymentManager/DeploymentManager.Web/Dockerfile -t deploymentmanager-web .

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Package management
COPY DeploymentManager/Directory.Packages.props DeploymentManager/
COPY AppBlueprint/Directory.Packages.props AppBlueprint/
COPY AppBlueprint/NuGet.Config /root/.nuget/NuGet.Config

# Project files for layer-cached restore
COPY DeploymentManager/DeploymentManager.Web/DeploymentManager.Web.csproj DeploymentManager/DeploymentManager.Web/

# AppBlueprint direct dependencies
COPY AppBlueprint/AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj AppBlueprint/AppBlueprint.ServiceDefaults/
COPY AppBlueprint/Shared-Modules/AppBlueprint.Application/AppBlueprint.Application.csproj AppBlueprint/Shared-Modules/AppBlueprint.Application/
COPY AppBlueprint/Shared-Modules/AppBlueprint.Contracts/AppBlueprint.Contracts.csproj AppBlueprint/Shared-Modules/AppBlueprint.Contracts/
COPY AppBlueprint/Shared-Modules/AppBlueprint.Domain/AppBlueprint.Domain.csproj AppBlueprint/Shared-Modules/AppBlueprint.Domain/
COPY AppBlueprint/Shared-Modules/AppBlueprint.SharedKernel/AppBlueprint.SharedKernel.csproj AppBlueprint/Shared-Modules/AppBlueprint.SharedKernel/
COPY AppBlueprint/Shared-Modules/AppBlueprint.UiKit/AppBlueprint.UiKit.csproj AppBlueprint/Shared-Modules/AppBlueprint.UiKit/
COPY AppBlueprint/Shared-Modules/AppBlueprint.AdminPortalKernel/AppBlueprint.AdminPortalKernel.csproj AppBlueprint/Shared-Modules/AppBlueprint.AdminPortalKernel/

# Infrastructure facade and all sub-modules (transitive)
COPY AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure/AppBlueprint.Infrastructure.csproj AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure/
COPY AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Authentication/AppBlueprint.Infrastructure.Authentication.csproj AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Authentication/
COPY AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Compliance/AppBlueprint.Infrastructure.Compliance.csproj AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Compliance/
COPY AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Email/AppBlueprint.Infrastructure.Email.csproj AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Email/
COPY AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Notifications/AppBlueprint.Infrastructure.Notifications.csproj AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Notifications/
COPY AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Payments/AppBlueprint.Infrastructure.Payments.csproj AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Payments/
COPY AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Persistence/AppBlueprint.Infrastructure.Persistence.csproj AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Persistence/
COPY AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Realtime/AppBlueprint.Infrastructure.Realtime.csproj AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Realtime/
COPY AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Search/AppBlueprint.Infrastructure.Search.csproj AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Search/
COPY AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Storage/AppBlueprint.Infrastructure.Storage.csproj AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Storage/

RUN dotnet restore "DeploymentManager/DeploymentManager.Web/DeploymentManager.Web.csproj"

COPY DeploymentManager/ DeploymentManager/
COPY AppBlueprint/ AppBlueprint/

WORKDIR /src/DeploymentManager/DeploymentManager.Web
RUN dotnet build "DeploymentManager.Web.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DeploymentManager.Web.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
USER $APP_UID
WORKDIR /app

COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish .

# Admin portal plugin modules: CI pipeline (Scripts/download-admin-plugins.ps1) populates
# this folder before docker build; it is always present (empty when no plugins configured).
COPY --chown=$APP_UID:$APP_UID plugins/ ./plugins/
ENV AdminPortal__PluginsPath=/app/plugins

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_GENERATE_ASPNET_CERTIFICATE=false \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "DeploymentManager.Web.dll"]
