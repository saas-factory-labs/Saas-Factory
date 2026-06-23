#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# mcr.microsoft.com/dotnet/aspnet:10.0
FROM mcr.microsoft.com/dotnet/aspnet@sha256:ddcf70ad1ab963a4fcd41fbd722a6b660e404e87567cfbd46fd2809c21b02088 AS base
USER app
WORKDIR /app

EXPOSE 443

# mcr.microsoft.com/dotnet/sdk:10.0
FROM mcr.microsoft.com/dotnet/sdk@sha256:548d93f8a18a1acbe6cc127bc4f47281430d34a9e35c18afa80a8d6741c2adc3 AS build

# Install SSL libraries
RUN apt-get update && apt-get install -y libssl-dev

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AppBlueprint.AppHost/AppBlueprint.AppHost.csproj", "AppBlueprint.AppHost/"]
COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
RUN dotnet restore "AppBlueprint.AppHost/AppBlueprint.AppHost.csproj"

COPY . .
WORKDIR "/src/AppBlueprint.AppHost"
RUN dotnet build "AppBlueprint.AppHost.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AppBlueprint.AppHost.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
USER app
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppBlueprint.AppHost.dll"]
