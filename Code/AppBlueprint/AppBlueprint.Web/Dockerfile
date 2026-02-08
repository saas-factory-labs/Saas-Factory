# Base runtime image (ASP.NET Core for Web) - Includes ICU for globalization
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

# Ensure the container listens on port 8080 (non-privileged port)
ENV ASPNETCORE_URLS="http://+:8080"

EXPOSE 8080


# Build stage - Microsoft SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Install Node.js for TypeScript compilation
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Copy package management files
COPY Code/AppBlueprint/Directory.Packages.props ./
COPY Code/AppBlueprint/NuGet.Config /root/.nuget/NuGet.Config

# Copy project files for Web and all dependencies
COPY ["Code/AppBlueprint/AppBlueprint.Web/AppBlueprint.Web.csproj", "AppBlueprint.Web/"]
COPY ["Code/AppBlueprint/AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
COPY ["Code/AppBlueprint/AppBlueprint.TodoAppKernel/AppBlueprint.TodoAppKernel.csproj", "AppBlueprint.TodoAppKernel/"]
COPY ["Code/AppBlueprint/Shared-Modules/AppBlueprint.Api.Client.Sdk/AppBlueprint.Api.Client.Sdk.csproj", "Shared-Modules/AppBlueprint.Api.Client.Sdk/"]
COPY ["Code/AppBlueprint/Shared-Modules/AppBlueprint.Application/AppBlueprint.Application.csproj", "Shared-Modules/AppBlueprint.Application/"]
COPY ["Code/AppBlueprint/Shared-Modules/AppBlueprint.Contracts/AppBlueprint.Contracts.csproj", "Shared-Modules/AppBlueprint.Contracts/"]
COPY ["Code/AppBlueprint/Shared-Modules/AppBlueprint.Domain/AppBlueprint.Domain.csproj", "Shared-Modules/AppBlueprint.Domain/"]
COPY ["Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure/AppBlueprint.Infrastructure.csproj", "Shared-Modules/AppBlueprint.Infrastructure/"]
COPY ["Code/AppBlueprint/Shared-Modules/AppBlueprint.SharedKernel/AppBlueprint.SharedKernel.csproj", "Shared-Modules/AppBlueprint.SharedKernel/"]
COPY ["Code/AppBlueprint/Shared-Modules/AppBlueprint.UiKit/AppBlueprint.UiKit.csproj", "Shared-Modules/AppBlueprint.UiKit/"]

# Restore dependencies
RUN dotnet restore "AppBlueprint.Web/AppBlueprint.Web.csproj"

# Copy the rest of the source code
COPY Code/AppBlueprint/ .

WORKDIR "/src/AppBlueprint.Web"

# Build the Web project
RUN dotnet build "AppBlueprint.Web.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AppBlueprint.Web.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

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

# Start the Web service
ENTRYPOINT ["dotnet", "AppBlueprint.Web.dll"]
