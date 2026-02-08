using System.Security.Cryptography;
using System.Text;
using DeploymentManager.CloudInfrastructure;
using Pulumi;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.ServiceBus;
using Pulumi.AzureNative.ServiceBus.Inputs;
using Pulumi.AzureNative.Sql;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using AzureNative = Pulumi.AzureNative;
using Deployment = Pulumi.Deployment;
using Queue = Pulumi.AzureNative.ServiceBus.Queue;
using QueueArgs = Pulumi.AzureNative.ServiceBus.QueueArgs;
using SkuName = Pulumi.AzureNative.Storage.SkuName;

return await Deployment.RunAsync(() =>
{
    #region Configuration

    var configurationManager = new ConfigurationManager();

    string? projectName = configurationManager.GetSetting(SettingType.ProjectName);
    string? stackName = configurationManager.GetSetting(SettingType.StackName);
    string? tenantId = configurationManager.GetSetting(SettingType.TenantId);
    string? region = configurationManager.GetSetting(SettingType.Region);
    string? cloudProvider = configurationManager.GetSetting(SettingType.CloudProvider);
    string? subscriptionId = configurationManager.GetSetting(SettingType.SubscriptionId);
    string? clientId = configurationManager.GetSetting(SettingType.ClientId);
    string? clientSecret = configurationManager.GetSetting(SettingType.ClientSecret);
    string? containerRegistryAdminUsername =
        configurationManager.GetSetting(SettingType.ContainerRegistryAdminUsername);
    string? containerRegistryAdminPassword =
        configurationManager.GetSetting(SettingType.ContainerRegistryAdminPassword);
    string? resourcePrefixName = configurationManager.GetSetting(SettingType.ResourcePrefixName);
    string? databaseAdminLogin = configurationManager.GetSetting(SettingType.DatabaseAdminLogin); // "dplmgradmin";


    var resourceManager = new ResourceManager();

    #endregion

    #region Resource tagging

    var globalTags = new Dictionary<string, string>
    {
        // https://www.pulumi.com/registry/packages/azure-native/api-docs/resources/tagatscope/

        { "Project", projectName },
        { "Environment", stackName },
        { "ProvisionedBy", "Pulumi" }
    };

    var resourceGroupTags = new Dictionary<string, string>();

    #endregion

    #region Password generation

    string secureRandomString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    byte[]? bytes = Encoding.UTF8.GetBytes(secureRandomString);
    byte[]? hash = SHA512.HashData(bytes);

    string? databaseAdminPassword = Convert.ToBase64String(hash);

    #endregion

    #region Naming Convention Service

    // setup and use Azure naming convention API/Frontend manually to generate names and tags and figure out how to apply them dynamically to azure resources in pulumi code (ask pulumi team for advice)

    // add restriction to not use the last 8 characters of the resource or resource group name to avoid conflicts with Pulumi auto-generated name suffixes

    #endregion

    # region Cost Estimation

    // setup and use Azure cost estimation API to estimate the cost of the resources that will be created in this pulumi code (ask pulumi team for advice)

    #endregion

    #region Setup External Infrastucture Resources

    // Grafanacloud
    // Elastic Cloud
    // Cloudflare

    #endregion

    #region Setup Azure Resources

    ResourceGroup? resourceGroup = resourceManager.CreateResourceGroup("saasdplmgr-prod-rg", region);

    Registry? containerRegistry = resourceManager.CreateContainerRegistry("saasdplmgr-prod-acr", resourceGroup);

    Server? sqlServer = resourceManager.CreateSqlServer("saasdplmgr-prod-sql-srv", resourceGroup, databaseAdminLogin,
        databaseAdminPassword);

    Database? sqlDatabase = resourceManager.CreateSqlDatabase("saasdplmgr-prod-sql-db", sqlServer, resourceGroup);

    Output<ListRegistryCredentialsResult>? credentials = Output.Tuple(resourceGroup.Name, containerRegistry.Name)
        .Apply(items =>
            ListRegistryCredentials.InvokeAsync(new ListRegistryCredentialsArgs
            {
                ResourceGroupName = items.Item1,
                RegistryName = items.Item2
            }));
    // _ContainerRegistryAdminUsername = credentials.Apply(c => c.Username);
    // _ContainerRegistryAdminPassword = credentials.Apply(c => c.Passwords[0].Value);

    var containerAppsManagedEnvironment = new ManagedEnvironment("saasdplmgr-prod-capps-env", new ManagedEnvironmentArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location
    });

    var storageAccount = new StorageAccount("saasdplmgrprodstorage", new StorageAccountArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        Sku = new SkuArgs
        {
            Name = SkuName.Standard_LRS
        },
        Kind = Kind.StorageV2
    });

    var imagesBlobContainer = new BlobContainer("images", new BlobContainerArgs
    {
        ResourceGroupName = resourceGroup.Name,
        AccountName = storageAccount.Name,
        PublicAccess = PublicAccess.Blob
    });

    var table = new Table("table", new TableArgs
    {
        AccountName = storageAccount.Name,
        ResourceGroupName = resourceGroup.Name,
        TableName = "UsageTracking"
    });

    var serviceBusNamespace = new Namespace("saasdplmgr-prod-sb-ns", new NamespaceArgs
    {
        Location = region,
        NamespaceName = "saasdplmgr-prod-sb-ns",
        ResourceGroupName = resourceGroup.Name,
        Sku = new SBSkuArgs
        {
            Name = AzureNative.ServiceBus.SkuName.Basic,
            Tier = SkuTier.Basic
        }
        // Tags =
        // {
        //     { "tag1", "value1" },
        //     { "tag2", "value2" },
        // },
    });

    var serviceBusQueue = new Queue("events-queue", new QueueArgs
    {
        EnablePartitioning = true,
        NamespaceName = serviceBusNamespace.Name,
        QueueName = "events-queue",
        ResourceGroupName = resourceGroup.Name
    });

    #endregion

    #region Post Deployment Processing

    // Output<string> serverName;
    // Output<string> administratorLogin;

    #endregion
});
