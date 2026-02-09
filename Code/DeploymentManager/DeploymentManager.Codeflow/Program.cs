using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Newtonsoft.Json;

namespace DeploymentManager.Codeflow;

internal class Program
{
    private const string _solutionPath = @"C:\Development\boligportal\Boligportal\Boligportal.sln";
    private const string _jsonFileOutputPath = @"C:\users\caspe\Downloads\classData.json";

    private static async Task Main(string[] args)
    {
        List<ClassDataInput>? classes = await AnalyzeSolution(_solutionPath);

        // Here, you would typically persist the data to a database or process it further
        foreach (ClassDataInput? classModel in classes)
        {
            Console.WriteLine(
                $"Class {classModel.ClassName} has {classModel.Methods.Count} methods and {classModel.Properties.Count} properties.");
            foreach (ClassDataInput? dependencyId in classModel.DependsOn)
                Console.WriteLine($"Class {classModel.ClassName} depends on class with ID {dependencyId}");
        }

        //var serviceCollection = new ServiceCollection();

        // write to json file
        string? json = JsonConvert.SerializeObject(classes, Formatting.Indented);
        File.WriteAllText(_jsonFileOutputPath, json);
    }

    /// <summary>
    ///     Analyze the solution and extract class data and dependencies
    /// </summary>
    /// <returns></returns>
    private static async Task<List<ClassDataInput>> AnalyzeSolution(string solutionPath)
    {
        var workspace = MSBuildWorkspace.Create();
        Solution? solution = await workspace.OpenSolutionAsync(solutionPath);

        ProjectDependencyGraph? projectDependencyGraph = solution.GetProjectDependencyGraph();

        IEnumerable<IEnumerable<ProjectId>>? dependencySets =
            solution.GetProjectDependencyGraph().GetDependencySets();
        IEnumerable<ProjectId>? topologicallySortedProjects =
            solution.GetProjectDependencyGraph().GetTopologicallySortedProjects();

        var classModels = new List<ClassDataInput>();

        foreach (Project? project in solution.Projects)
        {
            IImmutableSet<ProjectId>? projectsThatDirectlyDependOnThisProject = solution.GetProjectDependencyGraph()
                .GetProjectsThatDirectlyDependOnThisProject(project.Id);
            IImmutableSet<ProjectId>? projectsThatThisProjectDirectlyDependsOn = solution
                .GetProjectDependencyGraph().GetProjectsThatThisProjectDirectlyDependsOn(project.Id);

            foreach (Document? document in project.Documents)
            {
                SemanticModel? semanticModel = await document.GetSemanticModelAsync();
                SyntaxTree? syntaxTree = await document.GetSyntaxTreeAsync();

                if (syntaxTree is null)
                    continue;

                SyntaxNode? root = await syntaxTree.GetRootAsync();

                if (root is null || semanticModel is null)
                    continue;

                IEnumerable<ClassDeclarationSyntax> classes =
                    root.DescendantNodes().OfType<ClassDeclarationSyntax>();
                foreach (ClassDeclarationSyntax classDeclaration in classes)
                {
                    ClassDataInput classModel = CreateClassModel(classDeclaration, semanticModel);
                    classModels.Add(classModel);

                    ExtractProperties(classDeclaration, classModel);
                    ExtractMethods(classDeclaration, classModel, semanticModel);
                    ExtractDependencies(classDeclaration, classModel, semanticModel, classModels);
                }
            }
        }

        return classModels;
    }

    private static ClassDataInput CreateClassModel(ClassDeclarationSyntax classDeclaration,
        SemanticModel semanticModel)
    {
        string? className = classDeclaration.Identifier.Text;

        return new ClassDataInput { ClassName = className };
    }

    private static void ExtractProperties(ClassDeclarationSyntax classDeclaration, ClassDataInput classModel)
    {
        //var members = classDeclaration.Members;
        //foreach (var item in members)
        //{
        //    item.
        //}


        IEnumerable<PropertyDeclarationSyntax>? properties =
            classDeclaration.DescendantNodes().OfType<PropertyDeclarationSyntax>();
        foreach (PropertyDeclarationSyntax? property in properties)
        {
            string? propertyName = property.Identifier.Text;
            TypeSyntax? propertyType = property.Type;
            AccessorListSyntax? accessors = property.AccessorList;

            //foreach (var item in accessors.Accessors)
            //{
            //    var text = item.Body;
            //}

            classModel.Properties.Add(new PropertyInput
            {
                PropertyName = propertyName,
                ClassName = classModel.ClassName,
                PropertyType = propertyType.GetType().ToString()
            });
        }
    }

