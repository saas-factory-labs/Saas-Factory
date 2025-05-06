# 👨‍💻 Development workflow


## ⚙️ Components

- Swagger API UI
- NuGet.Config
- Directory.Build.props
- Directory.Packages.props

## 🛜 Port mappings

<table>
            <tr>
                <td>Project</td>
                <td>Url</td>
            </tr>
            <tr>
                <td>AppHost</td>
                <td>https://localhost:17298</td>
            </tr>
            <tr>
                <td>Web</td>
                <td>https://localhost:8083, http://localhost:8082</td>
            </tr>
            <tr>
                <td>Api</td>
                <td>https://localhost:8081/api, http://localhost:8080/api</td>
            </tr>
            <tr>
                <td>Gateway</td>
                <td>https://localhost:8085/gw/health, http://localhost:8084/gw/health</td>
            </tr>
        </table>        

[//]: # (<tabs>)

[//]: # (    <tab id="apphost-port-mappings" title="AppBlueprint.AppHost">)

[//]: # (        )

[//]: # (    </tab>)

[//]: # (    <tab id="docker-compose-port-mappings" title="Docker compose port mappings">)

[//]: # (        How to install on macOS.)

[//]: # (    </tab>    )

[//]: # ()

[//]: # (</tabs>)

![swagger-ui.png](swagger-ui.png)

_https://localhost:8081/index.html_

Swagger json file:

![swagger-json.png](swagger-json.png)

_https://localhost:8081/swagger/v1/swagger.json_

**Port mappings are determined in order of this precendence from highest to lowest:** 

- Command-line arguments (eg. dotnet run --urls "http://localhost:5000")
 
- Code-level configuration (eg. builder.WebHost.ConfigureKestrel(...))
 
- Launchsettings.json / Environment variables (eg. ASPNETCORE_URLS)

- Appsettings.json files (eg. appsettings.json/appsettings.{Environment}.json)



## 🛠️ Prerequisites

### Install Prerequisites on Windows

1. Open PowerShell as Administrator:

```powershell
   wsl --install
   ```

Then restart the computer.

2. Install required tools:

```powershell

@(
    "Microsoft.DotNet.SDK.9",
    "Git.Git",
    "Docker.DockerDesktop",
    "OpenJS.NodeJS",
    "GitHub.cli"
) | ForEach-Object { winget install --accept-package-agreements --accept-source-agreements --id $_ }      
   ```

# Run Github Action workflows locally

```powershell
# run all workflows
act -P self-hosted=ghcr.io/catthehacker/ubuntu:medium
# run specific workflow "publish-packages"
act -P self-hosted=ghcr.io/catthehacker/ubuntu:medium -j publish-packages
```

dokumenter hvordan jeg kører jobs og specifikt job med act kommand lokalt samt at agent type skal være ubuntu-latest i
stedet for self hosted før det virker så jeg skal sende agent type ind med en parameter i stedet for at bruge self
hosted

hvis en af workflows har en syntax fejl vil act kommando ikke virke så selvom det job/workflow man vil køre ikke har
fejl så skal alle andre workflows også være korrekte

## 🛠️ Recommended optional tools

```powershell
   @(
       "xxx",
   ) | ForEach-Object { winget install --accept-package-agreements --accept-source-agreements --id $_ }      
```

## Applying migrations to database

_The commands need to be executed in the AppBlueprint.Infrastructure module since the entity framework database contexts
reside there._

```powershell

# Get information about a database context and its status 
dotnet-ef dbcontext info --context "ApplicationDBContext"
# Apply migrations to a database context
dotnet ef migrations add InitialMigration --context ApplicationDBContext
# Update a database context
dotnet ef database update --context ApplicationDBContext --verbose
```

Null reference validation in the code:

````csharp

await _dbContext.GetUser(user.Id) ?? throw new ArgumentNullException(nameof(value));

if (user is null)
{
    throw new ArgumentNullException(nameof(value));
}

if (user is not null)
{
throw new ArgumentNullException(nameof(value));
}

````

Reduce size of VHD file for WSL Docker Desktop ubuntu server:

````powershell

# Run as Administrator
Optimize-VHD -Path "$env:USERPROFILE\AppData\Local\Docker\wsl\data\ext4.vhdx" -Mode Full

````

Fluent human readable regex nuget package: https://github.com/bcwood/FluentRegex

