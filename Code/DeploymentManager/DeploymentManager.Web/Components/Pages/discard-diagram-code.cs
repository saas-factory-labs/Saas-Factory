//namespace DeploymentManager.Web.Pages
//{
//    public partial class MyDiagram
//    {
//        public BlazorDiagram Diagram { get; set; } = null!;

//        private readonly string[] ExcludedDirectories = { "cloudflare-worker" }; // Folders to exclude

//        private async Task GetLocalRepoFileStructure()
//        {
//            string rootPath = @"C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint"; // Target directory

//            if (Directory.Exists(rootPath))
//            {
//                Console.WriteLine($"File structure of: {rootPath}");
//                await RenderSourceCodeStructure(rootPath, null, 1); // Start at level 1
//            }
//            else
//            {
//                Console.WriteLine("The specified directory does not exist.");
//            }
//        }

//        private async Task RenderSourceCodeStructure(string path, NodeModel? parentNode, int currentLevel)
//        {
//            if (currentLevel > 2) return; // Stop rendering beyond 2 levels

//            try
//            {
//                // Create the current directory node
//                var currentDirectoryNode = parentNode ?? CreateSizedNode(Path.GetFileName(path), new Point(50, 50));

//                if (parentNode is null)
//                {
//                    Diagram.Nodes.Add(currentDirectoryNode); // Add the root directory node only once
//                }

//                // Track position for child nodes
//                int offsetX = 200; // Horizontal spacing
//                int offsetY = 150; // Vertical spacing
//                int childX = (int)currentDirectoryNode.Position.X + offsetX;
//                int childY = (int)currentDirectoryNode.Position.Y;

//                // Add .cs files in the current directory
//                foreach (var file in Directory.GetFiles(path, "*.cs"))
//                {
//                    var fileName = Path.GetFileName(file);
//                    Console.WriteLine($"File: {fileName}");

//                    var fileNode = CreateSizedNode(fileName, new Point(childX, childY));

//                    Diagram.Nodes.Add(fileNode);
//                    Diagram.Links.Add(new LinkModel(currentDirectoryNode, fileNode));

//                    // Move the next file node down
//                    childY += offsetY;
//                }

//                // Add subdirectory nodes recursively, excluding specified directories
//                foreach (var dir in Directory.GetDirectories(path).Where(d => !ShouldExcludeDirectory(d)))
//                {
//                    var dirName = Path.GetFileName(dir);
//                    Console.WriteLine($"Dir: {dirName}");

//                    var dirNode = CreateSizedNode(dirName, new Point(childX, childY));

//                    Diagram.Nodes.Add(dirNode);
//                    Diagram.Links.Add(new LinkModel(currentDirectoryNode, dirNode));

//                    // Recursively render subdirectories (up to 2 levels)
//                    await RenderSourceCodeStructure(dir, dirNode, currentLevel + 1);

//                    // Move the next directory node down
//                    childY += offsetY;
//                }
//            }
//            catch (UnauthorizedAccessException)
//            {
//                Console.WriteLine($"Access denied: {path}");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error: {ex.Message}");
//            }
//        }

//        private bool ShouldExcludeDirectory(string directoryPath)
//        {
//            var dirName = Path.GetFileName(directoryPath);
//            return ExcludedDirectories.Contains(dirName, StringComparer.OrdinalIgnoreCase);
//        }

//        /// <summary>
//        /// Creates a node with dynamic sizing based on the text length.
//        /// </summary>
//        private NodeModel CreateSizedNode(string title, Point position)
//        {
//            int minWidth = 100; // Minimum width to ensure smaller texts fit
//            int padding = 20;   // Extra space for padding inside the node
//            int estimatedCharWidth = 8; // Approximate width of one character in pixels
//            int maxLineLength = 20; // Maximum number of characters before wrapping text
//            int calculatedWidth = Math.Max(minWidth, Math.Min(title.Length, maxLineLength) * estimatedCharWidth + padding);

//            // Adjust height dynamically if the title requires wrapping
//            int numberOfLines = (int)Math.Ceiling((double)title.Length / maxLineLength);
//            int heightPerLine = 20; // Height for one line of text
//            int calculatedHeight = Math.Max(40, numberOfLines * heightPerLine);

//            return new NodeModel(position)
//            {
//                Title = title,
//                Size = new Size(calculatedWidth, calculatedHeight)
//            };
//        }

//        protected override void OnInitialized()
//        {
//            var options = new BlazorDiagramOptions
//            {
//                AllowMultiSelection = true,
//                Zoom =
//                {
//                    Enabled = true,
//                    ScaleFactor = 1.2,
//                    Inverse = false
//                },
//                AllowPanning = true,
//                GridSize = 20,
//                Links =
//                {
//                    DefaultRouter = new NormalRouter(),
//                    DefaultPathGenerator = new SmoothPathGenerator()
//                },
//            };

//            Diagram = new BlazorDiagram(options);

//            // Call the method to load the file structure
//            _ = GetLocalRepoFileStructure();
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

////namespace DeploymentManager.Web.Pages
////{
////    public partial class MyDiagram
////    {
////        public BlazorDiagram Diagram { get; set; } = null!;

////        private readonly string[] ExcludedDirectories = { "cloudflare-worker" }; // Folders to exclude

////        private async Task GetLocalRepoFileStructure()
////        {
////            string rootPath = @"C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint"; // Target directory

////            if (Directory.Exists(rootPath))
////            {
////                Console.WriteLine($"File structure of: {rootPath}");
////                await RenderSourceCodeStructure(rootPath, null, 1); // Start at level 1
////            }
////            else
////            {
////                Console.WriteLine("The specified directory does not exist.");
////            }
////        }

////        private async Task RenderSourceCodeStructure(string path, NodeModel? parentNode, int currentLevel)
////        {
////            if (currentLevel > 2) return; // Stop rendering beyond 2 levels

////            try
////            {
////                // Create the current directory node
////                var currentDirectoryNode = parentNode ?? CreateSizedNode(Path.GetFileName(path), new Point(50, 50));

////                if (parentNode is null)
////                {
////                    Diagram.Nodes.Add(currentDirectoryNode); // Add the root directory node only once
////                }

