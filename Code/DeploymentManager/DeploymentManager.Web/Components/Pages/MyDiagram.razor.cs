using Blazor.Diagrams;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Options;

namespace DeploymentManager.Web.Pages;

public partial class MyDiagram
{
    private readonly string[] _excludedDirectories =
    {
        "cloudflare-worker", "bin", "obj", "Properties", ".idea", "ItemTemplates", ".template.config",
        "ProjectTemplates", ".vs", "wwwroot", "swagger", "v17", "DesignTimeBuild", "nupkg", "Cruip", "Cruip.old",
        "Supabase", "Generated", ".vscode"
    };

    public BlazorDiagram Diagram { get; set; } = null!;

    protected override async void OnInitialized()
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

        await RenderComponentStructure(@"C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint");
        ArrangeTree();
    }

    private async Task RenderComponentStructure(string rootPath)
    {
        if (!Directory.Exists(rootPath)) Console.WriteLine("Directory does not exist.");

        // NodeModel rootNode = CreateNode(Path.GetFileName(rootPath), new Point(0, 0), true);
        // Diagram.Nodes.Add(rootNode);

        // await TraverseComponents(rootPath, rootNode, 0);
    }

    private async Task TraverseComponents(string path, NodeModel parentNode, int level)
    {
        if (level > 3) return; // Limit depth to 3 levels to prevent overwhelming the diagram

        int offsetX = 250;
        int offsetY = 150;
        int childX = (int)parentNode.Position.X + offsetX;
        int childYStart = (int)parentNode.Position.Y;
        int childIndex = 0;

        // Process directories and subdirectories
        // foreach (string? dir in Directory.GetDirectories(path).Where(d => !ShouldExcludeDirectory(d)))
        // {
        //     string dirName = Path.GetFileName(dir);
        //     NodeModel dirNode = CreateNode(dirName, new Point(childX, childYStart + childIndex * offsetY), true);

        //     Diagram.Nodes.Add(dirNode);
        //     Diagram.Links.Add(new LinkModel(parentNode, dirNode));

        //     // Recursively process subdirectories
        //     await TraverseComponents(dir, dirNode, level + 1);

        //     childIndex++;
        // }
    }

    private bool ShouldExcludeDirectory(string directoryPath)
    {
        string? dirName = Path.GetFileName(directoryPath);
        return _excludedDirectories.Contains(dirName, StringComparer.OrdinalIgnoreCase);
    }

    // private NodeModel CreateNode(string title, Point position, bool isDirectory)
    // {
    //     return new NodeModel(position)
    //     {
    //         Title = title,
    //         Size = new Size(200, 50),            
    //     };
    // }

    private void ArrangeTree()
    {
        int ySpacing = 150;
        int xSpacing = 250;

        IOrderedEnumerable<IGrouping<double, NodeModel>>? levels = Diagram.Nodes.GroupBy(n => n.Position.Y / ySpacing)
            .OrderBy(g => g.Key);

        // foreach (IGrouping<double, NodeModel>? level in levels)
        // {
        //     int currentX = 50;
        //     foreach (NodeModel? node in level)
        //     {
        //         node.Position = new Point(currentX, node.Position.Y);
        //         currentX += xSpacing;
        //     }
        // }
    }
}

//using System;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using Blazor.Diagrams;
//using Blazor.Diagrams.Core.Geometry;
//using Blazor.Diagrams.Core.Models;
//using Blazor.Diagrams.Core.PathGenerators;
//using Blazor.Diagrams.Core.Routers;
//using Blazor.Diagrams.Options;

//namespace DeploymentManager.Web.Pages;

//public partial class MyDiagram
//{
//    public BlazorDiagram Diagram { get; set; } = null!;

//    // Folders to exclude
//    private readonly string[] ExcludedDirectories = { "cloudflare-worker", "bin", "obj", "Properties" };

//    protected override async void OnInitialized()
//    {
//        var options = new BlazorDiagramOptions
//        {
//            AllowMultiSelection = true,
//            Zoom =
//            {
//                Enabled = true,
//                ScaleFactor = 1.2,
//                Inverse = false
//            },
//            AllowPanning = true,
//            GridSize = 20, // Grid size for alignment
//            Links =
//            {
//                DefaultRouter = new NormalRouter(),
//                DefaultPathGenerator = new SmoothPathGenerator()
//            }
//        };