    private static void ExtractMethods(ClassDeclarationSyntax classDeclaration, ClassDataInput classModel,
        SemanticModel semanticModel)
    {
        IEnumerable<MethodDeclarationSyntax>? methods =
            classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();
        foreach (MethodDeclarationSyntax? method in methods)
        {
            string? methodName = method.Identifier.Text;
            classModel.Methods.Add(new MethodInput
            {
                MethodName = methodName,
                ClassName = classModel.ClassName,
                MethodReturnType = method.ReturnType.ToString()
            });
        }
    }

    private static void ExtractDependencies(ClassDeclarationSyntax classDeclaration, ClassDataInput classModel,
        SemanticModel semanticModel, List<ClassDataInput> classModels)
    {
        IEnumerable<MethodDeclarationSyntax>? methods =
            classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();
        foreach (MethodDeclarationSyntax? method in methods)
        {
            ISymbol? methodSymbol = semanticModel.GetDeclaredSymbol(method);
            IEnumerable<InvocationExpressionSyntax>? callees =
                method.DescendantNodes().OfType<InvocationExpressionSyntax>();

            foreach (InvocationExpressionSyntax? callee in callees)
            {
                var calleeSymbol = semanticModel.GetSymbolInfo(callee).Symbol as IMethodSymbol;
                if (calleeSymbol is not null)
                {
                    string? calleeClassName = calleeSymbol.ContainingType.Name;
                    ClassDataInput? targetClass = classModels.FirstOrDefault(c => c.ClassName == calleeClassName);

                    if (targetClass is not null && targetClass.ClassId != classModel.ClassId)
                    {
                        // FIX this!

                        //classModel.DependsOn.Add(targetClass.ClassName);


                        //foreach (var existingDependency in classModel.DependsOn)
                        //{
                        //    if (existingDependency == targetClass.ClassId)
                        //    {
                        //        return;
                        //    }
                        //}
                    }
                }
            }
        }
    }
}

public class MethodInput
{
    public int MethodId { get; init; }
    public string MethodName { get; init; }
    public string MethodReturnType { get; init; }
    public string ClassName { get; init; }
}

public class PropertyInput
{
    public int PropertyId { get; init; }
    public string PropertyType { get; init; }
    public string PropertyName { get; init; }
    public string ClassName { get; init; }
}

public class ClassDataInput
{
    public int ClassId { get; init; }
    public string ClassName { get; init; }
    public List<MethodInput> Methods { get; init; } = new();
    public List<PropertyInput> Properties { get; init; } = new();
    public List<ClassDataInput> DependsOn { get; init; } = new();
}


//public class ClassModel
//{
//    public int ClassId { get; set; }
//    public string ClassName { get; set; }
//    public List<MethodModel> Methods { get; set; } = new List<MethodModel>();
//    public List<PropertyModel> Properties { get; set; } = new List<PropertyModel>();
//    public List<int> DependsOn { get; set; } = new List<int>();
//}

//public class MethodModel
//{
//    public int MethodId { get; set; }
//    public string MethodName { get; set; }
//    public int ClassId { get; set; }
//}

//public class PropertyModel
//{
//    public int PropertyId { get; set; }
//    public string PropertyName { get; set; }
//    public int ClassId { get; set; }
//}

//public class DependencyModel
//{
//    public int DependencyId { get; set; }
//    public int SourceClassId { get; set; }
//    public int TargetClassId { get; set; }
//}

//public class DatabaseService
//{
//    //private readonly CodeContext _context;

//    //public DatabaseService(CodeContext context)
//    //{
//    //    _context = context;
//    //}

//    //public async Task SaveClassModelsAsync(List<ClassModel> classModels)
//    //{
//    //    foreach (var classModel in classModels)
//    //    {
//    //        _context.Classes.Add(classModel);
//    //        await _context.SaveChangesAsync();
//    //    }
//    //}
//}


//private static string solutionPath = @"C:\Development\boligportal\Boligportal\Boligportal.sln";


