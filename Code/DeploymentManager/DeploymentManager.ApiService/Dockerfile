# Build context: repo root (Railway default; also works locally from repo root)
# docker build -f Code/DeploymentManager/DeploymentManager.ApiService/Dockerfile -t deploymentmanager-api .

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled-extra@sha256:de3e2d510c3b30dd10a3ababad927725839aacd0bbd6a3e8aef9a5a4408ccc12 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0@sha256:548d93f8a18a1acbe6cc127bc4f47281430d34a9e35c18afa80a8d6741c2adc3 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Package management
COPY Code/DeploymentManager/Directory.Packages.props Code/DeploymentManager/
COPY Code/AppBlueprint/Directory.Packages.props Code/AppBlueprint/
COPY Code/AppBlueprint/NuGet.Config /root/.nuget/NuGet.Config

# Project files for layer-cached restore
COPY Code/DeploymentManager/DeploymentManager.ApiService/DeploymentManager.ApiService.csproj Code/DeploymentManager/DeploymentManager.ApiService/

# AppBlueprint direct dependencies
COPY Code/AppBlueprint/AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj Code/AppBlueprint/AppBlueprint.ServiceDefaults/
COPY Code/AppBlueprint/Shared-Modules/AppBlueprint.Application/AppBlueprint.Application.csproj Code/AppBlueprint/Shared-Modules/AppBlueprint.Application/
COPY Code/AppBlueprint/Shared-Modules/AppBlueprint.Contracts/AppBlueprint.Contracts.csproj Code/AppBlueprint/Shared-Modules/AppBlueprint.Contracts/
COPY Code/AppBlueprint/Shared-Modules/AppBlueprint.Domain/AppBlueprint.Domain.csproj Code/AppBlueprint/Shared-Modules/AppBlueprint.Domain/
COPY Code/AppBlueprint/Shared-Modules/AppBlueprint.SharedKernel/AppBlueprint.SharedKernel.csproj Code/AppBlueprint/Shared-Modules/AppBlueprint.SharedKernel/
COPY Code/AppBlueprint/Shared-Modules/AppBlueprint.AdminPortalKernel/AppBlueprint.AdminPortalKernel.csproj Code/AppBlueprint/Shared-Modules/AppBlueprint.AdminPortalKernel/
COPY Code/AppBlueprint/Shared-Modules/AppBlueprint.Presentation.ApiModule/AppBlueprint.Presentation.ApiModule.csproj Code/AppBlueprint/Shared-Modules/AppBlueprint.Presentation.ApiModule/

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

RUN dotnet restore "Code/DeploymentManager/DeploymentManager.ApiService/DeploymentManager.ApiService.csproj"

COPY Code/DeploymentManager/ Code/DeploymentManager/
COPY Code/AppBlueprint/ Code/AppBlueprint/

WORKDIR /src/Code/DeploymentManager/DeploymentManager.ApiService
RUN dotnet build "DeploymentManager.ApiService.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DeploymentManager.ApiService.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
USER $APP_UID
WORKDIR /app

COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish .

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_GENERATE_ASPNET_CERTIFICATE=false \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "DeploymentManager.ApiService.dll"]