////                // Track position for child nodes
////                int offsetX = 200; // Horizontal spacing
////                int offsetY = 150; // Vertical spacing
////                int childX = (int)currentDirectoryNode.Position.X + offsetX;
////                int childY = (int)currentDirectoryNode.Position.Y;

////                // Add .cs files in the current directory
////                foreach (var file in Directory.GetFiles(path, "*.cs"))
////                {
////                    var fileName = Path.GetFileName(file);
////                    Console.WriteLine($"File: {fileName}");

////                    var fileNode = CreateSizedNode(fileName, new Point(childX, childY));

////                    Diagram.Nodes.Add(fileNode);
////                    Diagram.Links.Add(new LinkModel(currentDirectoryNode, fileNode));

////                    // Move the next file node down
////                    childY += offsetY;
////                }

////                // Add subdirectory nodes recursively, excluding specified directories
////                foreach (var dir in Directory.GetDirectories(path).Where(d => !ShouldExcludeDirectory(d)))
////                {
////                    var dirName = Path.GetFileName(dir);
////                    Console.WriteLine($"Dir: {dirName}");

////                    var dirNode = CreateSizedNode(dirName, new Point(childX, childY));

////                    Diagram.Nodes.Add(dirNode);
////                    Diagram.Links.Add(new LinkModel(currentDirectoryNode, dirNode));

////                    // Recursively render subdirectories (up to 2 levels)
////                    await RenderSourceCodeStructure(dir, dirNode, currentLevel + 1);

////                    // Move the next directory node down
////                    childY += offsetY;
////                }
////            }
////            catch (UnauthorizedAccessException)
////            {
////                Console.WriteLine($"Access denied: {path}");
////            }
////            catch (Exception ex)
////            {
////                Console.WriteLine($"Error: {ex.Message}");
////            }
////        }

////        private bool ShouldExcludeDirectory(string directoryPath)
////        {
////            var dirName = Path.GetFileName(directoryPath);
////            return ExcludedDirectories.Contains(dirName, StringComparer.OrdinalIgnoreCase);
////        }

////        /// <summary>
////        /// Creates a node with dynamic sizing based on the text length.
////        /// </summary>
////        private NodeModel CreateSizedNode(string title, Point position)
////        {
////            int minWidth = 80; // Minimum node width
////            int padding = 20;  // Extra space for text inside the node
////            int estimatedCharWidth = 8; // Approximate width per character
////            int calculatedWidth = Math.Max(minWidth, title.Length * estimatedCharWidth + padding);
////            int height = 40; // Fixed height

////            return new NodeModel(position)
////            {
////                Title = title,
////                Size = new Size(calculatedWidth, height)
////            };
////        }

////        protected override void OnInitialized()
////        {
////            var options = new BlazorDiagramOptions
////            {
////                AllowMultiSelection = true,
////                Zoom =
////                {
////                    Enabled = true,
////                    ScaleFactor = 1.2,
////                    Inverse = false
////                },
////                AllowPanning = true,
////                GridSize = 20,
////                Links =
////                {
////                    DefaultRouter = new NormalRouter(),
////                    DefaultPathGenerator = new SmoothPathGenerator()
////                },
////            };

////            Diagram = new BlazorDiagram(options);

////            // Call the method to load the file structure
////            _ = GetLocalRepoFileStructure();
////        }
////    }
////}


//////using System;
//////using System.IO;
//////using System.Linq;
//////using System.Threading.Tasks;
//////using Blazor.Diagrams;
//////using Blazor.Diagrams.Core.Geometry;
//////using Blazor.Diagrams.Core.Models;
//////using Blazor.Diagrams.Core.PathGenerators;
//////using Blazor.Diagrams.Core.Routers;
//////using Blazor.Diagrams.Options;

//////namespace DeploymentManager.Web.Pages
//////{
//////    public partial class MyDiagram
//////    {
//////        public BlazorDiagram Diagram { get; set; } = null!;

//////        private readonly string[] ExcludedDirectories = { "cloudflare-worker" }; // Folders to exclude

//////        private async Task GetLocalRepoFileStructure()
//////        {
//////            string rootPath = @"C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint"; // Target directory

//////            if (Directory.Exists(rootPath))
//////            {
//////                Console.WriteLine($"File structure of: {rootPath}");
//////                await RenderSourceCodeStructure(rootPath, null, 1); // Start at level 1
//////            }
//////            else
//////            {
//////                Console.WriteLine("The specified directory does not exist.");
//////            }
//////        }

//////        private async Task RenderSourceCodeStructure(string path, NodeModel? parentNode, int currentLevel)
//////        {
//////            if (currentLevel > 2) return; // Stop rendering beyond 2 levels

//////            try
//////            {
//////                // Create the current directory node
//////                var currentDirectoryNode = parentNode ?? new NodeModel(position: new Point(50, 50))
//////                {
//////                    Title = Path.GetFileName(path),
//////                };

//////                if (parentNode is null)
//////                {
//////                    Diagram.Nodes.Add(currentDirectoryNode); // Add the root directory node only once
//////                }

//////                // Track position for child nodes
//////                int offsetX = 200; // Horizontal spacing
//////                int offsetY = 150; // Vertical spacing
//////                int childX = (int)currentDirectoryNode.Position.X + offsetX;
//////                int childY = (int)currentDirectoryNode.Position.Y;

//////                // Add .cs files in the current directory
//////                foreach (var file in Directory.GetFiles(path, "*.cs"))
//////                {
//////                    var fileName = Path.GetFileName(file);
//////                    Console.WriteLine($"File: {fileName}");

//////                    var fileNode = new NodeModel(position: new Point(childX, childY))
//////                    {
//////                        Title = fileName,
//////                    };

//////                    Diagram.Nodes.Add(fileNode);
//////                    Diagram.Links.Add(new LinkModel(currentDirectoryNode, fileNode));

//////                    // Move the next file node down
//////                    childY += offsetY;
//////                }

//////                // Add subdirectory nodes recursively, excluding specified directories
//////                foreach (var dir in Directory.GetDirectories(path).Where(d => !ShouldExcludeDirectory(d)))
//////                {
//////                    var dirName = Path.GetFileName(dir);
//////                    Console.WriteLine($"Dir: {dirName}");

