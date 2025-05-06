### Directory.Packages

Central package management for all C# projects.
The package is referenced in each C# project' .csproj file without a version which is sourced from the Directory.Packages.props file:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" />
````

Location: Code/AppBlueprint/Directory.Packages.props

````xml
<!-- This file is used to centrally define the versions of the packages used in the project -->
<Project>
    <PropertyGroup>
        <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    </PropertyGroup>

    <!-- Entity Framework Core and Database Providers -->
    <ItemGroup>
        <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="9.0.1" />
        <PackageVersion Include="Microsoft.EntityFrameworkCore.Relational" Version="9.0.1" />
        <PackageVersion Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.1">
            <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
            <PrivateAssets>all</PrivateAssets>
        </PackageVersion>
        <PackageVersion Include="Npgsql" Version="9.0.2" />
        <PackageVersion Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.3" />
        <PackageVersion Include="AspNetCore.HealthChecks.NpgSql" Version="9.0.0" />
    </ItemGroup>

    <!-- Configuration and Compliance -->
    <ItemGroup>
        <PackageVersion Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" Version="1.21.0" />
        <PackageVersion Include="Microsoft.Extensions.Http.Resilience" Version="9.1.0" />
        <PackageVersion Include="Microsoft.Extensions.ServiceDiscovery" Version="9.0.0" />
        <PackageVersion Include="Microsoft.Extensions.Http.Resiliency" Version="9.1.0" />
        <PackageVersion Include="Microsoft.Extensions.Hosting" Version="9.0.1" />
        <PackageVersion Include="Microsoft.Extensions.Configuration" Version="9.0.1" />
        <PackageVersion Include="Microsoft.Extensions.Configuration.Abstractions" Version="9.0.1" />
        <PackageVersion Include="Microsoft.Extensions.Configuration.UserSecrets" Version="9.0.1" />
        <PackageVersion Include="Microsoft.Extensions.Compliance.Abstractions" Version="9.1.0" />
        <PackageVersion Include="Microsoft.Extensions.Compliance.Redaction" Version="9.1.0" />
        <PackageVersion Include="Microsoft.Extensions.Compliance.Testing" Version="9.1.0" />
    </ItemGroup>

    <!-- ASP.NET Core and Middleware -->
    <ItemGroup>
        <PackageVersion Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.1" />
        <PackageVersion Include="Microsoft.IdentityModel.Tokens" Version="8.3.1" />
        <PackageVersion Include="System.IdentityModel.Tokens.Jwt" Version="8.3.1" />
        <PackageVersion Include="Microsoft.AspNetCore.Authorization" Version="9.0.0" />
        <PackageVersion Include="Microsoft.AspNetCore.DataProtection" Version="9.0.1" />
        <PackageVersion Include="Microsoft.AspNetCore.DataProtection.Abstractions" Version="9.0.1" />
        <PackageVersion Include="Microsoft.AspNetCore.OpenApi" Version="9.0.0" />
        <PackageVersion Include="Microsoft.AspNetCore.Components.Web" Version="9.0.1" />
        <PackageVersion Include="Microsoft.AspNetCore.App" Version="2.2.8" />
    </ItemGroup>

    <!-- OpenTelemetry Dependencies -->
    <ItemGroup>
        <PackageVersion Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.11.1" />
        <PackageVersion Include="OpenTelemetry.Extensions.Hosting" Version="1.11.1" />
        <PackageVersion Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.11.0" />
        <PackageVersion Include="OpenTelemetry.Instrumentation.Http" Version="1.11.0" />
        <PackageVersion Include="OpenTelemetry.Instrumentation.Runtime" Version="1.11.0" />
    </ItemGroup>

    <!-- Swashbuckle and OpenAPI -->
    <ItemGroup>
        <PackageVersion Include="Swashbuckle.AspNetCore" Version="7.2.0" />
        <PackageVersion Include="Swashbuckle.AspNetCore.Filters" Version="8.0.2" />
        <PackageVersion Include="Swashbuckle.AspNetCore.SwaggerGen" Version="7.2.0" />
        <PackageVersion Include="Swashbuckle.AspNetCore.SwaggerUI" Version="7.2.0" />
        <PackageVersion Include="Microsoft.OpenApi" Version="1.6.22" />
        <PackageVersion Include="Microsoft.OpenApi.Readers" Version="2.0.0-preview5" />
    </ItemGroup>

    <!-- Strawberry Shake (GraphQL) -->
    <ItemGroup>
        <PackageVersion Include="StrawberryShake" Version="14.3.0" />
        <PackageVersion Include="StrawberryShake.Transport.Http" Version="14.3.0" />
        <PackageVersion Include="StrawberryShake.Transport.WebSockets" Version="14.3.0" />
    </ItemGroup>

    <!-- MudBlazor and UI Components -->
    <ItemGroup>
        <PackageVersion Include="MudBlazor" Version="8.2.0" />
        <PackageVersion Include="MudBlazor.ThemeManager" Version="3.0.0" />
    </ItemGroup>

    <!-- Health Checks -->
    <ItemGroup>
        <PackageVersion Include="AspNetCore.HealthChecks.UI.Client" Version="9.0.0" />
        <PackageVersion Include="AspNetCore.HealthChecks.Redis" Version="9.0.0" />
        <PackageVersion Include="AspNetCore.HealthChecks.Uris" Version="9.0.0" />
    </ItemGroup>

    <!-- Testing Dependencies -->
    <ItemGroup>
        <PackageVersion Include="Testcontainers.PostgreSql" Version="4.1.0" />
        <PackageVersion Include="Testcontainers" Version="4.1.0" />
        <PackageVersion Include="TestContainers.Container.Abstractions" Version="1.5.4" />
        <PackageVersion Include="xunit" Version="2.9.3" />
        <PackageVersion Include="xunit.runner.visualstudio" Version="3.0.1">
            <PrivateAssets>all</PrivateAssets>
            <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        </PackageVersion>
        <PackageVersion Include="coverlet.collector" Version="6.0.3">
            <PrivateAssets>all</PrivateAssets>
            <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        </PackageVersion>
        <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
        <PackageVersion Include="TUnit" Version="0.6.89" />
        <PackageVersion Include="TUnit.Assertions" Version="0.6.89" />
    </ItemGroup>

    <!-- Miscellaneous Dependencies -->
    <ItemGroup>
        <PackageVersion Include="FluentValidation" Version="11.11.0" />
        <PackageVersion Include="AutoBogus" Version="2.13.1" />
        <PackageVersion Include="Bogus" Version="35.6.1" />
        <PackageVersion Include="Newtonsoft.Json" Version="13.0.3" />
        <PackageVersion Include="System.Text.Json" Version="9.0.1" />
        <PackageVersion Include="Serilog" Version="4.2.0" />
        <PackageVersion Include="Stripe.net" Version="47.2.0" />
        <PackageVersion Include="Spectre.Console" Version="0.49.1" />
        <PackageVersion Include="FigletFontArt" Version="1.0.0" />
        <PackageVersion Include="Scalar.AspNetCore" Version="1.2.75" />
        <PackageVersion Include="System.CommandLine" Version="2.0.0-beta4.22272.1" />
        <PackageVersion Include="System.CommandLine.NamingConventionBinder" Version="2.0.0-beta4.22272.1" />
        <PackageVersion Include="System.Configuration.ConfigurationManager" Version="9.0.0" />
        <PackageVersion Include="NetArchTest.Rules" Version="1.3.2" />
        <PackageVersion Include="JetBrains.Annotations" Version="2024.3.0" />
        <PackageVersion Include="Karambolo.PO" Version="1.11.1" />
        <PackageVersion Include="MongoDB.Driver.Core" Version="2.30.0" />
        <PackageVersion Include="Yarp.ReverseProxy" Version="2.2.0" />
    </ItemGroup>

    <!-- Application Hosting -->
    <ItemGroup>
        <PackageVersion Include="Aspire.Hosting.AppHost" Version="9.0.0" />
        <PackageVersion Include="Aspire.Hosting.PostgreSQL" Version="9.0.0" />
        <PackageVersion Include="Aspire.Hosting.Redis" Version="9.0.0" />
        <PackageVersion Include="Aspire.Hosting.Testing" Version="9.0.0" />
    </ItemGroup>

    <!-- API Versioning -->
    <ItemGroup>
        <PackageVersion Include="Asp.Versioning.Mvc" Version="8.1.0" />
        <PackageVersion Include="Asp.Versioning.Mvc.ApiExplorer" Version="8.1.0" />
    </ItemGroup>