//        Diagram = new BlazorDiagram(options);

//        await GetLocalRepoFileStructure();
//    }

//    private async Task GetLocalRepoFileStructure()
//    {
//        string rootPath = @"C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint"; // Target directory

//        if (Directory.Exists(rootPath))
//        {
//            Console.WriteLine($"File structure of: {rootPath}");
//            await RenderFileStructure(rootPath, null, 0); // Start at root level (0)
//            ArrangeInHierarchy(); // Arrange nodes in a hierarchical layout
//        }
//        else
//        {
//            Console.WriteLine("The specified directory does not exist.");
//        }
//    }

//    private async Task RenderFileStructure(string path, NodeModel? parentNode, int currentLevel)
//    {
//        if (currentLevel > 5) return; // Stop rendering beyond a specified depth

//        try
//        {
//            // Create the current directory node
//            var currentDirectoryNode = parentNode ?? CreateNode(Path.GetFileName(path), new Point(50, 50), isDirectory: true);

//            if (parentNode is null)
//            {
//                Diagram.Nodes.Add(currentDirectoryNode); // Add the root directory node only once
//            }

//            // Track position for child nodes
//            int offsetX = 250; // Horizontal spacing
//            int offsetY = 100; // Vertical spacing
//            int childX = (int)currentDirectoryNode.Position.X + offsetX;
//            int childY = (int)currentDirectoryNode.Position.Y;

//            // Add .cs files in the current directory
//            foreach (var file in Directory.GetFiles(path, "*.cs"))
//            {
//                var fileName = Path.GetFileName(file);

//                var fileNode = CreateNode(fileName, new Point(childX, childY), isDirectory: false);

//                Diagram.Nodes.Add(fileNode);
//                Diagram.Links.Add(new LinkModel(currentDirectoryNode, fileNode));

//                childY += offsetY; // Move to the next file position
//            }

//            // Add subdirectory nodes recursively, excluding specified directories
//            foreach (var dir in Directory.GetDirectories(path).Where(d => !ShouldExcludeDirectory(d)))
//            {
//                var dirName = Path.GetFileName(dir);

//                var dirNode = CreateNode(dirName, new Point(childX, childY), isDirectory: true);

//                Diagram.Nodes.Add(dirNode);
//                Diagram.Links.Add(new LinkModel(currentDirectoryNode, dirNode));

//                // Recursively render subdirectories
//                await RenderFileStructure(dir, dirNode, currentLevel + 1);

//                childY += offsetY; // Move to the next directory position
//            }
//        }
//        catch (UnauthorizedAccessException)
//        {
//            Console.WriteLine($"Access denied: {path}");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error: {ex.Message}");
//        }
//    }

//    private bool ShouldExcludeDirectory(string directoryPath)
//    {
//        var dirName = Path.GetFileName(directoryPath);
//        return ExcludedDirectories.Contains(dirName, StringComparer.OrdinalIgnoreCase);
//    }

//    /// <summary>
//    /// Creates a node with styling based on whether it's a directory or file.
//    /// </summary>
//    private NodeModel CreateNode(string title, Point position, bool isDirectory)
//    {
//        int minWidth = 150; // Minimum width for nodes
//        int padding = 20;   // Extra space inside the node
//        int estimatedCharWidth = 8; // Approximate width of one character
//        int calculatedWidth = Math.Max(minWidth, title.Length * estimatedCharWidth + padding);

//        return new NodeModel(position)
//        {
//            Title = title,
//            Size = new Size(calculatedWidth, 50),
//            //Style = new NodeStyle
//            //{
//            //    BackgroundColor = isDirectory ? "lightgray" : "lightblue", // Directory: gray, File: blue
//            //    BorderColor = "black",
//            //    BorderThickness = 1
//            //}
//        };
//    }

//    /// <summary>
//    /// Arranges nodes in a hierarchy based on their Y positions.
//    /// </summary>
//    private void ArrangeInHierarchy()
//    {
//        int ySpacing = 150; // Vertical spacing between levels
//        int xSpacing = 300; // Horizontal spacing between sibling nodes

//        var levels = Diagram.Nodes.GroupBy(n => n.Position.Y / ySpacing).OrderBy(g => g.Key);