//////                    var dirNode = new NodeModel(position: new Point(childX, childY))
//////                    {
//////                        Title = dirName,
//////                    };

//////                    Diagram.Nodes.Add(dirNode);
//////                    Diagram.Links.Add(new LinkModel(currentDirectoryNode, dirNode));

//////                    // Recursively render subdirectories (up to 2 levels)
//////                    await RenderSourceCodeStructure(dir, dirNode, currentLevel + 1);

//////                    // Move the next directory node down
//////                    childY += offsetY;
//////                }
//////            }
//////            catch (UnauthorizedAccessException)
//////            {
//////                Console.WriteLine($"Access denied: {path}");
//////            }
//////            catch (Exception ex)
//////            {
//////                Console.WriteLine($"Error: {ex.Message}");
//////            }
//////        }

//////        private bool ShouldExcludeDirectory(string directoryPath)
//////        {
//////            var dirName = Path.GetFileName(directoryPath);
//////            return ExcludedDirectories.Contains(dirName, StringComparer.OrdinalIgnoreCase);
//////        }

//////        protected override void OnInitialized()
//////        {
//////            var options = new BlazorDiagramOptions
//////            {
//////                AllowMultiSelection = true,
//////                Zoom =
//////                {
//////                    Enabled = true,
//////                    ScaleFactor = 1.2,
//////                    Inverse = false
//////                },
//////                AllowPanning = true,
//////                GridSize = 20,
//////                Links =
//////                {
//////                    DefaultRouter = new NormalRouter(),
//////                    DefaultPathGenerator = new SmoothPathGenerator()
//////                },
//////            };

//////            Diagram = new BlazorDiagram(options);

//////            // Call the method to load the file structure
//////            _ = GetLocalRepoFileStructure();
//////        }
//////    }
//////}


////////using System;
////////using System.IO;
////////using System.Linq;
////////using System.Threading.Tasks;
////////using Blazor.Diagrams;
////////using Blazor.Diagrams.Core.Geometry;
////////using Blazor.Diagrams.Core.Models;
////////using Blazor.Diagrams.Core.PathGenerators;
////////using Blazor.Diagrams.Core.Routers;
////////using Blazor.Diagrams.Options;

////////namespace DeploymentManager.Web.Pages;

////////public partial class MyDiagram
////////{
////////    public BlazorDiagram Diagram { get; set; } = null!;

////////    private readonly string[] ExcludedDirectories = { "cloudflare-worker" }; // Folders to exclude

////////    private async Task GetLocalRepoFileStructure()
////////    {
////////        string rootPath = @"C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint"; // Target directory

////////        if (Directory.Exists(rootPath))
////////        {
////////            Console.WriteLine($"File structure of: {rootPath}");
////////            await RenderSourceCodeStructure(rootPath, null, 1); // Start at level 1
////////        }
////////        else
////////        {
////////            Console.WriteLine("The specified directory does not exist.");
////////        }
////////    }

////////    private async Task RenderSourceCodeStructure(string path, NodeModel? parentNode, int currentLevel)
////////    {
////////        if (currentLevel > 2) return; // Stop rendering beyond 2 levels

////////        try
////////        {
////////            // Create the current directory node
////////            var currentDirectoryNode = parentNode ?? new NodeModel(position: new Point(50, 50))
////////            {
////////                Title = Path.GetFileName(path),
////////            };

////////            if (parentNode is null)
////////            {
////////                Diagram.Nodes.Add(currentDirectoryNode); // Add the root directory node only once
////////            }

////////            // Track position for child nodes
////////            int offsetX = 200; // Horizontal spacing
////////            int offsetY = 150; // Vertical spacing
////////            int childX = (int)currentDirectoryNode.Position.X + offsetX;
////////            int childY = (int)currentDirectoryNode.Position.Y;

////////            // Add .cs files in the current directory
////////            foreach (var file in Directory.GetFiles(path, "*.cs"))
////////            {
////////                var fileName = Path.GetFileName(file);
////////                Console.WriteLine($"File: {fileName}");

////////                var fileNode = new NodeModel(position: new Point(childX, childY))
////////                {
////////                    Title = fileName,
////////                };

////////                Diagram.Nodes.Add(fileNode);
////////                Diagram.Links.Add(new LinkModel(currentDirectoryNode, fileNode));

////////                // Move the next file node down
////////                childY += offsetY;
////////            }

////////            // Add subdirectory nodes recursively, excluding specified directories
////////            foreach (var dir in Directory.GetDirectories(path).Where(d => !ShouldExcludeDirectory(d)))
////////            {
////////                var dirName = Path.GetFileName(dir);
////////                Console.WriteLine($"Dir: {dirName}");

////////                var dirNode = new NodeModel(position: new Point(childX, childY))
////////                {
////////                    Title = dirName,
////////                };

////////                Diagram.Nodes.Add(dirNode);
////////                Diagram.Links.Add(new LinkModel(currentDirectoryNode, dirNode));

////////                // Recursively render subdirectories (up to 2 levels)
////////                await RenderSourceCodeStructure(dir, dirNode, currentLevel + 1);

////////                // Move the next directory node down
////////                childY += offsetY;
////////            }
////////        }
////////        catch (UnauthorizedAccessException)
////////        {
////////            Console.WriteLine($"Access denied: {path}");
////////        }
////////        catch (Exception ex)
////////        {
////////            Console.WriteLine($"Error: {ex.Message}");
////////        }
////////    }

////////    private bool ShouldExcludeDirectory(string directoryPath)
////////    {
////////        var dirName = Path.GetFileName(directoryPath);
////////        return ExcludedDirectories.Contains(dirName, StringComparer.OrdinalIgnoreCase);
////////    }

////////    protected override void OnInitialized()
////////    {
////////        var options = new BlazorDiagramOptions
////////        {
////////            AllowMultiSelection = true,
////////            Zoom =
////////            {
////////                Enabled = true,
////////                ScaleFactor = 1.2,
////////                Inverse = false
////////            },
////////            AllowPanning = true,
////////            GridSize = 20,
////////            Links =
////////            {
////////                DefaultRouter = new NormalRouter(),
////////                DefaultPathGenerator = new SmoothPathGenerator()
////////            },
////////        };

