# Notes

<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsAspireHost>true</IsAspireHost>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\AppBlueprint.ApiService\AppBlueprint.ApiService.csproj" />
    <ProjectReference Include="..\AppBlueprint.Web\AppBlueprint.Web.csproj" />
  </ItemGroup>

  <ItemGroup>
      <PackageReference Include="Aspire.Hosting.AppHost" Version="8.0.1" />
      <PackageReference Include="Aspire.Hosting.Azure" Version="8.0.1" />
      <PackageReference Include="Aspire.Hosting.Azure.Storage" Version="8.0.1" />      
      <PackageReference Include="Aspire.Hosting.SqlServer" Version="8.0.1" />
    <PackageReference Include="Aspire.Azure.Storage.Blobs" Version="8.0.1" />
    <PackageReference Include="Aspire.StackExchange.Redis.OutputCaching" Version="8.0.1" />
    <PackageReference Include="AspNetCore.HealthChecks.Uris" Version="8.0.1" />
    <PackageReference Include="Azure.Security.KeyVault.Secrets" Version="4.6.0" />
    <PackageReference Include="coverlet.collector" Version="6.0.2">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Meziantou.Xunit.ParallelTestFramework" Version="2.1.0" />
    <PackageReference Include="Microsoft.Extensions.Http.Resilience" Version="8.5.0" />
    <PackageReference Include="Microsoft.Extensions.ServiceDiscovery" Version="8.0.1" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
    <PackageReference Include="NJsonSchema" Version="11.0.0" />
    <PackageReference Include="NSubstitute" Version="5.1.0" />
    <PackageReference Include="NUlid" Version="1.7.2" />
    <PackageReference Include="OpenTelemetry" Version="1.8.1" />
    <PackageReference Include="Pulumi.AzureNative" Version="2.41.0" />
    <PackageReference Include="Scrutor" Version="4.2.2" />
    <PackageReference Include="xunit" Version="2.8.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Yarp.ReverseProxy" Version="2.1.0" />      
  </ItemGroup>
</Project>

<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <Folder Include="src\" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Aspire.Azure.Storage.Blobs" Version="8.0.1" />
    <PackageReference Include="Aspire.Hosting.AppHost" Version="8.0.1" />
    <PackageReference Include="Aspire.Hosting.Azure" Version="8.0.1" />
    <PackageReference Include="Aspire.Hosting.Azure.Storage" Version="8.0.1" />
    <PackageReference Include="Aspire.Hosting.SqlServer" Version="8.0.1" />
    <PackageReference Include="Aspire.StackExchange.Redis.OutputCaching" Version="8.0.1" />
    <PackageReference Include="AspNetCore.HealthChecks.Uris" Version="8.0.1" />
    <PackageReference Include="Azure.Security.KeyVault.Secrets" Version="4.6.0" />
    <PackageReference Include="Azure.Storage.Blobs" Version="12.20.0" />
    <PackageReference Include="coverlet.collector" Version="6.0.2">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Meziantou.Xunit.ParallelTestFramework" Version="2.1.0" />
    <PackageReference Include="Microsoft.Extensions.Http.Resilience" Version="8.5.0" />
    <PackageReference Include="Microsoft.Extensions.ServiceDiscovery" Version="8.0.1" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
    <PackageReference Include="NJsonSchema" Version="11.0.0" />
    <PackageReference Include="NSubstitute" Version="5.1.0" />
    <PackageReference Include="NUlid" Version="1.7.2" />
    <PackageReference Include="OpenTelemetry" Version="1.8.1" />
    <PackageReference Include="Pulumi.AzureNative" Version="2.41.0" />
    <PackageReference Include="Scrutor" Version="4.2.2" />
    <PackageReference Include="xunit" Version="2.8.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Yarp.ReverseProxy" Version="2.1.0" />
  </ItemGroup>
</Project>