//serviceCollection.ConfigureHttpClient(client => client.BaseAddress = new Uri("https://nameless-brook-560077.eu-central-1.aws.cloud.dgraph.io/graphql"));

//IServiceProvider services = serviceCollection.BuildServiceProvider();

//IConferenceClient client = services.GetRequiredService<IConferenceClient>();

//var result = await client.GetSessions.ExecuteAsync();
//result.EnsureNoErrors();

//foreach (var session in result.Data.Sessions.Nodes)
//{
//    Console.WriteLine(session.Title);
//}

//static async Task UploadToDgraph2(List<ClassDataInput> classDataInputs)
//{
//    var endpoint = "https://nameless-brook-560077.eu-central-1.aws.cloud.dgraph.io/";

//    var dgraphEndpoint = Environment.GetEnvironmentVariable("DGRAPH_ENDPOINT", EnvironmentVariableTarget.User);
//    var dgraphApiKey = Environment.GetEnvironmentVariable("DGRAPH_API_KEY", EnvironmentVariableTarget.User);

//    //client.Connect("127.0.0.1:9080");
//    using (IDgraphMutationsClient client = DgraphDotNet.Clients.NewDgraphMutationsClient(endpoint))
//    {
//        client.Connect(endpoint);

//        await client.AlterSchema(
//            "Username: string @index(hash) .\n"
//            + "Password: password .");

//        var schemaResult = await client.SchemaQuery();
//        if (schemaResult.IsFailed)
//        {
//            Console.WriteLine($"Something went wrong getting schema.");
//            return;
//        }

//        Console.WriteLine("Queried schema and got :");
//        foreach (var predicate in schemaResult.Value.Schema)
//        {
//            Console.WriteLine(predicate.ToString());
//        }

//        while (true)
//        {
//            Console.WriteLine("Hi, please enter your new username");
//            var username = Console.ReadLine();

//            // use Upsert to test for a node and value, and create if
//            // not already in the graph as an atomic operation.
//            var result = await client.Upsert(
//                "Username",
//                GraphValue.BuildStringValue(username),
//                $"{{\"uid\": \"_:myBlank\", \"Username\": \"{username}\"}}",
//                "myBlank");

//            if (result.IsFailed)
//            {
//                Console.WriteLine("Something went wrong : " + result);
//                continue;
//            }

//            var (node, existed) = result.Value;

//            if (existed)
//            {
//                Console.WriteLine("This user already existed.  Try another username.");
//                continue;
//            }

//            Console.WriteLine("Hi, please enter a password for the new user");
//            var password = Console.ReadLine();

//            using (var txn = client.NewTransactionWithMutations())
//            {
//                var mutation = txn.NewMutation();

//                mutation.AddEdge()

//                //var mutation = txn.NewMutation();
//                //var property = Clients.BuildProperty(node, "Password", GraphValue.BuildPasswordValue(password));
//                //if (property.IsFailed)
//                //{
//                //    // ... something went wrong
//                //}
//                //else
//                //{
//                //    mutation.AddProperty(property.Value);
//                //    var err = await mutation.Submit();
//                //    if (err.IsFailed)
//                //    {
//                //        // ... something went wrong
//                //    }
//                //}
//                await txn.Commit();
//            }
//        }
//    }
//}

//static async Task CreateMutation2(List<ClassDataInput> classModels)
//{
//    var endpoint = "https://nameless-brook-560077.eu-central-1.aws.cloud.dgraph.io/";

//    var dgraphEndpoint = Environment.GetEnvironmentVariable("DGRAPH_ENDPOINT", EnvironmentVariableTarget.User);
//    var dgraphApiKey = Environment.GetEnvironmentVariable("DGRAPH_API_KEY", EnvironmentVariableTarget.User);
//    //var client = new DgraphClient(SlashChannel.Create("https://nameless-brook-560077.eu-central-1.aws.cloud.dgraph.io/graphql", dgraphApiKey));

//    //var channelCredentials = new ChannelCredentials(new SslCredentials(), CallCredentials.FromInterceptor((context, metadata) =>
//    //{
//    //    metadata.Add("Authorization", dgraphApiKey);
//    //    return Task.CompletedTask;
//    //}));


//    //var options = new GrpcChannelOptions
//    //{
//    //    //CompressionProviders = < ...>, // List of Grpc ICompressionProvider
//    //    Credentials = Grpc.Core.ChannelCredentials.Create()
//    //};

