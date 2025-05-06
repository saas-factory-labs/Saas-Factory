using Projects;

// Aspire projects


IDistributedApplicationBuilder? builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<RedisResource>? cache = builder.AddRedis("cache");

IResourceBuilder<ProjectResource>? apiService = builder.AddProject<DeploymentManager_ApiService>("apiservice");

builder.AddProject<DeploymentManager_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();

// add aspire redis cache
//var apiService = builder.AddProject<DeploymentManager_ApiService>("apiservice");
//var batchService = builder.AddProject<DeploymentManager_BatchService>("batchservice");

#region create these extension methods

// 
// var certificatePassword = builder.CreateSslCertificateIfNotExists();
////var sqlPassword = builder.CreateStablePassword("sql-server-password");
////var sqlServer = builder.AddSqlServer("sql-server", sqlPassword, 9002)
////    .WithVolume("platform-platform-sql-server-data", "/var/opt/mssql");
/////var azureStorage = builder
//    .AddAzureStorage("azure-storage")
//    .RunAsEmulator(resourceBuilder =>
//    {
//        resourceBuilder.WithVolume("platform-platform-azure-storage-data", "/data");
//        resourceBuilder.WithBlobPort(10000);
//    }
//    )
//    .AddBlobs("blobs");

//builder
//    .AddContainer("mail-server", "axllent/mailpit")
//    .WithHttpEndpoint(9003, 8025)
//    .WithEndpoint(9004, 1025);

////var accountManagementDatabase = sqlServer
////    .AddDatabase("account-management-database", "account-management");

//CreateBlobContainer("avatars");

////var accountManagementApi = builder
////    .AddProject<AccountManagement_Api>("account-management-api")
////    .WithReference(accountManagementDatabase)
////    .WithReference(azureStorage);

////var accountManagementSpa = builder
////    .AddNpmApp("account-management-spa", "../account-management/WebApp", "dev")
////    .WithReference(accountManagementApi)
////    .WithEnvironment("CERTIFICATE_PASSWORD", certificatePassword);

////builder
////    .AddProject<AccountManagement_Workers>("account-management-workers")
////    .WithReference(accountManagementDatabase)
////    .WithReference(azureStorage);

////builder
////    .AddProject<AppGateway>("app-gateway")
////    .WithReference(accountManagementApi)
////    .WithReference(accountManagementSpa);

//builder.Build().Run();

//return;

//void CreateBlobContainer(string containerName)
//{
//    //var connectionString = builder.Configuration.GetConnectionString("blob-storage");

//    //new Task(() =>
//    //{
//    //    var blobServiceClient = new BlobServiceClient(connectionString);
//    //    var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
//    //    containerClient.CreateIfNotExists();
//    //}
//    //).Start();
//}

#endregion

IResourceBuilder<ProjectResource>? web = builder.AddProject<DeploymentManager_Web>("Web");
//.WithReference(apiService)
//.WithReference(batchService);

// Dependency Tracker for the Deployment Portal

builder.Build().Run();
