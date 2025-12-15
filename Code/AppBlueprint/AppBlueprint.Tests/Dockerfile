FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Code/AppBlueprint/AppBlueprint.Tests/AppBlueprint.Tests.csproj", "Code/AppBlueprint/AppBlueprint.Tests/"]
COPY ["Code/AppBlueprint/AppBlueprint.AppHost/AppBlueprint.AppHost.csproj", "Code/AppBlueprint/AppBlueprint.AppHost/"]
COPY ["Code/AppBlueprint/AppBlueprint.ApiService/AppBlueprint.ApiService.csproj", "Code/AppBlueprint/AppBlueprint.ApiService/"]
COPY ["Code/AppBlueprint/AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "Code/AppBlueprint/AppBlueprint.ServiceDefaults/"]
COPY ["Code/AppBlueprint/Shared-Modules/AppBlueprint.SharedKernel/AppBlueprint.SharedKernel.csproj", "Code/AppBlueprint/Shared-Modules/AppBlueprint.SharedKernel/"]
COPY ["Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure/AppBlueprint.Infrastructure.csproj", "Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure/"]
COPY ["Code/AppBlueprint/AppBlueprint.Web/AppBlueprint.Web.csproj", "Code/AppBlueprint/AppBlueprint.Web/"]
COPY ["Code/AppBlueprint/Shared-Modules/AppBlueprint.UiKit/AppBlueprint.UiKit.csproj", "Code/AppBlueprint/Shared-Modules/AppBlueprint.UiKit/"]
COPY ["Code/AppBlueprint/AppBlueprint.AppGateway/AppBlueprint.AppGateway.csproj", "Code/AppBlueprint/AppBlueprint.AppGateway/"]
RUN dotnet restore "Code/AppBlueprint/AppBlueprint.Tests/AppBlueprint.Tests.csproj"
COPY . .
WORKDIR "/src/Code/AppBlueprint/AppBlueprint.Tests"
RUN dotnet build "AppBlueprint.Tests.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AppBlueprint.Tests.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false
FROM base AS final
USER $APP_UID
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppBlueprint.Tests.dll"]