////////        Diagram = new BlazorDiagram(options);

////////        // Call the method to load the file structure
////////        _ = GetLocalRepoFileStructure();
////////    }
////////}


//////////using System;
//////////using System.IO;
//////////using System.Linq;
//////////using System.Threading.Tasks;
//////////using Blazor.Diagrams;
//////////using Blazor.Diagrams.Core.Geometry;
//////////using Blazor.Diagrams.Core.Models;
//////////using Blazor.Diagrams.Core.PathGenerators;
//////////using Blazor.Diagrams.Core.Routers;
//////////using Blazor.Diagrams.Options;

//////////namespace DeploymentManager.Web.Pages
//////////{
//////////    public partial class MyDiagram
//////////    {
//////////        public BlazorDiagram Diagram { get; set; } = null!;

//////////        private readonly string[] ExcludedDirectories = { "cloudflare-worker" }; // Folders to exclude

//////////        private async Task GetLocalRepoFileStructure()
//////////        {
//////////            string rootPath = @"C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint"; // Target directory

//////////            if (Directory.Exists(rootPath))
//////////            {
//////////                Console.WriteLine($"File structure of: {rootPath}");
//////////                await RenderSourceCodeStructure(rootPath); // Render only 1 level
//////////            }
//////////            else
//////////            {
//////////                Console.WriteLine("The specified directory does not exist.");
//////////            }
//////////        }

//////////        private async Task RenderSourceCodeStructure(string path)
//////////        {
//////////            try
//////////            {
//////////                // Create the root directory node
//////////                var rootNode = new NodeModel(position: new Point(50, 50))
//////////                {
//////////                    Title = Path.GetFileName(path),
//////////                };
//////////                Diagram.Nodes.Add(rootNode);

//////////                // Track position for child nodes
//////////                int offsetX = 200; // Horizontal spacing
//////////                int offsetY = 150; // Vertical spacing
//////////                int childX = (int)rootNode.Position.X + offsetX;
//////////                int childY = (int)rootNode.Position.Y;

//////////                // Add .cs files in the current directory
//////////                foreach (var file in Directory.GetFiles(path, "*.cs"))
//////////                {
//////////                    var fileName = Path.GetFileName(file);
//////////                    Console.WriteLine($"File: {fileName}");

//////////                    var fileNode = new NodeModel(position: new Point(childX, childY))
//////////                    {
//////////                        Title = fileName,
//////////                    };

//////////                    Diagram.Nodes.Add(fileNode);
//////////                    Diagram.Links.Add(new LinkModel(rootNode, fileNode));

//////////                    // Move the next file node down
//////////                    childY += offsetY;
//////////                }

//////////                // Add immediate subdirectories, excluding specified ones
//////////                foreach (var dir in Directory.GetDirectories(path).Where(d => !ShouldExcludeDirectory(d)))
//////////                {
//////////                    var dirName = Path.GetFileName(dir);
//////////                    Console.WriteLine($"Excluded Dir: {dirName} (not included in 1-level view)");
//////////                }
//////////            }
//////////            catch (UnauthorizedAccessException)
//////////            {
//////////                Console.WriteLine($"Access denied: {path}");
//////////            }
//////////            catch (Exception ex)
//////////            {
//////////                Console.WriteLine($"Error: {ex.Message}");
//////////            }
//////////        }

//////////        private bool ShouldExcludeDirectory(string directoryPath)
//////////        {
//////////            var dirName = Path.GetFileName(directoryPath);
//////////            return ExcludedDirectories.Contains(dirName, StringComparer.OrdinalIgnoreCase);
//////////        }

//////////        protected override void OnInitialized()
//////////        {
//////////            var options = new BlazorDiagramOptions
//////////            {
//////////                AllowMultiSelection = true,
//////////                Zoom =
//////////                {
//////////                    Enabled = true,
//////////                    ScaleFactor = 1.2,
//////////                    Inverse = false
//////////                },
//////////                AllowPanning = true,
//////////                GridSize = 20,
//////////                Links =
//////////                {
//////////                    DefaultRouter = new NormalRouter(),
//////////                    DefaultPathGenerator = new SmoothPathGenerator()
//////////                },
//////////            };

//////////            Diagram = new BlazorDiagram(options);

//////////            // Call the method to load the file structure
//////////            _ = GetLocalRepoFileStructure();
//////////        }
//////////    }
//////////}


////////////using System;
////////////using System.IO;
////////////using System.Linq;
////////////using System.Threading.Tasks;
////////////using Blazor.Diagrams;
////////////using Blazor.Diagrams.Core.Geometry;
////////////using Blazor.Diagrams.Core.Models;
////////////using Blazor.Diagrams.Core.PathGenerators;
////////////using Blazor.Diagrams.Core.Routers;
////////////using Blazor.Diagrams.Options;

////////////namespace DeploymentManager.Web.Pages
////////////{
////////////    public partial class MyDiagram
////////////    {
////////////        public BlazorDiagram Diagram { get; set; } = null!;

////////////        private readonly string[] ExcludedDirectories = { "bin", "obj", "Debug", "Release" }; // Folders to exclude

////////////        private async Task GetLocalRepoFileStructure()
////////////        {
////////////            string rootPath = @"C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint"; // Target directory

////////////            if (Directory.Exists(rootPath))
////////////            {
////////////                Console.WriteLine($"File structure of: {rootPath}");
////////////                await RenderSourceCodeStructure(rootPath, null, 1); // Start at level 1
////////////            }
////////////            else
////////////            {
////////////                Console.WriteLine("The specified directory does not exist.");
////////////            }
////////////        }

////////////        private async Task RenderSourceCodeStructure(string path, NodeModel? parentNode, int currentLevel)
////////////        {
////////////            if (currentLevel > 3) return; // Stop rendering beyond 3 levels

////////////            try
////////////            {
////////////                // Create a node for the current directory if it doesn't already have a parent
////////////                var currentDirectoryNode = parentNode ?? new NodeModel(position: new Point(50, 50))
////////////                {
////////////                    Title = Path.GetFileName(path),
////////////                    //Style = new NodeStyle
////////////                    //{
////////////                    //    BackgroundColor = "lightgray", // Directory nodes have a light gray background
////////////                    //    BorderColor = "black"
////////////                    //}
////////////                };

