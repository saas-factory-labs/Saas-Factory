# Base runtime image (ASP.NET Core for Gateway) - Microsoft Chiseled (Distroless)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS base
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

# Copy project files (Only Gateway)
COPY ["AppBlueprint.AppGateway/AppBlueprint.AppGateway.csproj", "AppBlueprint.AppGateway/"]
COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]

# Restore dependencies
RUN dotnet restore "AppBlueprint.AppGateway/AppBlueprint.AppGateway.csproj"

# Copy the rest of the source code
COPY . .

WORKDIR "/src/AppBlueprint.AppGateway"

# Build the Gateway service
RUN dotnet build "AppBlueprint.AppGateway.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AppBlueprint.AppGateway.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

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

# Start the Gateway service
ENTRYPOINT ["dotnet", "AppBlueprint.AppGateway.dll"]


