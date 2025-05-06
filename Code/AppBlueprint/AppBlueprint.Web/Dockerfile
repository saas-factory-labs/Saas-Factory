# Base runtime image (ASP.NET Core for Web)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app

# Ensure the container listens on port 80
ENV ASPNETCORE_URLS="http://+:80"

EXPOSE 80


# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy package management files
COPY Directory.Packages.props ./
COPY NuGet.Config /root/.nuget/NuGet.Config

# Copy project files (Only Web)
COPY ["AppBlueprint.Web/AppBlueprint.Web.csproj", "AppBlueprint.Web/"]
COPY ["Shared-Modules/AppBlueprint.UiKit/AppBlueprint.UiKit.csproj", "AppBlueprint.UiKit/"]


# Restore dependencies
RUN dotnet restore "AppBlueprint.Web/AppBlueprint.Web.csproj"

# Copy the rest of the source code
COPY . .

WORKDIR "/src/AppBlueprint.Web"

# Build the Web project
RUN dotnet build "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Ensure app only listens on 80
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_GENERATE_ASPNET_CERTIFICATE=false

# Start the Web service
ENTRYPOINT ["dotnet", "AppBlueprint.Web.dll"]


## Base runtime image (ASP.NET Core)
#FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#USER app
#WORKDIR /app
#
## Expose only HTTP ports (Remove HTTPS)

#
## Build stage
#FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
#
#ARG BUILD_CONFIGURATION=Release
#
#WORKDIR /src
#
## Copy NuGet configuration and package management files
#COPY Directory.Packages.props ./
#COPY NuGet.Config /root/.nuget/NuGet.Config
#
## Copy project files
#COPY ["AppBlueprint.Web/AppBlueprint.Web.csproj", "AppBlueprint.Web/"]
#COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
#COPY ["Shared-Modules/AppBlueprint.UiKit/AppBlueprint.UiKit.csproj", "AppBlueprint.UiKit/"]
#
## Restore dependencies before copying full source code (Docker layer optimization)
#RUN dotnet restore "AppBlueprint.Web/AppBlueprint.Web.csproj"
#
## Copy all project files
#COPY . .
#
#WORKDIR "/src/AppBlueprint.Web"
#
## Build the application
#RUN dotnet build "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
## Publish stage
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
## Final runtime image
#FROM base AS final
#WORKDIR /app
#
## Copy the published build output
#COPY --from=publish /app/publish .
#
## Force-disable HTTPS (Ensuring No Certificate Issues)

#
## Start the application
#ENTRYPOINT ["dotnet", "AppBlueprint.Web.dll"]
#
#
### Base runtime image
##FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
##USER app
##WORKDIR /app
##
### Expose only HTTP port (Remove HTTPS ports)

##
### Build stage
##FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
##
##ARG BUILD_CONFIGURATION=Release
##
##WORKDIR /src
##
### Copy NuGet configuration and package management files
##COPY Directory.Packages.props ./
##COPY NuGet.Config /root/.nuget/NuGet.Config
##
### Copy project files
##COPY ["AppBlueprint.Web/AppBlueprint.Web.csproj", "AppBlueprint.Web/"]
##COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
##COPY ["Shared-Modules/AppBlueprint.UiKit/AppBlueprint.UiKit.csproj", "AppBlueprint.UiKit/"]
##
### Restore dependencies before copying full source code (Docker layer optimization)
##RUN dotnet restore "AppBlueprint.Web/AppBlueprint.Web.csproj"
##
### Copy all project files
##COPY . .
##
##WORKDIR "/src/AppBlueprint.Web"
##
### Build the application
##RUN dotnet build "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build
##
### Publish stage
##FROM build AS publish
##ARG BUILD_CONFIGURATION=Release
##RUN dotnet publish "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
##
### Final runtime image
##FROM base AS final
##WORKDIR /app
##
### Copy the published build output
##COPY --from=publish /app/publish .
##
### Force-disable HTTPS (This is the main fix)
##ENV ASPNETCORE_URLS=http://+:8080
##ENV DOTNET_GENERATE_ASPNET_CERTIFICATE=false
##ENV ASPNETCORE_Kestrel__Certificates__Default__Path=""
##ENV ASPNETCORE_Kestrel__Certificates__Default__Password=""
##
### Start the application
##ENTRYPOINT ["dotnet", "AppBlueprint.Web.dll"]
##
##
#### Base runtime image
###FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
###USER app
###WORKDIR /app
###
#### Expose only HTTP ports (No HTTPS)

