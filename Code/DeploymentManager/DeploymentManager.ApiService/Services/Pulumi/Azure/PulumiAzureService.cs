// using Pulumi.AzureNative.App.Inputs;
// using Pulumi.AzureNative.AzureData;
// using Pulumi.AzureNative.Cache;
// using Pulumi.AzureNative.Resources;
// using Pulumi.AzureNative.Web;
// using Pulumi.AzureNative.Storage;
// using Pulumi.AzureNative.ServiceBus;
// using ServiceBusSkuArgs = Pulumi.AzureNative.ServiceBus.Inputs;
// using Pulumi.AzureNative.KeyVault;
// using StorageAccountSkuArgs = Pulumi.AzureNative.Storage.Inputs.SkuArgs;
// using StorageAccountSkuName = Pulumi.AzureNative.Storage.SkuName;
// using Queue = Pulumi.AzureNative.ServiceBus.Queue;
// using QueueArgs = Pulumi.AzureNative.ServiceBus.QueueArgs;
// using VaultPropertiesArgs = Pulumi.AzureNative.KeyVault.Inputs.VaultPropertiesArgs;
// using RedisSkuArgs = Pulumi.AzureNative.Cache.Inputs.SkuArgs;
// using ContainerApp = Pulumi.AzureNative.App.ContainerApp;
// using ContainerAppArgs = Pulumi.AzureNative.App.ContainerAppArgs;
// using DeploymentPortal.ApiService.Domain.Enum.Pulumi;
// using Domain.Entities;
// using Pulumi.AzureNative.Storage.Inputs;
// using Microsoft.EntityFrameworkCore.Storage;
//
// namespace DeploymentManager.DeploymentPortal.ApiService.Services.Pulumi.Azure
// {
//     public enum AzureResourceType // Azure resource types
//     {
//         ResourceGroup,
//         ContainerAppEnvironment,
//         ContainerApp,
//         ContainerRegistry,
//         StorageAccount,
//         SQLServer,
//         SQLDatabase,
//         ServiceBus,
//         RedisCache,
//         KeyVault,
//         // PostGreSQLServer,
//         // PostGreSQLDatabase,
//     }
//
//     public class ContainerAppEnvironment
//     {
//         public string Name { get; set; }
//         public string Description { get; set; }
//         public string Namespace { get; set; }
//         public ResourceGroup ResourceGroupName { get; set; }
//     }
//     public class PulumiAzureService
//     {
//         public async Task<ResourceGroup> CreateResourceGroup(AppEntity appEntity, string location)
//         {
//             // projectAbbreviation + "-" + environmentName + "-" + "rg"
//             var resourceGroupName = $"{appEntity.Name}-rg";
//             var resourceGroup = new ResourceGroup(resourceGroupName, new ResourceGroupArgs
//             {
//                 ResourceGroupName = resourceGroupName,
//             });
//             return resourceGroup;
//         }
//
//         public async Task CreateSQLServer(AppEntity appEntity, ResourceGroup resourceGroup)
//         {
//             var sqlServer = new SqlServer("sqlserver", new SqlServerArgs
//             {
//                 ResourceGroupName = resourceGroup.Name,
//                 //Location = "West Europe",
//                 //AdministratorLogin = "admin",
//                 //AdministratorLoginPassword = "Password1234!",
//                 //Version = "12.0",
//             });
//         }
//
//         public async Task<Database> CreateSQLDatabase(string databaseName, ResourceGroup resourceGroup, SqlServer sqlServer)
//         {
//             var database = new Database(databaseName, new DatabaseArgs
//             {
//                 DatabaseName = databaseName,
//                 ResourceGroupName = resourceGroup.Name,
//             });
//             return database;    
//         }
//
//         public async Task<StorageAccount> CreateStorageAccount(AppEntity appEntity,  ResourceGroup resourceGroup)
//         {
//             var storageAccount = new StorageAccount(appEntity.Name + "sa", new StorageAccountArgs
//             {
//                 ResourceGroupName = resourceGroup.Name,
//                 AccountName = appEntity.Name + "sa",
//                 Location = resourceGroup.Location,
//                 AccessTier = AccessTier.Hot,
//                 AllowBlobPublicAccess = true,
//                 AllowCrossTenantReplication = false,
//                 AllowSharedKeyAccess = false,
//                 EnableHttpsTrafficOnly = true,
//                 IsHnsEnabled = false,
//                 IsSftpEnabled = false,
//                 MinimumTlsVersion = MinimumTlsVersion.TLS1_2,
//                 Sku = new StorageAccountSkuArgs
//                 {
//                     Name = StorageAccountSkuName.Standard_LRS,
//                 },
//                 Encryption = new EncryptionArgs
//                 {
//                     Services = new EncryptionServicesArgs
//                     {
//                         Blob = new EncryptionServiceArgs
//                         {
//                             Enabled = true,
//                         },
//                         File = new EncryptionServiceArgs
//                         {
//                             Enabled = true,
//                         },
//                     },
//                     KeySource = "Microsoft.Storage",
//                 },
//             });      
//             return storageAccount;
//         }
//
//         public async Task<BlobContainer> CreateBlobContainer(string blobContainerName, StorageAccount storageAccount)
//         {
//             var blobContainer = new BlobContainer(blobContainerName, new BlobContainerArgs
//             {
//                 AccountName = storageAccount.Name,
//                 PublicAccess = PublicAccess.Container
//             });
//             return blobContainer;
//         }
//
//         public async Task<AppServiceEnvironment> CreateContainerAppEnvironment(AppEntity appEntity, EnvironmentType environmentType, ResourceGroup resourceGroup)
//         {
//             var containerAppEnvironmentName = $"{appEntity.Name}-cae";
//
//             var containerAppEnvironment = new AppServiceEnvironmentArgs
//             {
//                 Name = containerAppEnvironmentName,
//                 ResourceGroupName = resourceGroup.Name,
//                 Location = resourceGroup.Location,
//                 Kind = "Linux",
//             };
//             return new AppServiceEnvironment(containerAppEnvironmentName, containerAppEnvironment);
//         }
//
//         public async Task CreateContainerApp(string appName, ContainerAppEnvironment containerAppEnvironment)
//         {
//             // projectName + "-" + environmentName + appName,
//             // projectName + "-" + environmentName + appName, new Azure.App.ContainerAppArgs
//             var containerApp = new ContainerApp("", new ContainerAppArgs()
//             {
//                 ContainerAppName = "appname",                
//                 ManagedEnvironmentId = "saasdplmgr-prod-capps-env0c3bc020",
//                 Configuration = new ConfigurationArgs
//                 {
//                     Ingress = new IngressArgs
//                     {
//                         External = true,
//                         TargetPort = 80
//                     },
//                     Registries = {
//                             new RegistryCredentialsArgs
//                             {
//                                 Server = "saasdplmgrprodacr14b46368.azurecr.io",
//                                 Username = "",
//                                 PasswordSecretRef = "egistrypassword"
//                             }
//                         }
//                 },
//                 Template = new TemplateArgs
//                 {
//                     Containers = {
//                             new ContainerArgs
//                             {
//                                 Name = "appName",
//                                 Image = "imageName"
//                             }
//                         }
//                 }
//             });
//             //var url = Output.Format($"https://{containerApp.Configuration.Apply(c => c.Ingress).Apply(i => i.Fqdn)}");
//
//         }
//        
//         public async Task<Namespace> CreateServiceBusNamespace(AppEntity appEntity, ResourceGroup resourceGroup)
//         {
//             var serviceBusNamespace = new Namespace(appEntity.Name + "sb", new NamespaceArgs
//             {
//                 ResourceGroupName = resourceGroup.Name,
//                 NamespaceName = appEntity.Name + "sb",
//                 Location = resourceGroup.Location,
//                 Sku = new ServiceBusSkuArgs.SBSkuArgs
//                 {
//                     Capacity = 1,
//                     Name = "Basic",
//                     Tier = "Basic",
//                 },
//             });
//             return serviceBusNamespace;
//         }
//         
//         public async Task<Queue> CreateServiceBusQueue(string queueName, Namespace serviceBusNamespace)
//         {
//             var queue = new Queue(queueName, new QueueArgs
//             {
//                 NamespaceName = serviceBusNamespace.Name,
//                 QueueName = queueName,
//             });
//             return queue;
//         }
//
//         public async Task<Vault> CreateKeyVault(AppEntity appEntity, ResourceGroup resourceGroup)
//         {
//             var keyVault = new Vault(appEntity.Name + "kv", new VaultArgs
//             {
//                 ResourceGroupName = resourceGroup.Name,
//                 Location = resourceGroup.Location,
//                 VaultName = "fd" + "kv",
//                 Properties = new VaultPropertiesArgs
//                 {
//                     TenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47",
//                     //AccessPolicies = new Pulumi.AzureNative.KeyVault.Inputs.VaultAccessPolicyEntryArgs
//                     //{
//                     //    TenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47",
//                     //    ObjectId = "00000000-0000-0000-0000-000000000000",
//                     //    ApplicationId = "00000000-0000-0000-0000-000000000000",
//                     //    Permissions = new Pulumi.AzureNative.KeyVault.Inputs.PermissionsArgs
//                     //    {
//                     //        Keys = new string[] { "get", "list" },
//                     //        Secrets = new string[] { "get", "list" },
//                     //        Certificates = new string[] { "get", "list" },
//                     //    },
//                     //},
//                 },
//                 
//                 //TenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47",
//                 //AccessPolicies = new KeyVaultAccessPolicyEntryArgs
//                 //{
//                 //    TenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47",
//                 //    ObjectId = "00000000-0000-0000-0000-000000000000",
//                 //    ApplicationId = "00000000-0000-0000-0000-000000000000",
//                 //    Permissions = new PermissionsArgs
//                 //    {
//                 //        Keys = new string[] { "get", "list" },
//                 //        Secrets = new string[] { "get", "list" },
//                 //        Certificates = new string[] { "get", "list" },
//                 //    },
//                 //},
//             });
//             return keyVault;
//         }
//         
//         public async Task<Redis> CreateRedisCache(AppEntity appEntity, ResourceGroup resourceGroup)
//         {
//             var redisCache = new Redis(appEntity.Name + "rc", new RedisArgs
//             {
//                 ResourceGroupName = resourceGroup.Name,
//                 Location = resourceGroup.Location,
//                 Sku = new RedisSkuArgs()
//                 {
//                     Family = "C",
//                     Name = "Basic",
//                     Capacity = 1,
//                 },
//             });
//             return redisCache;
//         }
//
//         private bool DoesNameAdhereToLimitations(string name, AzureResourceType resourceType)
//         {
//             switch (resourceType)
//             {
//                 case AzureResourceType.StorageAccount:
//                     if (name.Length > 24 && name.Contains("-"))
//                     {
//                         return false;
//                     }
//                     else
//                     {
//                         return true;
//                     }
//
//                 case AzureResourceType.ContainerApp:
//                     if (name.Length > 24)
//                     {
//                         return false;
//                     }
//                     else
//                     {
//                         return true;
//                     }
//                 case AzureResourceType.ContainerRegistry:
//                     if (name.Length > 50)
//                     {
//                         return false;
//                     }
//                     else
//                     {
//                         return true;
//                     }
//                 //case AzureResourceType.PostGreSQLServer:
//                 //    if (name.Length > 63)
//                 //    {
//                 //        return false;
//                 //    }
//                 //    else
//                 //    {
//                 //        return true;
//                 //    }
//                 //case AzureResourceType.PostGreSQLDatabase:
//                 //    if (name.Length > 63)
//                 //    {
//                 //        return false;
//                 //    }
//                 //    else
//                 //    {
//                 //        return true;
//                 //    }
//                 default:
//                     return false;
//             }
//         }
//     }
// }
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
//
// //await AppServiceEnvironment.CreateAsync(containerAppEnvironmentName, containerAppEnvironment);
// //return containerAppEnvironmentName;