////////////                if (parentNode is null)
////////////                {
////////////                    Diagram.Nodes.Add(currentDirectoryNode);
////////////                }

////////////                // Track position for child nodes
////////////                int offsetX = 200; // Horizontal spacing
////////////                int offsetY = 150; // Vertical spacing
////////////                int childX = (int)(currentDirectoryNode.Position.X + offsetX);
////////////                int childY = (int)currentDirectoryNode.Position.Y;

////////////                // Add .cs file nodes in the current directory
////////////                foreach (var file in Directory.GetFiles(path, "*.cs"))
////////////                {
////////////                    var fileName = Path.GetFileName(file);
////////////                    Console.WriteLine($"File: {fileName}");

////////////                    var fileNode = new NodeModel(position: new Point(childX, childY))
////////////                    {
////////////                        Title = fileName,
////////////                        //Style = new NodeStyle
////////////                        //{
////////////                        //    BackgroundColor = "lightblue", // File nodes have a light blue background
////////////                        //    BorderColor = "black"
////////////                        //}
////////////                    };

////////////                    Diagram.Nodes.Add(fileNode);
////////////                    Diagram.Links.Add(new LinkModel(currentDirectoryNode, fileNode));

////////////                    // Move the next file node down
////////////                    childY += offsetY;
////////////                }

////////////                // Add subdirectory nodes recursively, excluding specific directories
////////////                foreach (var dir in Directory.GetDirectories(path).Where(d => !ShouldExcludeDirectory(d)))
////////////                {
////////////                    var dirName = Path.GetFileName(dir);
////////////                    Console.WriteLine($"Dir: {dirName}");

////////////                    var dirNode = new NodeModel(position: new Point(childX, childY))
////////////                    {
////////////                        Title = dirName,
////////////                        //Style = new NodeStyle
////////////                        //{
////////////                        //    BackgroundColor = "lightgray",
////////////                        //    BorderColor = "black"
////////////                        //}
////////////                    };

////////////                    Diagram.Nodes.Add(dirNode);
////////////                    Diagram.Links.Add(new LinkModel(currentDirectoryNode, dirNode));

////////////                    // Recursively render subdirectories
////////////                    await RenderSourceCodeStructure(dir, dirNode, currentLevel + 1);

////////////                    // Move the next directory node down
////////////                    childY += offsetY;
////////////                }
////////////            }
////////////            catch (UnauthorizedAccessException)
////////////            {
////////////                Console.WriteLine($"Access denied: {path}");
////////////            }
////////////            catch (Exception ex)
////////////            {
////////////                Console.WriteLine($"Error: {ex.Message}");
////////////            }
////////////        }

////////////        private bool ShouldExcludeDirectory(string directoryPath)
////////////        {
////////////            var dirName = Path.GetFileName(directoryPath);
////////////            return ExcludedDirectories.Contains(dirName, StringComparer.OrdinalIgnoreCase);
////////////        }

////////////        protected override void OnInitialized()
////////////        {
////////////            var options = new BlazorDiagramOptions
////////////            {
////////////                AllowMultiSelection = true,
////////////                Zoom =
////////////                {
////////////                    Enabled = true,
////////////                    ScaleFactor = 1.2,
////////////                    Inverse = false
////////////                },
////////////                AllowPanning = true,
////////////                GridSize = 20,
////////////                Links =
////////////                {
////////////                    DefaultRouter = new NormalRouter(),
////////////                    DefaultPathGenerator = new SmoothPathGenerator()
////////////                },
////////////            };

////////////            Diagram = new BlazorDiagram(options);

////////////            // Call the method to load the file structure
////////////            _ = GetLocalRepoFileStructure();
////////////        }
////////////    }
////////////}


//////////////using System;
//////////////using System.IO;
//////////////using System.Threading.Tasks;
//////////////using Blazor.Diagrams;
//////////////using Blazor.Diagrams.Core.Geometry;
//////////////using Blazor.Diagrams.Core.Models;
//////////////using Blazor.Diagrams.Core.PathGenerators;
//////////////using Blazor.Diagrams.Core.Routers;
//////////////using Blazor.Diagrams.Options;

//////////////namespace DeploymentManager.Web.Pages
//////////////{
//////////////    public partial class MyDiagram
//////////////    {
//////////////        public BlazorDiagram Diagram { get; set; } = null!;

//////////////        private async Task GetLocalRepoFileStructure()
//////////////        {
//////////////            string rootPath = @"C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint"; // Target directory

//////////////            if (Directory.Exists(rootPath))
//////////////            {
//////////////                Console.WriteLine($"File structure of: {rootPath}");
//////////////                await RenderSourceCodeStructure(rootPath, null, 1); // Start at level 1
//////////////            }
//////////////            else
//////////////            {
//////////////                Console.WriteLine("The specified directory does not exist.");
//////////////            }
//////////////        }

//////////////        private async Task RenderSourceCodeStructure(string path, NodeModel? parentNode, int currentLevel)
//////////////        {
//////////////            if (currentLevel > 3) return; // Stop rendering beyond 3 levels

//////////////            try
//////////////            {
//////////////                // Create a node for the current directory if it doesn't already have a parent
//////////////                var currentDirectoryNode = parentNode ?? new NodeModel(position: new Point(50, 50))
//////////////                {
//////////////                    Title = Path.GetFileName(path),
//////////////                    //Style = new NodeStyle
//////////////                    //{
//////////////                    //    BackgroundColor = "lightgray", // Directory nodes have a light gray background
//////////////                    //    BorderColor = "black"
//////////////                    //}
//////////////                };

//////////////                if (parentNode is null)
//////////////                {
//////////////                    Diagram.Nodes.Add(currentDirectoryNode);
//////////////                }

//////////////                // Track position for child nodes
//////////////                int offsetX = 200; // Horizontal spacing
//////////////                int offsetY = 150; // Vertical spacing
//////////////                int childX = (int)(currentDirectoryNode.Position.X + offsetX);
//////////////                int childY = (int)currentDirectoryNode.Position.Y;

//////////////                // Add .cs file nodes in the current directory
//////////////                foreach (var file in Directory.GetFiles(path, "*.cs"))
//////////////                {
//////////////                    var fileName = Path.GetFileName(file);
//////////////                    Console.WriteLine($"File: {fileName}");