//    //GrpcChannel channel = dgra.Create(endpoint, dgraphApiKey);
//    //using var dgraphClient = DgraphClient.Create(channel);

//    //GrpcChannel channel = GrpcChannel.ForAddress(endpoint, new GrpcChannelOptions() { Credentials = new });

//    //GrpcChannel channel = DgraphCloudChannel.Create(ENDPOINT, API_KEY);

//    //var dgraphClient = new DgraphDotNet.(SlashChannel.Create(endpoint, dgraphApiKey));

//    //var client = new DgraphClient(SlashChannel.Create(endpoint, dgraphApiKey));

//    //var client = new DgraphClient(channel);

//    //using (var txn = dgraphClient.NewTransaction())
//    //{
//    //    //             SetJson = ByteString.CopyFromUtf8("{\"name\": \"Alice\"}")

//    //    //var app = new App { Name = "Fisk", Id = Guid.NewGuid() };
//    //    var json = JsonConvert.SerializeObject(classModels, Formatting.Indented);

//    //    // Convert the JSON string to a ByteString
//    //    //var byteString = ByteString.CopyFromUtf8(json);

//    //    var mutationBuilder = new MutationBuilder { SetJson = json };

//    //    var requestBuilder = new RequestBuilder()
//    //        .WithMutations(mutationBuilder);

//    //    var transactionResult = await txn.Mutate(requestBuilder);
//    //    var result = await txn.Commit();

//    //    //var response = await client.NewTransaction().MutateAsync(mutation);
//    //    //Console.WriteLine("Mutation succeeded");

//    //    if (result.IsSuccess)
//    //    {
//    //        await Console.Out.WriteLineAsync("added class");
//    //    }
//    //    else
//    //    {
//    //        await Console.Out.WriteLineAsync("failed to add class");
//    //    }
//    }
//}


//static async Task DgraphCloudTest2()
//{
//    var endpoint = "https://nameless-brook-560077.eu-central-1.aws.cloud.dgraph.io/graphql";

//    var classDataList = new List<ClassDataInput>
//        {
//            new ClassDataInput
//            {
//                ClassId = 0,
//                ClassName = "Boligportal_ApiService",
//                Methods = new List<MethodInput>(),
//                Properties = new List<PropertyInput>
//                {
//                    new PropertyInput { PropertyId = 0, PropertyName = "ProjectPath", ClassId = 0 }
//                },
//                DependsOn = new List<ClassDataInput>()
//            },
//            // Add more ClassDataInput instances as needed
//        };

//    HttpClient client = new HttpClient();
//    client.BaseAddress = new Uri(endpoint);

//    //var services = new ServiceCollection();
//    //services.add("DgraphClient", client =>
//    //{
//    //    client.BaseAddress = new Uri(endpoint);
//    //    client.DefaultRequestHeaders.Add("X-Auth-Token", apiKey);
//    //});
//    //services.AddGraphQLClient("DgraphClient")
//    //        .ConfigureHttpClient(client => client.BaseAddress = new Uri(endpoint));
//    //var serviceProvider = services.BuildServiceProvider();
//    //var graphQLClient = serviceProvider.GetRequiredService<IGraphQLClient>();

//    // create a new strawberry shake client


//    string mutation = @"
//            mutation AddClassData($input: [ClassDataInput!]!) {
//                addClassData(input: $input) {
//                    classData {
//                        ClassId
//                        ClassName
//                    }
//                }
//            }";

//    //var request = QueryRequestBuilder.New()
//    //    .SetQuery(mutation)
//    //    .SetVariableValue("input", classDataList)
//    //    .Create();

//    //try
//    //{
//    //    var response = await graphQLClient.ExecuteAsync(request);
//    //    Console.WriteLine("Mutation executed successfully.");
//    //    Console.WriteLine(response.ToJson());
//    //}
//    //catch (GraphQLException ex)
//    //{
//    //    Console.WriteLine($"GraphQL Error: {ex.Message}");
//    //}
//    //catch (HttpRequestException ex)
//    //{
//    //    Console.WriteLine($"HTTP Request Error: {ex.Message}");
//    //}
//    //catch (Exception ex)
//    //{
//    //    Console.WriteLine($"Unexpected Error: {ex.Message}");
//    //}
//}