//        int currentY = 50;
//        foreach (var level in levels)
//        {
//            int currentX = 50;
//            foreach (var node in level)
//            {
//                node.Position = new Point(currentX, currentY);
//                currentX += xSpacing;
//            }
//            currentY += ySpacing;
//        }
//    }
//}

////using System;
////using System.IO;
////using System.Linq;
////using System.Threading.Tasks;
////using Blazor.Diagrams;
////using Blazor.Diagrams.Core.Geometry;
////using Blazor.Diagrams.Core.Models;
////using Blazor.Diagrams.Core.PathGenerators;
////using Blazor.Diagrams.Core.Routers;
////using Blazor.Diagrams.Options;

////namespace DeploymentManager.Web.Pages;

////public partial class MyDiagram
////{
////    public BlazorDiagram Diagram { get; set; } = null!;

////    // Folders to exclude
////    private readonly string[] ExcludedDirectories =
////        { "cloudflare-worker",
////              "bin",
////              "obj",
////              "Properties"
////        };

////    protected override async void OnInitialized()
////    {
////        var options = new BlazorDiagramOptions
////        {
////            AllowMultiSelection = true,
////            Zoom =
////                {
////                    Enabled = true,
////                    ScaleFactor = 1.2,
////                    Inverse = false
////                },
////            AllowPanning = true,
////            GridSize = 20, // Grid size for alignment
////            Links =
////                {
////                    DefaultRouter = new NormalRouter(),
////                    DefaultPathGenerator = new SmoothPathGenerator()
////                },
////            GridSnapToCenter = true
////            //Grid = new DiagramGrid // Add dotted grid widget
////            //{
////            //    Style = new DiagramGridStyle
////            //    {
////            //        BackgroundColor = "#2E2E2E", // Dark grey background
////            //        LineColor = "#555",         // Light grey dots
////            //        LineThickness = 1,
////            //        LineSpacing = 20            // Dots spacing
////            //    }
////            //}
////        };

////        Diagram = new BlazorDiagram(options);

////        await GetLocalRepoFileStructure();
////    }

////    private async Task GetLocalRepoFileStructure()
////    {
////        string rootPath = @"C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint"; // Target directory

////        if (Directory.Exists(rootPath))
////        {
////            Console.WriteLine($"File structure of: {rootPath}");
////            await RenderSourceCodeStructure(rootPath, null, 1); // Start at level 1
////            ArrangeInHierarchy(); // Arrange the nodes in a hierarchical layout
////        }
////        else
////        {
////            Console.WriteLine("The specified directory does not exist.");
////        }
////    }

////    private async Task RenderSourceCodeStructure(string path, NodeModel? parentNode, int currentLevel)
////    {
////        if (currentLevel > 3) return; // Stop rendering beyond x number of levels

////        try
////        {
////            // Create the current directory node
////            var currentDirectoryNode = parentNode ?? CreateSizedNode(Path.GetFileName(path), new Point(50, 50), isDirectory: true);

////            if (parentNode is null)
////            {
////                Diagram.Nodes.Add(currentDirectoryNode); // Add the root directory node only once
////            }

////            // Track position for child nodes
////            int offsetX = 200; // Horizontal spacing
////            int offsetY = 150; // Vertical spacing
////            int childX = (int)currentDirectoryNode.Position.X + offsetX;
////            int childY = (int)currentDirectoryNode.Position.Y;

////            // Add .cs files in the current directory
////            foreach (var file in Directory.GetFiles(path, "*.cs"))
////            {
////                var fileName = Path.GetFileName(file);
////                Console.WriteLine($"File: {fileName}");

////                var fileNode = CreateSizedNode(fileName, new Point(childX, childY), isDirectory: false);

////                Diagram.Nodes.Add(fileNode);
////                Diagram.Links.Add(new LinkModel(currentDirectoryNode, fileNode));

////                // Move the next file node down
////                childY += offsetY;
////            }

////            // Add subdirectory nodes recursively, excluding specified directories
////            foreach (var dir in Directory.GetDirectories(path).Where(d => !ShouldExcludeDirectory(d)))
////            {
////                var dirName = Path.GetFileName(dir);
////                Console.WriteLine($"Dir: {dirName}");