//////////////                    var fileNode = new NodeModel(position: new Point(childX, childY))
//////////////                    {
//////////////                        Title = fileName,
//////////////                        //Style = new NodeStyle
//////////////                        //{
//////////////                        //    BackgroundColor = "lightblue", // File nodes have a light blue background
//////////////                        //    BorderColor = "black"
//////////////                        //}
//////////////                    };

//////////////                    Diagram.Nodes.Add(fileNode);
//////////////                    Diagram.Links.Add(new LinkModel(currentDirectoryNode, fileNode));

//////////////                    // Move the next file node down
//////////////                    childY += offsetY;
//////////////                }

//////////////                // Add subdirectory nodes recursively
//////////////                foreach (var dir in Directory.GetDirectories(path))
//////////////                {
//////////////                    var dirName = Path.GetFileName(dir);
//////////////                    Console.WriteLine($"Dir: {dirName}");

//////////////                    var dirNode = new NodeModel(position: new Point(childX, childY))
//////////////                    {
//////////////                        Title = dirName,
//////////////                        //Style = new NodeStyle
//////////////                        //{
//////////////                        //    BackgroundColor = "lightgray",
//////////////                        //    BorderColor = "black"
//////////////                        //}
//////////////                    };

//////////////                    Diagram.Nodes.Add(dirNode);
//////////////                    Diagram.Links.Add(new LinkModel(currentDirectoryNode, dirNode));

//////////////                    // Recursively render subdirectories
//////////////                    await RenderSourceCodeStructure(dir, dirNode, currentLevel + 1);

//////////////                    // Move the next directory node down
//////////////                    childY += offsetY;
//////////////                }
//////////////            }
//////////////            catch (UnauthorizedAccessException)
//////////////            {
//////////////                Console.WriteLine($"Access denied: {path}");
//////////////            }
//////////////            catch (Exception ex)
//////////////            {
//////////////                Console.WriteLine($"Error: {ex.Message}");
//////////////            }
//////////////        }

//////////////        protected override void OnInitialized()
//////////////        {
//////////////            var options = new BlazorDiagramOptions
//////////////            {
//////////////                AllowMultiSelection = true,
//////////////                Zoom =
//////////////                {
//////////////                    Enabled = true,
//////////////                    ScaleFactor = 1.2,
//////////////                    Inverse = false
//////////////                },
//////////////                AllowPanning = true,
//////////////                GridSize = 20,
//////////////                Links =
//////////////                {
//////////////                    DefaultRouter = new NormalRouter(),
//////////////                    DefaultPathGenerator = new SmoothPathGenerator()
//////////////                },
//////////////            };

//////////////            Diagram = new BlazorDiagram(options);

//////////////            // Call the method to load the file structure
//////////////            _ = GetLocalRepoFileStructure();
//////////////        }
//////////////    }
//////////////}


//////////////using System;
//////////////using System.IO;
//////////////using System.Threading.Tasks;
//////////////using Blazor.Diagrams;
//////////////using Blazor.Diagrams.Core.Geometry;
//////////////using Blazor.Diagrams.Core.Models;
//////////////using Blazor.Diagrams.Core.PathGenerators;
//////////////using Blazor.Diagrams.Core.Routers;
//////////////using Blazor.Diagrams.Options;

//////////////namespace DeploymentManager.Web.Pages
//////////////{
//////////////    public partial class MyDiagram
//////////////    {
//////////////        public BlazorDiagram Diagram { get; set; } = null!;

//////////////        private async Task GetLocalRepoFileStructure()
//////////////        {
//////////////            string rootPath = @"C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint"; // Target directory

//////////////            if (Directory.Exists(rootPath))
//////////////            {
//////////////                Console.WriteLine($"File structure of: {rootPath}");
//////////////                await RenderSourceCodeStructure(rootPath, null);
//////////////            }
//////////////            else
//////////////            {
//////////////                Console.WriteLine("The specified directory does not exist.");
//////////////            }
//////////////        }

//////////////        private async Task RenderSourceCodeStructure(string path, NodeModel? parentNode)
//////////////        {
//////////////            try
//////////////            {
//////////////                // Create a node for the current directory if it has a parent
//////////////                var currentDirectoryNode = parentNode ?? new NodeModel(position: new Point(50, 50))
//////////////                {
//////////////                    Title = Path.GetFileName(path),
//////////////                };

//////////////                if (parentNode is null)
//////////////                {
//////////////                    Diagram.Nodes.Add(currentDirectoryNode);
//////////////                }

//////////////                // Iterate over all .cs files in the current directory
//////////////                foreach (var file in Directory.GetFiles(path, "*.cs"))
//////////////                {
//////////////                    var fileName = Path.GetFileName(file);
//////////////                    Console.WriteLine($"File: {fileName}");

//////////////                    // Create a node for the file
//////////////                    var fileNode = new NodeModel(position: new Point(currentDirectoryNode.Position.X + 200, currentDirectoryNode.Position.Y + 50))
//////////////                    {
//////////////                        Title = fileName,
//////////////                        //Style = new NodeStyle
//////////////                        //{
//////////////                        //    BackgroundColor = "lightblue", // Differentiate files by color
//////////////                        //    BorderColor = "black"
//////////////                        //}
//////////////                    };

//////////////                    Diagram.Nodes.Add(fileNode);

//////////////                    // Link the directory to the file
//////////////                    Diagram.Links.Add(new LinkModel(currentDirectoryNode, fileNode));
//////////////                }

//////////////                // Iterate over all subdirectories
//////////////                foreach (var dir in Directory.GetDirectories(path))
//////////////                {
//////////////                    var dirName = Path.GetFileName(dir);
//////////////                    Console.WriteLine($"Dir: {dirName}");

//////////////                    // Recursively call for subdirectories
//////////////                    await RenderSourceCodeStructure(dir, currentDirectoryNode);
//////////////                }
//////////////            }
//////////////            catch (UnauthorizedAccessException)
//////////////            {
//////////////                Console.WriteLine($"Access denied: {path}");
//////////////            }
//////////////            catch (Exception ex)
//////////////            {
//////////////                Console.WriteLine($"Error: {ex.Message}");
//////////////            }
//////////////        }

