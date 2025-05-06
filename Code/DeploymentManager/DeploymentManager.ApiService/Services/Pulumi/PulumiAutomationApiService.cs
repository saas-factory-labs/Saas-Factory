// // using Pulumi;
// // using Pulumi.Automation;
// // using DeploymentManager.DeploymentPortal.ApiService.Services.Pulumi.Azure;
// using DeploymentPortal.ApiService.Domain.Interfaces;
// using Domain.DTOs.Project;
// using Domain.Entities;

// namespace DeploymentPortal.Services.Pulumi
// {
//     public class PulumiAutomationApiService : IInfrastructureCodeProvider
//     {
//         // private PulumiAzureService _pulumiAzureService;
//         //private LocalWorkspace _localWorkspace;

//         //private string projectName;
//         //private string projectAbbreviation;
//         //private string environmentName;
//         //private string ResourceGroupName;
//         //private string databaseName;
//         //private string SQLServerName;
//         //private List<string> blobContainerNames = new List<string>();

//         // public PulumiAutomationApiService(PulumiAzureService pulumiAzureService)
//         // {
//         //     _pulumiAzureService = pulumiAzureService;
//         // }

//         public async Task<bool> CreateOrUpdateProject(ProjectEntity project, string environmentName)
//         {
//             Console.WriteLine("Running CreateOrUpdateProject");

//             //projectAbbreviation = inputModel.ProjectAbbreviation;
//             //blobContainerNames = inputModel.BlobContainerNames;
//             //environmentName = Enum.Parse<EnvironmentType>(inputModel.Environment.ToString()).ToString();
//             //ResourceGroupName = projectAbbreviation + "-" + inputModel.ProjectAbbreviation.ToString() + "-" + "rg";
//             //databaseName = projectAbbreviation + "-" + environmentName.ToString() + "-" + "db";

//             //Console.WriteLine(inputModel.ProjectName);

//             bool wasInfrastructureUpdated = await CreateOrUpdateProjectInfrastructure(project, environmentName);
//             if (!wasInfrastructureUpdated) return false;
//             bool wasSuccess = await CreateOrUpdateProjectEnvironmentDatabase(project, environmentName);

//             if (!wasSuccess) return false;
//             return true;
//         }

//         public async Task<bool> CreateOrUpdateProjectInfrastructure(ProjectEntity projectEntity, string environmentName)
//         {
//             // create project infrastructure for a specific app environment (e.g. prod, dev, test, demo) and configure use of shared infrastructure resources

//             Console.WriteLine("Running CreateOrUpdateInfrastructure for project" + projectEntity.Name);

//             var program = PulumiFn.Create(async () => { });

//             var stackArgs = new InlineProgramArgs(projectEntity.Name, environmentName, program);
//             WorkspaceStack? stack = await LocalWorkspace.CreateOrSelectStackAsync(stackArgs);
//             UpResult? result = await stack.UpAsync();

//             if (result.StandardError is not null || result.StandardError != "") return false;
//             Console.WriteLine("Infrastructure created successfully");
//             return true;

//         }

//         public async Task<bool> CreateOrUpdateProjectEnvironmentDatabase(ProjectEntity projectEntity, string environmentName)
//         {
//             Console.WriteLine("Running CreateOrUpdateProjectEnvironmentDatabase");
//             // create azure sql database or postgre sql flexible burstable server for a specific app environment (e.g. prod, dev, test, demo)

//             var program = PulumiFn.Create(async () => { });

//             var stackArgs = new InlineProgramArgs(projectEntity.Name, environmentName, program);
//             WorkspaceStack? stack = await LocalWorkspace.CreateOrSelectStackAsync(stackArgs);

//             UpResult? result = await stack.UpAsync();

//             if (result.StandardError is not null || result.StandardError != "") return false;

//             Console.WriteLine("database created successfully");
//             return true;
//         }

//         public async Task<bool> CreateOrUpdateContainerWorkload(ProjectEntity projectEntity, string environmentName, string appName, string imageName)
//         {
//             var program = PulumiFn.Create(async () =>
//             { });

//             var stackArgs = new InlineProgramArgs(projectEntity.Name, environmentName, program);
//             WorkspaceStack? stack = await LocalWorkspace.CreateOrSelectStackAsync(stackArgs);
//             UpResult? result = await stack.UpAsync();

//             if (result.StandardError is not null || result.StandardError != "") return false;

//             Console.WriteLine("container app deployed successfully");
//             return true;

//         }

//         public void Dispose()
//         {
//             throw new NotImplementedException();
//         }

//         public Task<bool> CreateOrUpdateProject(ProjectRequestDto projectRequestDto)
//         {
//             throw new NotImplementedException();
//         }

//         public Task<bool> CreateOrUpdateProjectInfrastructure(ProjectEntity projectEntity)
//         {
//             throw new NotImplementedException();
//         }

//         public Task<bool> CreateOrUpdateProjectEnvironmentDatabase(ProjectEntity projectEntity)
//         {
//             throw new NotImplementedException();
//         }

//         public Task<bool> CreateOrUpdateContainerWorkload(string appName, string imageName)
//         {
//             throw new NotImplementedException();
//         }

//         // [Output("url")]
//         // public Output<string> Url { get; set; }
//     }
// }

// //private async Task<bool> CreateOrUpdateContainerWorkload(string appName, string imageName, string registryUsername, string registrypassword)
// //{

// //}

// #region Notes 

// // project name: Cloudcostify
// // project name abbreviation: cfy
// // environment: prod
// // combination: <project abbreviation>-<environment>-<resource type>
// // storage account: cfyprodstoacc