////                var dirNode = CreateSizedNode(dirName, new Point(childX, childY), isDirectory: true);

////                Diagram.Nodes.Add(dirNode);
////                Diagram.Links.Add(new LinkModel(currentDirectoryNode, dirNode));

////                //var group = new GroupModel(new[] { dirNode });
////                //Diagram.Groups.Add(group);

////                // Recursively render subdirectories (up to 2 levels)
////                await RenderSourceCodeStructure(dir, dirNode, currentLevel + 1);

////                // Add sub-groups inside the current group
////                //foreach (var subDir in Directory.GetDirectories(dir).Where(d => !ShouldExcludeDirectory(d)))
////                //{
////                //    var subDirName = Path.GetFileName(subDir);
////                //    var subDirNode = CreateSizedNode(subDirName, new Point(childX, childY), isDirectory: true);

////                //    Diagram.Nodes.Add(subDirNode);
////                //    Diagram.Links.Add(new LinkModel(dirNode, subDirNode));

////                //    var subGroup = new GroupModel(new[] { subDirNode });
////                //    group.AddChild(subGroup);
////                //}
////                // Move the next directory node down
////                childY += offsetY;
////            }
////        }
////        catch (UnauthorizedAccessException)
////        {
////            Console.WriteLine($"Access denied: {path}");
////        }
////        catch (Exception ex)
////        {
////            Console.WriteLine($"Error: {ex.Message}");
////        }
////    }

////    private bool ShouldExcludeDirectory(string directoryPath)
////    {
////        var dirName = Path.GetFileName(directoryPath);
////        return ExcludedDirectories.Contains(dirName, StringComparer.OrdinalIgnoreCase);
////    }

////    /// <summary>
////    /// Creates a node with dynamic sizing based on the text length.
////    /// </summary>
////    private NodeModel CreateSizedNode(string title, Point position, bool isDirectory)
////    {
////        int minWidth = 100; // Minimum width to ensure smaller texts fit
////        int padding = 20;   // Extra space for padding inside the node
////        int estimatedCharWidth = 8; // Approximate width of one character in pixels
////        int maxLineLength = 20; // Maximum number of characters before wrapping text
////        int calculatedWidth = Math.Max(minWidth, Math.Min(title.Length, maxLineLength) * estimatedCharWidth + padding);

////        // Adjust height dynamically if the title requires wrapping
////        int numberOfLines = (int)Math.Ceiling((double)title.Length / maxLineLength);
////        int heightPerLine = 20; // Height for one line of text
////        int calculatedHeight = Math.Max(40, numberOfLines * heightPerLine);

////        return new NodeModel(position)
////        {
////            Title = title,
////            Size = new Size(calculatedWidth, calculatedHeight),
////            //Style = new NodeStyle
////            //{
////            //    BackgroundColor = isDirectory ? "lightgray" : "lightblue", // Differentiate file and directory nodes
////            //    BorderColor = "black",
////            //    BorderThickness = 2
////            //}
////        };
////    }

////    /// <summary>
////    /// Arranges the nodes in a hierarchical layout.
////    /// </summary>
////    private void ArrangeInHierarchy()
////    {
////        int ySpacing = 200; // Increased vertical spacing between levels
////        int xSpacing = 300; // Increased horizontal spacing between sibling nodes
////        int groupSpacing = 150; // Reduced spacing between groups

////        var levels = Diagram.Nodes.GroupBy(n => n.Position.Y / ySpacing).OrderBy(g => g.Key);

////        int currentY = 50;
////        foreach (var level in levels)
////        {
////            int currentX = 50;
////            foreach (var node in level)
////            {
////                node.Position = new Point(currentX, currentY);
////                currentX += xSpacing;
////            }
////            currentY += ySpacing;
////        }

////        // Arrange groups with reduced spacing and avoid overlapping
////        //int groupY = currentY + groupSpacing;
////        //foreach (var group in Diagram.Groups)
////        //{
////        //    int maxGroupHeight = 0;
////        //    foreach (var node in group.Children)
////        //    {
////        //        node.Position = new Point(node.Position.X, groupY);
////        //        maxGroupHeight = Math.Max(maxGroupHeight, (int)node.Size.Height);
////        //    }
////        //    groupY += maxGroupHeight + groupSpacing;
////        //}
////    }
////}