//////////////        protected override void OnInitialized()
//////////////        {
//////////////            var options = new BlazorDiagramOptions
//////////////            {
//////////////                AllowMultiSelection = true,
//////////////                Zoom =
//////////////                {
//////////////                    Enabled = true,
//////////////                    ScaleFactor = 1.2,
//////////////                    Inverse = false
//////////////                },
//////////////                AllowPanning = true,
//////////////                GridSize = 20,
//////////////                Links =
//////////////                {
//////////////                    DefaultRouter = new NormalRouter(),
//////////////                    DefaultPathGenerator = new SmoothPathGenerator()
//////////////                },
//////////////            };

//////////////            Diagram = new BlazorDiagram(options);

//////////////            // Call the method to load the file structure
//////////////            _ = GetLocalRepoFileStructure();
//////////////        }
//////////////    }
//////////////}


//////////////using System.Text.Json.Serialization;
//////////////using Blazor.Diagrams;
//////////////using Blazor.Diagrams.Core.Anchors;
//////////////using Blazor.Diagrams.Core.Geometry;
//////////////using Blazor.Diagrams.Core.Models;
//////////////using Blazor.Diagrams.Core.PathGenerators;
//////////////using Blazor.Diagrams.Core.Routers;
//////////////using Blazor.Diagrams.Options;
//////////////using Newtonsoft.Json;

//////////////namespace DeploymentManager.Web.Pages;

//////////////public partial class MyDiagram
//////////////{
//////////////    public BlazorDiagram Diagram { get; set; } = null!;

//////////////    private async Task GetGithubRepoFileStructure()
//////////////    {

//////////////    }

//////////////    private async Task GetLocalRepoFileStructure()
//////////////    {
//////////////        string rootPath = @"C:\Development\Development-Projects\SaaS-Factory"; 

//////////////        if (Directory.Exists(rootPath))
//////////////        {
//////////////            Console.WriteLine($"File structure of: {rootPath}");
//////////////            PrintFileStructure(rootPath, "");
//////////////        }
//////////////        else
//////////////        {
//////////////            Console.WriteLine("The specified directory does not exist.");
//////////////        }
//////////////    }

//////////////    private async Task PrintFileStructure(string path, string indent)
//////////////    {
//////////////        try
//////////////        {
//////////////            // Print all files in the current directory
//////////////            foreach (var file in Directory.GetFiles(path, "*.cs"))
//////////////            {
//////////////                Console.WriteLine($"{indent}File: {Path.GetFileName(file)}");
//////////////            }

//////////////            // Print all subdirectories in the current directory
//////////////            foreach (var dir in Directory.GetDirectories(path))
//////////////            {
//////////////                Console.WriteLine($"{indent}Dir: {Path.GetFileName(dir)}");
//////////////                // Recursively call for subdirectories
//////////////                PrintFileStructure(dir, indent + "  ");
//////////////            }
//////////////        }
//////////////        catch (UnauthorizedAccessException)
//////////////        {
//////////////            Console.WriteLine($"{indent}Access denied: {path}");
//////////////        }
//////////////        catch (Exception ex)
//////////////        {
//////////////            Console.WriteLine($"{indent}Error: {ex.Message}");
//////////////        }
//////////////    }

//////////////    protected override void OnInitialized()
//////////////    {
//////////////        var options = new BlazorDiagramOptions
//////////////        {

//////////////            AllowMultiSelection = true,
//////////////            Zoom =
//////////////            {
//////////////                Enabled = false,
//////////////                ScaleFactor = 2.0,
//////////////                Inverse = true                
//////////////            },
//////////////            AllowPanning = true,
//////////////            GridSize = 10,            
//////////////            Links =
//////////////            {
//////////////                DefaultRouter = new NormalRouter(),
//////////////                DefaultPathGenerator = new SmoothPathGenerator()
//////////////            },            
//////////////        };

//////////////        Diagram = new BlazorDiagram(options);

//////////////        var file = File.ReadAllText(@"C:\Users\caspe\Downloads\csharp_class_relationships.json");

//////////////        var classRelationships = JsonConvert.DeserializeObject<ClassRelationships>(file);

//////////////        // Create nodes with grid positioning
//////////////        int spacingX = 200;
//////////////        int spacingY = 150;
//////////////        int currentX = 50;
//////////////        int currentY = 50;
//////////////        int rowCounter = 0;

//////////////        // call github api to get repo file structure

//////////////        // render file structure in diagram

//////////////        // change color of diagram node based on the file type (.cs, docker-compose, .md and so on)

//////////////        GetLocalRepoFileStructure();


//////////////        foreach (var class1 in classRelationships.classes)
//////////////        {
//////////////            if (class1.methods > 0) {
//////////////                // class has methods and method calls and should be rendered

//////////////                Diagram.Nodes.Add(new NodeModel(position: new Point(currentX, currentY))
//////////////                {
//////////////                    Title = class1.name
//////////////                });
//////////////                // Update x position for the next node
//////////////                currentX += spacingX;
//////////////                rowCounter++;

//////////////                if (rowCounter % 5 == 0) // Move to next row after 5 nodes
//////////////                {
//////////////                    currentX = 50;
//////////////                    currentY += spacingY;
//////////////                }
//////////////            }           
//////////////        }

//////////////        foreach (var relationship in classRelationships.relationships)
//////////////        {
//////////////            var sourceNode = Diagram.Nodes.FirstOrDefault(x => x.Title == relationship.from);
//////////////            var targetNode = Diagram.Nodes.FirstOrDefault(x => x.Title == relationship.to);

//////////////            sourceNode?.AddPort(PortAlignment.Left);
//////////////            targetNode?.AddPort(PortAlignment.Right);

//////////////            if (sourceNode is not null && targetNode is not null)
//////////////            {
//////////////                var link = new LinkModel(sourceNode, targetNode);
//////////////                Diagram.Links.Add(link);

//////////////                var sourceAnchor = new ShapeIntersectionAnchor(sourceNode);
//////////////                var targetAnchor = new ShapeIntersectionAnchor(targetNode);

//////////////                Diagram.Links.Add(new LinkModel(sourceAnchor, targetAnchor));
//////////////            }
//////////////        }