//static async Task Main(string[] args)
//{
//    var solution = await workspace.OpenSolutionAsync(solutionPath);
//    Console.WriteLine("Solution loaded.");
//    // Proceed to analyze the solution

//    await ProcessProjects(solution);

//    // Output the class dependencies
//    foreach (var kvp in classDependencies)
//    {
//        Console.WriteLine($"Class {kvp.Key} depends on: {string.Join(", ", kvp.Value)}");
//    }

//    foreach (var kvp in classProperties)
//    {
//        Console.WriteLine($"Class {kvp.Key} has properties: {string.Join(", ", kvp.Value)}");
//    }

//    //foreach (var property in classProperties)
//    //{
//    //    var propertyName = property.Identifier.Text;
//    //    classProperties[className].Add(propertyName);
//    //}
//}

////static async Task ProcessProperties(List<PropertyDeclarationSyntax> properties)
////{

////}

//static async Task ProcessClasses(List<ClassDeclarationSyntax> classes)
//{
//    foreach (var classDeclaration in classes)
//    {
//        var className = classDeclaration.Identifier.Text;
//        if (!classDependencies.ContainsKey(className))
//        {
//            classDependencies[className] = new HashSet<string>();
//        }

//        if (!classProperties.ContainsKey(className))
//        {
//            classProperties[className] = new List<string>();
//        }

//        var properties = classDeclaration.DescendantNodes().OfType<PropertyDeclarationSyntax>();


//        var methodDeclarations = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();
//        foreach (var method in methodDeclarations)
//        {
//            var methodSymbol = semanticModel.GetDeclaredSymbol(method);
//            var callees = method.DescendantNodes().OfType<InvocationExpressionSyntax>();

//            foreach (var callee in callees)
//            {
//                var calleeSymbol = semanticModel.GetSymbolInfo(callee).Symbol as IMethodSymbol;
//                if (calleeSymbol is not null)
//                {
//                    var calleeClass = calleeSymbol.ContainingType.Name;
//                    if (calleeClass != className)
//                    {
//                        classDependencies[className].Add(calleeClass);
//                    }
//                }
//            }
//        }
//    }

//}

//static async Task ProcessCodeFiles(List<Document> documents)
//{
//    foreach (var document in documents)
//    {
//        var semanticModel = await document.GetSemanticModelAsync();
//        var syntaxTree = await document.GetSyntaxTreeAsync();
//        var root = await syntaxTree.GetRootAsync();

//        // Analyze the syntax tree and semantic model
//        AnalyzeSyntaxTree(syntaxTree);
//        AnalyzeSemanticModel(semanticModel);

//        var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

//    }
//}

//static async Task ProcessProjects(Solution solution)
//{
//    foreach (var project in solution.Projects)
//    {
//        Console.WriteLine($"Project: {project.Name}");

//        await ProcessCodeFiles(project.Documents.ToList());

//    }

//}

//static void AnalyzeSyntaxTree(SyntaxTree syntaxTree)
//{
//    var root = syntaxTree.GetRoot();
//    var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

//    foreach (var classDeclaration in classDeclarations)
//    {
//        Console.WriteLine($"Class: {classDeclaration.Identifier.Text}");
//        var methodDeclarations = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();

//        foreach (var method in methodDeclarations)
//        {
//            Console.WriteLine($"  Method: {method.Identifier.Text}");
//        }
//    }
//}

//static void AnalyzeSemanticModel(SemanticModel semanticModel)
//{
//    // Use semanticModel to get more detailed information
//    // about the types and symbols in the code
//}

////static void PrintDependencies(Project project)
////{
////    // Analyze and print dependencies between classes and methods
////}

//static void SaveToDgraph()
//{
//    // Save the class dependencies to a Dgraph file and then to dgraph cloud database to be visualized with blazor.diagrams
//    // visualize the tree using blazor diagrams in depenceny tracker page in deployment manager web app
//}

//private static async Task CreateDgraphClient(DgraphClient _dgraphClient)
//{


//    //var channel = GrpcChannel.ForAddress(dgraphEndpoint, new GrpcChannelOptions
//    //{
//    //    Credentials = ChannelCredentials.Create(new SslCredentials(), CallCredentials.FromInterceptor((context, metadata) =>
//    //    {
//    //        metadata.Add("Authorization", $"Bearer {dgraphApiKey}");
//    //        return Task.CompletedTask;
//    //    })),
//    //    HttpClient = new HttpClient(new HttpClientHandler
//    //    {
//    //        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
//    //        SslProtocols = System.Security.Authentication.SslProtocols.Tls12

