using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.ContainerRegistry.Inputs;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Sql;
using SQLServer = Pulumi.AzureNative.Sql.Server;

namespace DeploymentManager.CloudInfrastructure;

public class ResourceManager
{
    public ResourceGroup CreateResourceGroup(string resourceGroupName, string location)
    {
        // "saasdplmgr-prod-rg-" + _region;

        return new ResourceGroup(resourceGroupName, new ResourceGroupArgs
        {
            Location = location
        });
    }

    public Registry CreateContainerRegistry(string containerRegistryName, ResourceGroup resourceGroup)
    {
        var containerRegistry = new Registry("saasdplmgrprodacr", new RegistryArgs
        {
            Location = resourceGroup.Location,
            ResourceGroupName = resourceGroup.Name,
            Sku = new SkuArgs
            {
                Name = "Basic"
            },
            AdminUserEnabled = true
        });

        return containerRegistry;
    }

    public SQLServer CreateSqlServer(string serverName, ResourceGroup resourceGroup, string administratorLogin,
        string administratorLoginPassword)
    {
        var sqlServer = new SQLServer("saasdplmgr-prod-sql", new ServerArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            AdministratorLogin = administratorLogin,
            AdministratorLoginPassword = administratorLoginPassword
            // ServerName = "slsrv" + "-" + projectName + "-" + stackName + "-" + regionShortCode,
        });
        return sqlServer;
    }

    public Database CreateSqlDatabase(string databaseName, SQLServer sqlServer, ResourceGroup resourceGroup)
    {
        var database = new Database("saasdplmgr-prod-sql-db", new DatabaseArgs
        {
            DatabaseName = databaseName,
            Location = sqlServer.Location,
            ResourceGroupName = resourceGroup.Location,
            ServerName = sqlServer.Name,
            Sku = new Pulumi.AzureNative.Sql.Inputs.SkuArgs
            {
                Capacity = 5,
                Family = "Gen5",
                Name = "Basic",
                Tier = "Basic"
            }
            // AzureADOnlyAuthentication = false,
            // Login = "casper@fambm.dk",
            // PrincipalType = "User",
            // Sid = "1acab470-dfe5-41bf-b342-8ccbacdb5b04",
            // TenantId = "066136d6-59c1-47a0-9f59-e9874c06ae75",
        });
        return database;
    }


    // using Pulumi;

    // public enum SettingType
    // {
    //     projectName,
    //     stackName,
    //     tenantId,
    //     region,
    //     cloudProvider,
    //     subscriptionId,
    //     clientId,
    //     clientSecret,
    //     ContainerRegistryAdminUsername,
    //     ContainerRegistryAdminPassword,
    //     ResourcePrefixName
    // }
    // public class ConfigurationManager
    // {
    //     private readonly Config _config;
    //     private readonly string _projectName;
    //     private readonly string _stackName;
    //     private readonly string _tenantId;
    //     private readonly string _region;
    //     private readonly string _cloudProvider;
    //     private readonly string _subscriptionId;
    //     private readonly string _clientId;
    //     private readonly string _clientSecret;
    //     private readonly string _ContainerRegistryAdminUsername;
    //     private readonly string _ContainerRegistryAdminPassword;

    //     private readonly string _resourcePrefixName;

    //     public string GetSetting(SettingType settingType)
    //     {
    //         switch (settingType)
    //         {
    //             case SettingType.projectName:
    //                 return _projectName;
    //             case SettingType.stackName:
    //                 return _stackName;
    //             case SettingType.tenantId:
    //                 return _tenantId;
    //             case SettingType.region:
    //                 return _region;
    //             case SettingType.cloudProvider:
    //                 return _cloudProvider;
    //             case SettingType.subscriptionId:
    //                 return _subscriptionId;
    //             case SettingType.clientId:
    //                 return _clientId;
    //             case SettingType.clientSecret:
    //                 return _clientSecret;
    //             case SettingType.ContainerRegistryAdminUsername:
    //                 return _ContainerRegistryAdminUsername;
    //             case SettingType.ContainerRegistryAdminPassword:
    //                 return _ContainerRegistryAdminPassword;
    //             case SettingType.ResourcePrefixName:
    //                 return _resourcePrefixName;
    //             default:
    //                 return "";
    //         }
    //     }

    // public ConfigurationManager()
    // {
    //     _config = new Config();

    //     var projectName = _config.Require("projectName"); // saasdplmgr
    //     var stackName = _config.Require("stackName"); // prod (environment name) 

    //     _tenantId = _config.Require("tenantId"); // 066136d6-59c1-47a0-9f59-e9874c06ae75";
    //     _region = _config.Require("region");
    //     _cloudProvider = _config.Require("cloudProvider");
    //     _subscriptionId = _config.Require("subscriptionId");
    //     _clientId = _config.Require("clientId");
    //     _clientSecret = _config.RequireSecret("clientSecret").ToString();

    //     _resourcePrefixName = projectName + stackName;
    // }
}
