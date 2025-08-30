# Base runtime image (ASP.NET Core for API)
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

# Copy project files (Only API)
COPY ["AppBlueprint.ApiService/AppBlueprint.ApiService.csproj", "AppBlueprint.ApiService/"]
COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
COPY ["Shared-Modules/AppBlueprint.Infrastructure/AppBlueprint.Infrastructure.csproj", "AppBlueprint.Infrastructure/"]
COPY ["Shared-Modules/AppBlueprint.SharedKernel/AppBlueprint.SharedKernel.csproj", "AppBlueprint.SharedKernel/"]

# Restore dependencies
RUN dotnet restore "AppBlueprint.ApiService/AppBlueprint.ApiService.csproj"

# Copy the rest of the source code
COPY . .

WORKDIR "/src/AppBlueprint.ApiService"

# Build the API service
RUN dotnet build "AppBlueprint.ApiService.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AppBlueprint.ApiService.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Ensure API only listens on 80
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_GENERATE_ASPNET_CERTIFICATE=false

# Start the API service
ENTRYPOINT ["dotnet", "AppBlueprint.ApiService.dll"]


## Base runtime image
#FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#USER app
#WORKDIR /app
#
## Set default ports for production

#

#
## Install SSL libraries for HTTPS
#RUN apt-get update && apt-get install -y libssl-dev
#
#RUN dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p $CREDENTIAL_PLACEHOLDER$ 
#RUN dotnet dev-certs https --trust
#
#WORKDIR /src
#
## Copy the central package management file and NuGet config from AppBlueprint directory
#COPY Directory.Packages.props ./
#COPY NuGet.Config /root/.nuget/NuGet.Config
#
## Replace the token placeholder in NuGet config
#
## Copy the project files into the image
#COPY ["AppBlueprint.ApiService/AppBlueprint.ApiService.csproj", "AppBlueprint.ApiService/"]
#COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
#COPY ["Shared-Modules/AppBlueprint.Presentation.ApiModule/AppBlueprint.Presentation.ApiModule.csproj", "AppBlueprint.Presentation.ApiModule/"]
#COPY ["Shared-Modules/AppBlueprint.Infrastructure/AppBlueprint.Infrastructure.csproj", "AppBlueprint.Infrastructure/"]
#COPY ["Shared-Modules/AppBlueprint.SharedKernel/AppBlueprint.SharedKernel.csproj", "AppBlueprint.SharedKernel/"]
#
## Restore dependencies
#RUN dotnet restore "AppBlueprint.ApiService/AppBlueprint.ApiService.csproj" --force
#
#COPY . .
#WORKDIR "/src/AppBlueprint.ApiService"
#RUN dotnet build "AppBlueprint.ApiService.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
## Publish stage
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "AppBlueprint.ApiService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
## Development stage (dev)
##FROM build AS dev
##WORKDIR /src/AppBlueprint.ApiService
##EXPOSE 443

#
## Final runtime image
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#
#ENTRYPOINT ["dotnet", "AppBlueprint.ApiService.dll"]
#