//    //    })
//    //});
//}


//private static async Task DefineSchema(DgraphClient client)
//{
//    var operation = new Operation
//    {

//         = @"
//    type Class {
//      className: String! @index(term) .
//      methods: [Method] .
//      properties: [Property] .
//      dependsOn: [Class] .
//    }

//    type Method {
//      methodName: String! @index(term) .
//      belongsTo: Class .
//    }

//    type Property {
//      propertyName: String! @index(term) .
//      belongsTo: Class .
//    }
//    "
//    };

//    await client.Alter(operation);
//    Console.WriteLine("Schema defined.");
//}

//private static async Task UploadData(List<ClassDataInput> classModels)
//{
//    var jsonData = JsonConvert.SerializeObject(classModels);

//    //Dgraph.Transactions.MutationBuilder mutationBuilder = new Dgraph.Transactions.MutationBuilder();
//    //mutationBuilder.SetJson = jsonData;
//    //var RequestBuilder = new Dgraph.Transactions.RequestBuilder
//    //{ };
//    //RequestBuilder.WithMutations(mutationBuilder);
//    //var transaction = client.NewTransaction();
//    //var result = await transaction.Mutate(RequestBuilder);

//    // "https://your-dgraph-cloud-endpoint";
//    var dgraphEndpoint = Environment.GetEnvironmentVariable("DGRAPH_ENDPOINT", EnvironmentVariableTarget.User);
//    var dgraphApiKey = Environment.GetEnvironmentVariable("DGRAPH_API_KEY", EnvironmentVariableTarget.User);
//    //var client = new DgraphClient(SlashChannel.Create("https://nameless-brook-560077.eu-central-1.aws.cloud.dgraph.io/graphql", dgraphApiKey));
//    // kig på hvordan jeg har lavet min tidligere dgraph kode i deployment manager depenceny tracker

//    //using (var txn = client.NewTransaction())
//    //{
//    //    var json = JsonConvert.SerializeObject("alice");
//    //    var result = await txn.Mutate(new RequestBuilder().WithMutations(new MutationBuilder { SetJson = json }));
//    //    await txn.Commit();

//    //    if (result.IsSuccess) Console.WriteLine("successfully uploaded to dgraph cloud");
//    //    if (result.IsFailed) Console.WriteLine("failed to upload to dgraph cloud");
//    //}


//    //if (result.IsSuccess) await .Commit(); Console.WriteLine(" successfully uploaded to dgraph cloud");
//    //if (result.IsFailed) await transaction.Discard(); Console.WriteLine("failed to upload to dgraph cloud");


//}

//private static string GenerateUid()
//{
//    return $"_:uid{Guid.NewGuid().ToString().Replace("-", "")}";
//}

//static async Task Test()
//{
//    //var endpoint = "https://nameless-brook-560077.eu-central-1.aws.cloud.dgraph.io/graphql";            
//    var endpoint = "grpc://nameless-brook-560077.eu-central-1.aws.cloud.dgraph.io/graphql";
//    var dgraphApiKey = Environment.GetEnvironmentVariable("DGRAPH_API_KEY", EnvironmentVariableTarget.User);

//    //var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
//    //{
//    //    metadata.Add("X-Auth-Token", dgraphApiKey);
//    //    return Task.CompletedTask;
//    //});
//    //ChannelCredentials channelCredentials = ChannelCredentials.Create(new SslCredentials(), callCredentials);
//    //var credentials = ChannelCredentials.Create(channelCredentials, callCredentials);

//    var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
//    {
//        metadata.Add("X-Auth-Token", dgraphApiKey);
//        return Task.CompletedTask;
//    });

//    ChannelCredentials channelCredentials = ChannelCredentials.Create(new SslCredentials(), callCredentials);

//    //var channel = new Channel(endpoint, channelCredentials);

//    IDgraphMutationsClient client = DgraphDotNet.Clients.NewDgraphMutationsClient(endpoint);
//    client.Connect(endpoint, channelCredentials);