###
#### Build stage
###FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
###
###ARG BUILD_CONFIGURATION=Release
###
###WORKDIR /src
###
#### Copy NuGet configuration and package management files
###COPY Directory.Packages.props ./
###COPY NuGet.Config /root/.nuget/NuGet.Config
###
#### Copy project files
###COPY ["AppBlueprint.Web/AppBlueprint.Web.csproj", "AppBlueprint.Web/"]
###COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
###COPY ["Shared-Modules/AppBlueprint.UiKit/AppBlueprint.UiKit.csproj", "AppBlueprint.UiKit/"]
###
#### Restore dependencies before copying full source code (Docker layer optimization)
###RUN dotnet restore "AppBlueprint.Web/AppBlueprint.Web.csproj"
###
#### Copy all project files
###COPY . .
###
###WORKDIR "/src/AppBlueprint.Web"
###
#### Build the application
###RUN dotnet build "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build
###
#### Publish stage
###FROM build AS publish
###ARG BUILD_CONFIGURATION=Release
###RUN dotnet publish "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
###
#### Final runtime image
###FROM base AS final
###WORKDIR /app
###
#### Copy the published build output
###COPY --from=publish /app/publish .
###
#### Set environment variables to disable HTTPS
###ENV ASPNETCORE_URLS=http://+:8080
###ENV DOTNET_GENERATE_ASPNET_CERTIFICATE=false
###
#### Start the application
###ENTRYPOINT ["dotnet", "AppBlueprint.Web.dll"]
###
###
##### Base runtime image
####FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
####USER app
####WORKDIR /app
####
####EXPOSE 80
####EXPOSE 8080
####EXPOSE 8081
####EXPOSE 443
####
##### Build stage
####FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
####
####ARG BUILD_CONFIGURATION=Release
####ARG CERT_PASSWORD="YourSecurePassword"
####
##### Install SSL libraries for HTTPS
####RUN apt-get update && apt-get install -y libssl-dev
####
##### Generate the development certificate inside the container
#####RUN mkdir -p /root/.aspnet/https \
#####    && dotnet dev-certs https -ep /root/.aspnet/https/aspnetapp.pfx -p "$CERT_PASSWORD"
####
####WORKDIR /src
####
##### Copy NuGet config and package management files
####COPY Directory.Packages.props ./
####COPY NuGet.Config /root/.nuget/NuGet.Config
####
##### Copy project files
####COPY ["AppBlueprint.Web/AppBlueprint.Web.csproj", "AppBlueprint.Web/"]
####COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
####
##### Restore dependencies
####RUN dotnet restore "AppBlueprint.Web/AppBlueprint.Web.csproj"
####
##### Copy the rest of the source code
####COPY . .
####
####WORKDIR "/src/AppBlueprint.Web"
####RUN dotnet build "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build
####
##### Publish stage
####FROM build AS publish
####ARG BUILD_CONFIGURATION=Release
####RUN dotnet publish "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
####
##### Final runtime image
####FROM base AS final
####WORKDIR /app
####
##### Copy published output
####COPY --from=publish /app/publish .
####
##### Set environment variables for Kestrel to use the generated certificate
####ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/root/.aspnet/https/aspnetapp.pfx
####ENV ASPNETCORE_Kestrel__Certificates__Default__Password=YourSecurePassword
####
####ENTRYPOINT ["dotnet", "AppBlueprint.Web.dll"]
####
####
###### Base runtime image
#####FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#####USER app
#####WORKDIR /app
#####EXPOSE 443
#####
###### Build stage
#####FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
#####ARG BUILD_CONFIGURATION=Release
#####ARG GITHUB_TRUBADOR_PAT
#####
###### Install SSL libraries for HTTPS
#####RUN apt-get update && apt-get install -y libssl-dev
#####
#####RUN dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p $CREDENTIAL_PLACEHOLDER$ 
#####RUN dotnet dev-certs https --trust
#####
#####WORKDIR /src
#####
###### Copy the central package management file and NuGet config from AppBlueprint directory
#####COPY Directory.Packages.props ./
#####COPY NuGet.Config /root/.nuget/NuGet.Config
#####
###### Replace the token placeholder in NuGet config
#####RUN sed -i "s|%GITHUB_TRUBADOR_PAT%|$GITHUB_TRUBADOR_PAT|g" /root/.nuget/NuGet.Config
#####
###### Copy project files
#####COPY ["AppBlueprint.Web/AppBlueprint.Web.csproj", "AppBlueprint.Web/"]
#####COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
#####
###### Restore packages (Now using the updated NuGet config with correct GitHub token)
#####RUN dotnet restore "AppBlueprint.Web/AppBlueprint.Web.csproj"
#####
###### Copy the rest of the source code
#####COPY . .
#####
#####WORKDIR "/src/AppBlueprint.Web"
#####RUN dotnet build "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build
#####
###### Publish stage
#####FROM build AS publish
#####ARG BUILD_CONFIGURATION=Release
#####RUN dotnet publish "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#####
###### Final runtime image
#####FROM base AS final
#####WORKDIR /app
#####COPY --from=publish /app/publish .
#####ENTRYPOINT ["dotnet", "AppBlueprint.Web.dll"]
#####
#####
####### Base runtime image
######FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
######USER app
######WORKDIR /app
######EXPOSE 443
######
######ARG GITHUB_TOKEN
######
######
####### Build stage
######FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
######ARG BUILD_CONFIGURATION=Release
######ARG GITHUB_TOKEN
######
####### Install SSL libraries for HTTPS
######RUN apt-get update && apt-get install -y libssl-dev
######
######WORKDIR /src
######
####### Copy central package management file
######COPY Directory.Packages.props ./
######
####### Authenticate with GitHub Packages (NuGet)
######RUN echo "machine nuget.pkg.github.com\nlogin Trubador\npassword $GITHUB_TOKEN" > ~/.netrc
######
####### Add GitHub NuGet source
######RUN dotnet nuget add source "https://nuget.pkg.github.com/Trubador/index.json" \
######    --name github \
######    --username Trubador \
######    --password $GITHUB_TOKEN \
######    --store-password-in-clear-text
######
####### Test if NuGet can authenticate with GitHub Packages
######RUN dotnet nuget list source
######
####### Copy project files
######COPY ["AppBlueprint.Web/AppBlueprint.Web.csproj", "AppBlueprint.Web/"]
######COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
######
####### Restore packages
######RUN dotnet restore "AppBlueprint.Web/AppBlueprint.Web.csproj"
######
####### Copy the rest of the source code
######COPY . .
######
######WORKDIR "/src/AppBlueprint.Web"
######RUN dotnet build "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build
######
####### Publish stage
######FROM build AS publish
######ARG BUILD_CONFIGURATION=Release
######RUN dotnet publish "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
######
####### Final runtime image
######FROM base AS final
######WORKDIR /app
######COPY --from=publish /app/publish .
######ENTRYPOINT ["dotnet", "AppBlueprint.Web.dll"]
######
######
######## Base runtime image
#######FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#######USER app
#######WORKDIR /app
#######EXPOSE 443
#######
######## Build stage
#######FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
#######ARG BUILD_CONFIGURATION=Release
#######ARG GITHUB_TOKEN
#######
######## Install SSL libraries for HTTPS
#######RUN apt-get update && apt-get install -y libssl-dev
#######
#######WORKDIR /src
#######
######## Copy the central package management file and other shared files
#######COPY Directory.Packages.props ./
#######
######## Add GitHub NuGet source
#######RUN dotnet nuget add source "https://nuget.pkg.github.com/Trubador/index.json" \
#######    --name github \
#######    --username Trubador \
#######    --password $GITHUB_TOKEN \
#######    --store-password-in-clear-text
#######
######## Copy project files
#######COPY ["AppBlueprint.Web/AppBlueprint.Web.csproj", "AppBlueprint.Web/"]
#######COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
#######
######## Restore packages
#######RUN dotnet restore "AppBlueprint.Web/AppBlueprint.Web.csproj"
#######
######## Copy the rest of the source code
#######COPY . .
#######
#######WORKDIR "/src/AppBlueprint.Web"
#######RUN dotnet build "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build
#######
######## Publish stage
#######FROM build AS publish
#######ARG BUILD_CONFIGURATION=Release
#######RUN dotnet publish "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#######
######## Development stage (dev)
#######FROM build AS dev
#######WORKDIR /src/AppBlueprint.Web
#######EXPOSE 443
#######CMD ["dotnet", "watch", "run", "--urls", "http://+:443"]
#######
######## Final runtime image
#######FROM base AS final
#######WORKDIR /app
#######COPY --from=publish /app/publish .
#######ENTRYPOINT ["dotnet", "AppBlueprint.Web.dll"]
#######
#######
######### Base runtime image
########FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
########USER app
########WORKDIR /app
########EXPOSE 443
########
######### Build stage
########FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
########ARG BUILD_CONFIGURATION=Release
########
######### Install SSL libraries for HTTPS
########RUN apt-get update && apt-get install -y libssl-dev
########
########WORKDIR /src
########
######### Copy the central package management file (and any other shared files) first
########COPY Directory.Packages.props ./
######### If you have a NuGet.config, you may want to copy it as well:
######### COPY NuGet.config ./
########
######### Copy project files
########COPY ["AppBlueprint.Web/AppBlueprint.Web.csproj", "AppBlueprint.Web/"]
########COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
########
######### Run restore; now the central file is present so the correct package versions will be used
########RUN dotnet restore "AppBlueprint.Web/AppBlueprint.Web.csproj"
########
######### Copy the rest of the source code
########COPY . .
########
########WORKDIR "/src/AppBlueprint.Web"
########RUN dotnet build "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build
########
######### Publish stage
########FROM build AS publish
########ARG BUILD_CONFIGURATION=Release
########RUN dotnet publish "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
########
######### Development stage (dev)
########FROM build AS dev
########WORKDIR /src/AppBlueprint.Web
########EXPOSE 443
########CMD ["dotnet", "watch", "run", "--urls", "http://+:443"]
########
######### Final runtime image
########FROM base AS final
########WORKDIR /app
########COPY --from=publish /app/publish .
########ENTRYPOINT ["dotnet", "AppBlueprint.Web.dll"]
#######
#######
######### Base runtime image
########FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
########USER app
########WORKDIR /app
########EXPOSE 443
########
######### Build stage
########FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
########ARG BUILD_CONFIGURATION=Release
########
######### Install SSL libraries for HTTPS
########RUN apt-get update && apt-get install -y libssl-dev
########
########WORKDIR /src
########COPY ["AppBlueprint.Web/AppBlueprint.Web.csproj", "AppBlueprint.Web/"]
########COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
########RUN dotnet restore "AppBlueprint.Web/AppBlueprint.Web.csproj"
########
########COPY . .
########WORKDIR "/src/AppBlueprint.Web"
########RUN dotnet build "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build
########
######### Publish stage
########FROM build AS publish
########ARG BUILD_CONFIGURATION=Release
########RUN dotnet publish "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
########
######### Development stage (dev)
########FROM build AS dev
########WORKDIR /src/AppBlueprint.Web
########EXPOSE 443
########CMD ["dotnet", "watch", "run", "--urls", "http://+:443"]
########
######### Final runtime image
########FROM base AS final
########WORKDIR /app
########COPY --from=publish /app/publish .
########ENTRYPOINT ["dotnet", "AppBlueprint.Web.dll"]
#######
#######
#########See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
########
########FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
########USER app
########WORKDIR /app
#########EXPOSE 8080
#########EXPOSE 8081
########EXPOSE 443
########
########FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
########
######### Install SSL libraries
########RUN apt-get update && apt-get install -y libssl-dev
########
########ARG BUILD_CONFIGURATION=Release
########WORKDIR /src
########COPY ["AppBlueprint.Web/AppBlueprint.Web.csproj", "AppBlueprint.Web/"]
########COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
########RUN dotnet restore "AppBlueprint.Web/AppBlueprint.Web.csproj"
########
#########COPY ["AppBlueprint.Web.csproj", "AppBlueprint.Web/"]
#########COPY ["AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]
#########RUN dotnet restore "AppBlueprint.Web.csproj"
########
########COPY . .
########WORKDIR "/src/AppBlueprint.Web"
########RUN dotnet build "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build
########
########FROM build AS publish
########ARG BUILD_CONFIGURATION=Release
########RUN dotnet publish "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
########
########FROM base AS final
########WORKDIR /app
########COPY --from=publish /app/publish .
########ENTRYPOINT ["dotnet", "AppBlueprint.Web.dll"]