//////////////    }

//////////////    private void Nodes_Added(NodeModel obj)
//////////////    {
//////////////        Console.WriteLine("node " + obj.Title + " added");
//////////////    }
//////////////}

////////////////int x = 50;
////////////////int y = 50;
////////////////int maxWidth = 5200; // Adjust this value based on the desired width of the grid
////////////////int spacing = 100; // Adjust this value to increase or decrease the spacing between nodes

//////////////// If the x position exceeds the max width, move to the next row
////////////////if (x > maxWidth)
////////////////{
////////////////    x = 50; // Reset x to the initial value
////////////////    y += spacing; // Move down to the next row
////////////////}


//////////////// var firstNode = Diagram.Nodes.Add(new NodeModel(position: new Point(50, 50))
//////////////// {
////////////////     Title = "Node 1"
//////////////// });
//////////////// var secondNode = Diagram.Nodes.Add(new NodeModel(position: new Point(200, 100))
//////////////// {
////////////////     Title = "Node 2"
//////////////// });
//////////////// var leftPort = secondNode.AddPort(PortAlignment.Left);
//////////////// var rightPort = secondNode.AddPort(PortAlignment.Right);

//////////////// // The connection point will be the intersection of
//////////////// // a line going from the target to the center of the source
//////////////// var sourceAnchor = new ShapeIntersectionAnchor(firstNode);
//////////////// // The connection point will be the port's position
//////////////// var targetAnchor = new SinglePortAnchor(leftPort);
//////////////// var link = Diagram.Links.Add(new LinkModel(sourceAnchor, targetAnchor));

//////////////// implementer zoom animation på et subsektion i diagrammet - dvs zoom ind på en mappe hvor der fokuseres på dens undermapper ligesom i prezi presentationer
//////////////// https://prezi.com/p/dwmcmsh3ree5/rage-room/

//////////////// Output the first 10 positions to keep the output manageable
////////////////for (int i = 0; i < 10; i++)
////////////////{
////////////////    var position = boxPositions[i];
////////////////    Console.WriteLine($"Box {i + 1}: ({position.X}, {position.Y})");
////////////////}

////////////////double centerX = 2560, centerY = 720; // Center of the screen

////////////////double boxWidth = 50, boxHeight = 50; // Size of each box


////////////////_ = new NodeModel(position: new Point(centerX, centerY))
////////////////{
////////////////    Title = "One Drive Root",
////////////////    Size = new Size(boxWidth, boxHeight)
////////////////};

////////////////var nodes = File.ReadAllLines(@"nodes.txt").ToList();

////////////////int numberOfBoxes = nodes.Count;

/////////////////* double radius = 500;*/ // Chosen radius for the circular layout

////////////////double spacing = 50; // Spacing between boxes


////////////////var boxPositions = CircularLayoutCalculator.CalculateBoxPositions(numberOfBoxes, centerX, centerY, boxWidth, boxHeight, spacing);

////////////////Box 1: (3060, 720)
////////////////Box 2: (3059.753280182866, 735.7053795390641)
////////////////Box 3: (3059.013364214136, 751.3952597646567)
////////////////Box 4: (3057.78098230154, 767.0541566592572)
////////////////Box 5: (3056.057350657239, 782.6666167821521)
////////////////Box 6: (3053.844170297569, 798.2172325201154)
////////////////Box 7: (3051.143625364344, 813.6906572928623)
////////////////Box 8: (3047.9583809693736, 829.0716206982713)
////////////////Box 9: (3044.2915805643156, 844.3449435824274)
////////////////Box 10: (3040.1468428384715, 859.4955530196146)

////////////////var centerNode = new NodeModel(position: new Point(centerX, centerY))
////////////////{
////////////////    Title = "ROOT",
////////////////    Size = new Size(200, 200)
////////////////};

////////////////int boxIndex = 0;
////////////////foreach (var node in nodes)
////////////////{
////////////////    //var position = boxPositions[boxIndex];

////////////////    //var newNode = new NodeModel(position: new Point(position.X, position.Y)) { Title = node };

////////////////    //newNode.RefreshAll();

////////////////    //Diagram.Nodes.Add(newNode);

////////////////    //var link = Diagram.Links.Add(new LinkModel(centerNode, newNode));

////////////////    //Diagram.Nodes.Added += Nodes_Added;

////////////////    //newNode.RefreshAll();

////////////////    //var leftPort = newNode.AddPort(PortAlignment.Left);
////////////////    //var rightPort = newNode.AddPort(PortAlignment.Right);

////////////////    // The connection point will be the intersection of
////////////////    // a line going from the target to the center of the source
////////////////    //var sourceAnchor = new ShapeIntersectionAnchor(centerNode);
////////////////    //// The connection point will be the port's position
////////////////    //var targetAnchor = new SinglePortAnchor(leftPort);
////////////////    //var link = Diagram.Links.Add(new LinkModel(sourceAnchor, targetAnchor));

////////////////    boxIndex++;
////////////////}

////////////////x = x += 30;
////////////////y = y += 50;

////////////////int x = 10;
////////////////int y = 10;


////////////////File.ReadAllLines(File.ReadAllText("nodes.txt")).ToList().ForEach(x => Diagram.Nodes.Add(new NodeModel(position: new Point(50, 50)) { Title = x }));

////////////////var firstNode = Diagram.Nodes.Add(new NodeModel(position: new Point(50, 50))
////////////////{
////////////////    Title = "Node 1"
////////////////});
////////////////var secondNode = Diagram.Nodes.Add(new NodeModel(position: new Point(200, 100))
////////////////{
////////////////    Title = "Node 2"
////////////////});
////////////////var leftPort = secondNode.AddPort(PortAlignment.Left);
////////////////var rightPort = secondNode.AddPort(PortAlignment.Right);

////////////////// The connection point will be the intersection of
////////////////// a line going from the target to the center of the source
////////////////var sourceAnchor = new ShapeIntersectionAnchor(firstNode);
////////////////// The connection point will be the port's position
////////////////var targetAnchor = new SinglePortAnchor(leftPort);
////////////////var link = Diagram.Links.Add(new LinkModel(sourceAnchor, targetAnchor));