</Project>
````




### Directory.Build

Location: Code/AppBlueprint/Directory.Build.props

````xml
<!-- This file is used to define the common properties for all projects in the solution -->
<Project>
    <PropertyGroup>
        
        <Targetframework>net9.0</Targetframework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>

        <!-- Enable source code analysis using sonar nuget package -->
        <AnalysisLevel>latest</AnalysisLevel>
        <AnalysisMode>All</AnalysisMode>
        <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
        <CodeAnalysisTreatWarningsAsErrors>false</CodeAnalysisTreatWarningsAsErrors>
        <EnforceCodeStyleInBuild>false</EnforceCodeStyleInBuild>
        
        <!-- Enable packaging the shared projects to Nuget package files -->        
        <IsPackable>true</IsPackable>
        <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
        <GeneratePackage>true</GeneratePackage>                
        <Authors>Casper Rubæk</Authors>
        <Company>SaaS Factory</Company>
        <Copyright>© SaaaS Factory</Copyright>
        <Description>Shared .nuget packages for Appblueprint template from Shared-Modules folder</Description>        
        <RepositoryUrl>https://github.com/Trubador/SaaS-Factory</RepositoryUrl>
        <RepositoryType>git</RepositoryType>        
    </PropertyGroup>

    <!-- Enable source code analysis using sonar nuget package across all projects except docker projects eg. .dcproj -->
    <ItemGroup Condition="'$(MSBuildProjectExtension)' != '.dcproj'">