//    // JSON data to be uploaded
//    var jsonData = @"
//        [
//            {
//                ""ClassId"": 0,
//                ""ClassName"": ""Boligportal_ApiService"",
//                ""Methods"": [],
//                ""Properties"": [{ ""PropertyId"": 0, ""PropertyName"": ""ProjectPath"", ""ClassId"": 0 }],
//                ""DependsOn"": []
//            },
//            {
//                ""ClassId"": 0,
//                ""ClassName"": ""Boligportal_Web"",
//                ""Methods"": [],
//                ""Properties"": [{ ""PropertyId"": 0, ""PropertyName"": ""ProjectPath"", ""ClassId"": 0 }],
//                ""DependsOn"": []
//            },
//            {
//                ""ClassId"": 0,
//                ""ClassName"": ""Boligportal_AppHost"",
//                ""Methods"": [],
//                ""Properties"": [{ ""PropertyId"": 0, ""PropertyName"": ""ProjectPath"", ""ClassId"": 0 }],
//                ""DependsOn"": []
//            },
//            {
//                ""ClassId"": 0,
//                ""ClassName"": ""Extensions"",
//                ""Methods"": [
//                    { ""MethodId"": 0, ""MethodName"": ""AddServiceDefaults"", ""ClassId"": 0 },
//                    { ""MethodId"": 0, ""MethodName"": ""ConfigureOpenTelemetry"", ""ClassId"": 0 },
//                    { ""MethodId"": 0, ""MethodName"": ""AddOpenTelemetryExporters"", ""ClassId"": 0 },
//                    { ""MethodId"": 0, ""MethodName"": ""AddDefaultHealthChecks"", ""ClassId"": 0 },
//                    { ""MethodId"": 0, ""MethodName"": ""MapDefaultEndpoints"", ""ClassId"": 0 }
//                ],
//                ""Properties"": [],
//                ""DependsOn"": []
//            },
//            {
//                ""ClassId"": 0,
//                ""ClassName"": ""EjerData"",
//                ""Methods"": [],
//                ""Properties"": [
//                    { ""PropertyId"": 0, ""PropertyName"": ""primaerKontakt"", ""ClassId"": 0 },
//                    { ""PropertyId"": 0, ""PropertyName"": ""actualShare"", ""ClassId"": 0 },
//                    { ""PropertyId"": 0, ""PropertyName"": ""tinglystShare"", ""ClassId"": 0 },
//                    { ""PropertyId"": 0, ""PropertyName"": ""name"", ""ClassId"": 0 },
//                    { ""PropertyId"": 0, ""PropertyName"": ""coName"", ""ClassId"": 0 },
//                    { ""PropertyId"": 0, ""PropertyName"": ""address"", ""ClassId"": 0 }
//                ],
//                ""DependsOn"": []
//            }
//        ]";

//    // Create the mutation
//    //var mutation = new Mutation
//    //{
//    //    SetJson = ByteString.CopyFromUtf8(jsonData)
//    //};

//    var txn = client.NewTransactionWithMutations();

//    var respone = await txn.Mutate(jsonData);

//    if (respone.IsSuccess)
//    {
//        Console.WriteLine("success");
//    }
//    else
//    {
//        Console.WriteLine("failed");
//    }

//    //var values = respone.Value.ToDictionary();

//    //foreach (var value in values)
//    //{
//    //    Console.WriteLine(value.Key);
//    //}


//    //using (var txn = client.NewTransactionWithMutations())
//    //{
//    //    await txn.Mutate(jsonData);

//    //    //var mutation = txn.NewMutation();
//    //    //var node = NewNode().Value;
//    //    //var property = Clients.BuildProperty(node, "someProperty", GraphValue.BuildStringValue("HI"));

//    //    //mutation.AddProperty(property.Value);
//    //    //var err = await mutation.Submit();
//    //    //if (err.IsFailed)
//    //    //{
//    //    //    // ... something went wrong
//    //    //}
//    //    await txn.Commit();
//    //}

//    // Create the request and commit
//    //var request = new Request
//    //{
//    //    CommitNow = true
//    //};

//    //request.Mutations.Add(mutation);

//    //client.(request);

//    //try
//    //{
//    //    var response = await client.ex.Do(request);
//    //    Console.WriteLine("Data uploaded successfully");
//    //}
//    //catch (RpcException ex)
//    //{
//    //    Console.WriteLine($"Error uploading data: {ex.Message}");
//    //}
//    //finally
//    //{
//    //    await channel.ShutdownAsync();
//    //}
//}
