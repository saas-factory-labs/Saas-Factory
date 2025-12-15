# Base runtime image (ASP.NET Core for Gateway)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app

# Expose only HTTP port (inside container)
EXPOSE 80

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
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

# Final runtime image
FROM base AS final
USER app
WORKDIR /app
COPY --from=publish /app/publish .

# Ensure Gateway only listens on 80
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_GENERATE_ASPNET_CERTIFICATE=false

# Start the Gateway service
ENTRYPOINT ["dotnet", "AppBlueprint.AppGateway.dll"]