<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <Folder Include="src\" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Aspire.Azure.Storage.Blobs" Version="8.0.1" />
    <PackageReference Include="Aspire.Hosting.AppHost" Version="8.0.1" />
    <PackageReference Include="Aspire.Hosting.Azure" Version="8.0.1" />
    <PackageReference Include="Aspire.Hosting.Azure.Storage" Version="8.0.1" />
    <PackageReference Include="Aspire.Hosting.SqlServer" Version="8.0.1" />
    <PackageReference Include="Aspire.StackExchange.Redis.OutputCaching" Version="8.0.1" />
    <PackageReference Include="AspNetCore.HealthChecks.Uris" Version="8.0.1" />
    <PackageReference Include="Azure.Security.KeyVault.Secrets" Version="4.6.0" />
    <PackageReference Include="Azure.Storage.Blobs" Version="12.20.0" />
    <PackageReference Include="coverlet.collector" Version="6.0.2">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Meziantou.Xunit.ParallelTestFramework" Version="2.1.0" />
    <PackageReference Include="Microsoft.Extensions.Http.Resilience" Version="8.5.0" />
    <PackageReference Include="Microsoft.Extensions.ServiceDiscovery" Version="8.0.1" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
    <PackageReference Include="NJsonSchema" Version="11.0.0" />
    <PackageReference Include="NSubstitute" Version="5.1.0" />
    <PackageReference Include="NUlid" Version="1.7.2" />
    <PackageReference Include="OpenTelemetry" Version="1.8.1" />
    <PackageReference Include="Pulumi.AzureNative" Version="2.41.0" />
    <PackageReference Include="Scrutor" Version="4.2.2" />
    <PackageReference Include="xunit" Version="2.8.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Yarp.ReverseProxy" Version="2.1.0" />
  </ItemGroup>
</Project>

using DeploymentManagerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentManagerApi.Controllers.Base
{
    public class CustomerBaseController : ControllerBase
    {
        // private ILogger<DeploymentController> _logger = new Logger<>();

        public PulumiAutomationAPI pulumiAutomationApiService = new PulumiAutomationAPI();
    }
}

namespace DeploymentManagerApi.Models;

public class InputModel
{
    public string ProjectName { get; set; }
    public string ProjectAbbreviation { get; set; }
    public string Environment { get; set; }
    public string RegistryUsername { get; set; }
    public string Registrypassword { get; set; }
    public string ImageName { get; set; }
    public string AppName { get; set; }
    public List<string> BlobContainerNames { get; set; }
}

Guid as UserId in User class?

Release artifact status - docker image tag <==> product version of specific app (fx boligportal):

- Dev
- ReleaseCandidate
- GenerallyAvailable

- Icollection or var instead of List<> since it can then be used in unit testing

- SQL Hyperscale elastic pool
    - per database minimum capacity
    - per database max capacity

ICollect<CustomerEnvironmentPortal> CustomerPortals </CustomerEnvironmentPortal>


- API controller summary

/// <summary>

///</summary>

- feature flag api controller

// aks
- ThirdPartyNodePool 

// NodaTime.Instant

- value dictionary

    - hashcode() method

- value list
    - hash list


- elastic search cloud

    - api key

- grafana cloud 

    - api key
    - grafana slug url (instance id)


- cloudflare 

    - cloudflareZoneId
    - cloudflareApiToken

- Extensions (folder)
    - DbSetExtensions => IncludeRequired()
    - Enumerable => intersperse()


- pulumi resource naming helper class
    - resource group name => return $"{region.location}_{region.id.ToString(N")[..6]}";
    - CustomerName => name.Replace(" ", ".")
    - valid kubernetes namespace name() => return "(([A-Za-z0-9][-A-Za-z0-9_.]*)?[A-Za-z0-9])?"
    - Input<string>
    - InputList<string>
    - InputMap<string>
    - Input<Guid>
    - Repository Args authentication (until then helm chart)
    - CORS
    - CSP
    - CSRF
    - HSTS
    - Granafa Loki (log viewer)
    - Granafa Jaeger log collector => grafana cloud
    - Base64
    - kubernetes.io/dockerconfigjson

- Services (folder)

    - Auth (folder)

    - ComponentResources (folder)
    - DestinationStoragesComponent.cs => configures settings for Azure blob storage account resource
    





Helpers (folder)

    - PasswordGenerator  => GenerateRandomPassword(int latinLowerCase, int latinUpperCase, int digits, int sepcialCharacters);

    - PulumiHelper => JsonElement GetPulumiArray(object arrayLike)



find tool som kan gøre det nemt at se highlight hvilken element på blazor website som blazor koden tilhører og så man kan se component render tree til at fejlsøge med




for at undgå problem med blazor igen (min fejl var jeg troede jeg rettede i den rigtige MyAccountSettings.razor fil, men det gjorde jeg ikke da den der faktisk blev brugt er i Web projekt - ikke razor class lib projekt.

For at undgå fremtidige problemer - gør dette:

- konsolider og ryd op razor components på tværs af web og uikit projekt

- dokumenter hvordan jeg debugger projekter med at køre dotnet watch run (web) og dotnet watch build (uikit)

- lav en simpel app til at vise render tree i mine apps ligesom med dependency diagram så jeg kan få overblik over hvilke komponenter renders da det kan være svært at holde styr på når komponenter nestes


Blazor build fejl - known error?  https://github.com/dotnet/aspnetcore/issues/52148