// // https://www.pulumi.com/blog/azure-container-apps/
// // https://github.com/pulumi/examples/blob/master/azure-ts-containerapps/node-app/Dockerfile
// // https://lucid.app/lucidchart/ca4abe5d-9bb3-4b1c-a348-b8b28201827f/edit?beaconFlowId=1DE8EA7C7FE78819&invitationId=inv_edce297b-26e3-461f-b836-a0912886fe2e&page=H.YTrnmeERFw#
// // https://learn.microsoft.com/en-us/azure/azure-sql/database/saas-tenancy-app-design-patterns?view=azuresql


// #endregion


// // Cloudflare DDOS protection (free - unmetered)

// // InputModel inputModel

// //    var zoneId = "<your-cloudflare-zone-id>";
// //    var domainName = "<saas-project-domain-name>";

// //    // Create a new Cloudflare DDoS protection group
// //    var ddosProtection = new DdosProtection("ddos-protection", new DdosProtectionArgs
// //    {
// //        ZoneId = zoneId,
// //        Name = "pulumi-ddos-protection",
// //        Default = true,
// //    });

// //    // Enable the DDoS protection for the specified domain
// //    var domainDdosProtection = new DdosProtectionRule("domain-ddos-protection", new DdosProtectionRuleArgs
// //    {
// //        ZoneId = zoneId,
// //        Domain = domainName,
// //        DdosProtectionId = ddosProtection.Id,
// //    });

// //    var filter = new Filter("block-malicicious-actors", new Pulumi.Cloudflare.FilterArgs
// //    {
// //        Description = "Block IP range",
// //        Expression = "(ip.src in {192.0.2.0/24})",
// //        Paused = false
// //    });

// //    var firewallRule = new Pulumi.Cloudflare.FirewallRule("ddos-mitigation-firewall-rule", new Pulumi.Cloudflare.FirewallRuleArgs 
// //    {
// //        Action = "block",
// //        Description = "Block IP range",
// //        FilterId = filter.Id,
// //        Priority = 1,
// //        ZoneId = "your_zone_id"
// //    });

// //    return new Dictionary<string, object?>
// //{
// //    { "filterId", filter.Id },
// //    { "firewallRuleId", firewallRule.Id }
// //};


// // discarded code
// /*

//  var credentials = Output.Tuple(resourceGroup.Name, registry.Name)
//                         .Apply(items =>
//                 ListRegistryCredentials.InvokeAsync(new ListRegistryCredentialsArgs
//                 {
//                     ResourceGroupName = items.Item1,
//                     RegistryName = items.Item2
//                 }));
//                 var adminUsername = credentials.Apply(c => c.Username);
//                 var adminPassword = credentials.Apply(c => c.Passwords[0].Value);

//                 // var kubeEnv = new KubeEnvironment("", new KubeEnvironmentArgs
//                 // {
//                 //     ResourceGroupName = resourceGroup.Name,
//                 // });

//  // to destroy our program, we can run "dotnet run destroy"
//             // var destroy = args.Any() && args[0] == "destroy";

//             // var projectName = projectAbbreviation;
//             //     var stackName = environment.ToString();

//             //     // create or select a stack matching the specified name and project
//             //     // this will set up a workspace with everything necessary to run our inline program (program)
//             //     var stackArgs = new InlineProgramArgs(projectName, stackName, program);
//             //     var stack = await LocalWorkspace.CreateOrSelectStackAsync(stackArgs);
//             //     var preview = await stack.PreviewAsync();
//             //     System.Console.WriteLine(preview.ChangeSummary);
//             // System.Console.WriteLine(preview.StandardOutput);

//             // var result = await stack.UpAsync();


//             // var up = await stack.UpAsync();

// */


// // discarded code

// // discarded because the database schema will be created using EF core code first approach when developing the app locally
// // private async Task CreateInitialDatabaseMigration()
// //         {
// //             // should this be done using EF core code first approach migration instead of using pulumi?

// //             // make sure the table exists
// //             const string createTableQuery = @"
// //                                             CREATE TABLE IF NOT EXISTS hello_pulumi(
// //                                             id int(9) NOT NULL PRIMARY KEY,
// //                                             color varchar(14) NOT NULL);";

// //             using var createCommand = new ngsql MySqlCommand(createTableQuery, connection);
// //     await createCommand.ExecuteNonQueryAsync();

// //     // seed the table with some data to start
// //     const string seedTableQuery = @"
// // INSERT IGNORE INTO hello_pulumi (id, color)
// // VALUES
// //     (1, 'Purple'),
// //     (2, 'Violet'),
// //     (3, 'Plum');";

// //             using var seedCommand = new MySqlCommand(seedTableQuery, connection);
// //     await seedCommand.ExecuteNonQueryAsync();

// //     Console.WriteLine("rows inserted!");
// //             Console.WriteLine("querying to verify data...");

// //             const string readTableQuery = "SELECT COUNT(*) FROM hello_pulumi;";
// //             using var readCommand = new MySqlCommand(readTableQuery, connection);
// //     var readResult = await readCommand.ExecuteScalarAsync();
// //     Console.WriteLine($"Result: {readResult} rows");

// //             Console.WriteLine("database, table, and rows successfully configured");
// //         }


// //public enum SendGridResourceType // SendGrid resource types
// //{
// //    Email,
// //    Template,
// //    Contact,
// //    List,
// //    MarketingCampaign,
// //    MarketingCampaignTemplate,
// //    MarketingCampaignContact,
// //    MarketingCampaignList,
// //}



