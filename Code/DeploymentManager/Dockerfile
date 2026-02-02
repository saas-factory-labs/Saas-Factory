# See https://aka.ms/customizecontainer to learn how to customize your debug container
# Base runtime image - Microsoft Chiseled (Distroless)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS base
USER $APP_UID
WORKDIR /app

# Expose non-privileged ports only
EXPOSE 8080

# Build stage - Microsoft SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DeploymentManager.Web/DeploymentManager.Web.csproj", "DeploymentManager.Web/"]
COPY ["DeploymentManager.ServiceDefaults/DeploymentManager.ServiceDefaults.csproj", "DeploymentManager.ServiceDefaults/"]
RUN dotnet restore "./DeploymentManager.Web/DeploymentManager.Web.csproj"
RUN dotnet restore "./DeploymentManager.ServiceDefaults/DeploymentManager.ServiceDefaults.csproj"

COPY . .
WORKDIR "/src/DeploymentManager.Web"
RUN dotnet build "./DeploymentManager.Web.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DeploymentManager.Web.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

# Final runtime image - Hardened distroless
FROM base AS final
USER $APP_UID
WORKDIR /app

# Copy published app with proper ownership
COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish .

# Security hardening environment variables
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_GENERATE_ASPNET_CERTIFICATE=false \
    DOTNET_EnableDiagnostics=0 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Note: Health checks should be configured in orchestration layer (Kubernetes/Docker Compose)
# Chiseled images don't include curl/wget for minimal attack surface

ENTRYPOINT ["dotnet", "DeploymentManager.Web.dll"]