#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app

EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

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
