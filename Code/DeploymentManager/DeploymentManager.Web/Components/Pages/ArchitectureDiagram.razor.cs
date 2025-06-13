using System.Reflection;
using Blazor.Diagrams;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

//using Microsoft.OpenApi.Models;

namespace DeploymentManager.Web.Pages;

public partial class ArchitectureDiagram
{
    public BlazorDiagram Diagram { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var options = new BlazorDiagramOptions
        {
            AllowMultiSelection = true,
            Zoom =
            {
                Enabled = true,
                ScaleFactor = 1.2,
                Inverse = false
            },
            AllowPanning = true,
            GridSize = 20,
            Links =
            {
                DefaultRouter = new NormalRouter(),
                DefaultPathGenerator = new SmoothPathGenerator()
            }
        };

        Diagram = new BlazorDiagram(options);

        await LoadDiagram();
    }

    private async Task LoadDiagram()
    {
        // Get Open Api controllers 
        //ExtractApiControllers();

        // Get Open Api spec
        //ExtractOpenApiSpec();


        // Add nodes for API methods, services, databases, message queues, and topics
        NodeModel? apiNode = CreateNode("API Method", new Point(100, 100));
        NodeModel? serviceNode = CreateNode("Service", new Point(300, 100));
        NodeModel? databaseNode = CreateNode("Database", new Point(500, 100));
        NodeModel? messageQueueNode = CreateNode("Message Queue", new Point(700, 100));
        NodeModel? topicNode = CreateNode("Topic", new Point(900, 100));

        Diagram.Nodes.Add(apiNode);
        Diagram.Nodes.Add(serviceNode);
        Diagram.Nodes.Add(databaseNode);
        Diagram.Nodes.Add(messageQueueNode);
        Diagram.Nodes.Add(topicNode);

        // Define connections between the nodes to represent the architecture
        Diagram.Links.Add(new LinkModel(apiNode, serviceNode));
        Diagram.Links.Add(new LinkModel(serviceNode, databaseNode));
        Diagram.Links.Add(new LinkModel(serviceNode, messageQueueNode));
        Diagram.Links.Add(new LinkModel(messageQueueNode, topicNode));

        // Automatically populate the dependency data for the nodes
        PopulateDependencies();
    }

    private NodeModel CreateNode(string title, Point position)
    {
        return new NodeModel(position)
        {
            Title = title,
            Size = new Size(150, 50)
        };
    }

    private void PopulateDependencies()
    {
        // Example of automatically populating dependencies
        var dependencies = new Dictionary<string, List<string>>
        {
            { "API Method", new List<string> { "Service" } },
            { "Service", new List<string> { "Database", "Message Queue" } },
            { "Message Queue", new List<string> { "Topic" } }
        };

        foreach (NodeModel? node in Diagram.Nodes)
            if (dependencies.ContainsKey(node.Title))
                foreach (string? dependency in dependencies[node.Title])
                {
                    NodeModel? targetNode = Diagram.Nodes.First(n => n.Title == dependency);
                    if (targetNode is not null) Diagram.Links.Add(new LinkModel(node, targetNode));
                }
    }

    private void ExtractApiControllers()
    {
        Console.Clear();

        Console.WriteLine("Extracting api controller metadata");

        var assembly =
            Assembly.LoadFile(
                @"C:\\Development\\Development-Projects\\SaaS-Factory\\Code\\AppBlueprint\\Shared-Modules\\AppBlueprint.Presentation.ApiModule\\bin\\Debug\\net9.0\AppBlueprint.Presentation.ApiModule.dll");

        // Load the target assembly by its name.
        //var assembly = Assembly.Load("AppBlueprint.Presentation.ApiModule");

        // Optionally, if your controllers aren�t public, use GetTypes() instead:
        // var types = assembly.GetTypes();
        Type[]? controllers = assembly.GetExportedTypes()
            .Where(t => t.IsClass
                        && !t.IsAbstract
                        && typeof(ControllerBase).IsAssignableFrom(t)
                        && t.GetCustomAttributes(typeof(ApiControllerAttribute), true).Any())
            .ToArray();

        foreach (Type? controller in controllers) Console.WriteLine(controller.FullName);

        //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        //foreach (var assembly in assemblies)
        //{
        //    //Console.WriteLine("Processing assembly : " + assembly.FullName);

        //    var controllerTypes = assembly.GetTypes()
        //        .Where(t => t.IsSubclassOf(typeof(ControllerBase)) || t.GetCustomAttributes<ApiControllerAttribute>().Any());

        //    foreach (var controllerType in controllerTypes)
        //    {
        //        Console.WriteLine("Found controller type : " + controllerType.FullName);

        //        var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        //        foreach (var method in methods)
        //        {
        //            Console.WriteLine("Found api method  : " + method.Name);

        //            var routeAttributes = method.GetCustomAttributes<RouteAttribute>();
        //            foreach (var routeAttribute in routeAttributes)
        //            {
        //                Console.WriteLine("Found route attribute : " + routeAttribute.Name);

        //                var route = routeAttribute.Template;
        //                var node = CreateNode($"{controllerType.Name}.{method.Name}", new Point(0, 0));
        //                Diagram.Nodes.Add(node);
        //            }
        //        }
        //    }
        //}
    }

    private void ExtractOpenApiSpec()
    {
        string? openApiSpecPath =
            @"C:\Development\Development-Projects\SaaS-Factory\Writerside\specifications\swagger.json";

        using FileStream? stream = File.OpenRead(openApiSpecPath);
        OpenApiDocument? openApiDocument = new OpenApiStreamReader().Read(stream, out OpenApiDiagnostic? diagnostic);

        foreach (KeyValuePair<string, OpenApiPathItem> path in openApiDocument.Paths)
        foreach (KeyValuePair<OperationType, OpenApiOperation> operation in path.Value.Operations)
        {
            NodeModel? node = CreateNode($"{operation.Key} {path.Key}", new Point(0, 0));
            Diagram.Nodes.Add(node);

            Console.WriteLine("node :" + node.Title);
        }
    }

    //private void ConfigureOpenTelemetry()
    //{
    //    Sdk.CreateTracerProviderBuilder()
    //        .AddSource("MyApp")
    //        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyApp"))
    //        .AddAspNetCoreInstrumentation()
    //        .AddHttpClientInstrumentation()
    //        .AddJaegerExporter()
    //        .Build();
    //}

    private void VisualizeTelemetryData()
    {
        // Example of visualizing telemetry data
        var telemetryData = new List<(string Source, string Target)>
        {
            ("API Method", "Service"),
            ("Service", "Database"),
            ("Service", "Message Queue"),
            ("Message Queue", "Topic")
        };

        foreach ((string Source, string Target) data in telemetryData)
        {
            NodeModel? sourceNode = Diagram.Nodes.First(n => n.Title == data.Source);
            NodeModel? targetNode = Diagram.Nodes.First(n => n.Title == data.Target);
            if (sourceNode is not null && targetNode is not null)
                Diagram.Links.Add(new LinkModel(sourceNode, targetNode));
        }
    }
}