<!--        Version="*"-->
<!--        <PackageReference Include="SonarAnalyzer.CSharp" >-->
<!--            <PrivateAssets>all</PrivateAssets>-->
<!--            <IncludeAssets>-->
<!--                runtime; build; native; contentfiles; analyzers; buildtransitive-->
<!--            </IncludeAssets>-->
<!--        </PackageReference>-->
    </ItemGroup>

</Project>
````

### NuGet.Config

Location: Code/AppBlueprint/NuGet.Config

````xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
        <!-- <add key="LocalNugetFeed" value="C:\Development\LocalNugetFeed" /> -->
        <add key="trubador-github" value="https://nuget.pkg.github.com/Trubador/index.json" />
    </packageSources>

    <packageSourceMapping>
        <packageSource key="nuget.org">
            <package pattern="*" />            
        </packageSource>
        <!-- <packageSource key="LocalNugetFeed"> -->
        <!--     <package pattern="*" /> -->
        <!-- </packageSource> -->
        <packageSource key="trubador-github">
            <package pattern="AppBlueprint.*" />
            <package pattern="SaaS-Factory.*" />
        </packageSource>
    </packageSourceMapping>

    <config>
        <add key="globalPackagesFolder" value="packages" />
    </config>

    <packageSourceCredentials>
        <trubador-github>
            <add key="Username" value="Trubador" />
            <add key="ClearTextPassword" value="%GITHUB_TRUBADOR_PAT%"  />
        </trubador-github>
    </packageSourceCredentials>
</configuration>

````

### Package versioning (Nuget, Docker)

Set this property in the .csproj file of a C# project to enable automatic versioning using the gitversion tool:

```xml
<PropertyGroup>
        <Version>$(GITVERSION_SemVer)</Version>
</PropertyGroup>
```

Add these steps in the Workflow for publishing packages to Github at .github/workflows/publish-nuget-packages-github-packages.yml:

```yaml
- name: Install GitVersion
        run: dotnet tool install --global GitVersion.Tool

      - name: Debug GitVersion Output
        run: |
          dotnet-gitversion /output json || echo "GitVersion failed!"

      - name: Determine Version
        id: gitversion
        run: |
          RAW_VERSION=$(dotnet-gitversion /output json 2>/dev/null || echo '{}')
          echo "Raw GitVersion Output: $RAW_VERSION"
          VERSION=$(echo "$RAW_VERSION" | jq -r '.SemVer // empty')
          
          if [[ -z "$VERSION" ]]; then
            echo "GitVersion failed to generate a version. Check Git tags and branch naming."
            exit 1
          fi
          
          echo "VERSION=$VERSION" >> $GITHUB_ENV
        shell: bash
````

### Dockerfile

Building a Dockerfile:

```bash
cd Code/AppBlueprint
docker build -f AppBlueprint.Web/Dockerfile . -t appblueprint-web:dev
|
docker buildx bake appblueprint-web
```
This ensures that the build context has access to the files in the Code/AppBlueprint directory needed to pull in dependendant files and C# projects to build the Dockerfile

````dockerfile
# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 443

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG GITHUB_TRUBADOR_PAT

# Install SSL libraries for HTTPS
RUN apt-get update && apt-get install -y libssl-dev

WORKDIR /src

# Copy the central package management file and NuGet config from AppBlueprint directory
COPY Directory.Packages.props ./
COPY NuGet.Config /root/.nuget/NuGet.Config

# Replace the token placeholder in NuGet config
RUN sed -i "s|%GITHUB_TRUBADOR_PAT%|$GITHUB_TRUBADOR_PAT|g" /root/.nuget/NuGet.Config

# Copy project files
COPY ["AppBlueprint.Web/AppBlueprint.Web.csproj", "AppBlueprint.Web/"]
COPY ["AppBlueprint.ServiceDefaults/AppBlueprint.ServiceDefaults.csproj", "AppBlueprint.ServiceDefaults/"]

# Restore packages (Now using the updated NuGet config with correct GitHub token)
RUN dotnet restore "AppBlueprint.Web/AppBlueprint.Web.csproj"

# Copy the rest of the source code
COPY . .

WORKDIR "/src/AppBlueprint.Web"
RUN dotnet build "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AppBlueprint.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppBlueprint.Web.dll"]
````


### Docker Compose

Location: Code/AppBlueprint/docker-compose.yml

### Login to Github registry on Coolify VM

````bash
$: docker login ghcr.io -u Trubador
$: Password:
$: WARNING! Your password will be stored unencrypted in /root/.docker/config.json.
$: Configure a credential helper to remove this warning. See
$: https://docs.docker.com/engine/reference/commandline/login/#credentials-store
$: Login Succeeded
````


1. First list item
   - First nested list item
     - Second nested list item

> [!NOTE]
> Useful information that users should know, even when skimming content.

> [!TIP]
> Helpful advice for doing things better or more easily.

> [!IMPORTANT]
> Key information users need to know to achieve their goal.

> [!WARNING]
> Urgent info that needs immediate user attention to avoid problems.

> [!CAUTION]
> Advises about risks or negative outcomes of certain actions.


<details>

<summary> ### You can add a header </summary>

### You can add a header

You can add text within a collapsed section.

You can add an image or a code block, too.

```ruby
   puts "Hello World"
```

</details>
