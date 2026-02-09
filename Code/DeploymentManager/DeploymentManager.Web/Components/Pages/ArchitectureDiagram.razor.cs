using Blazor.Diagrams;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Options;

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
        {
            if (node is not null && node.Title is not null && dependencies.ContainsKey(node.Title))
            {
                foreach (string? dependency in dependencies[node.Title])
                {
                    NodeModel? targetNode = Diagram.Nodes.First(n => n.Title == dependency);
                    if (targetNode is not null) Diagram.Links.Add(new LinkModel(node, targetNode));
                }
            }
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
}
