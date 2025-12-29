using System.Diagnostics;

namespace AppBlueprint.DeveloperCli.Commands;

/// <summary>
/// Command to display current development environment information.
/// Shows configuration, dependencies, and service status.
/// </summary>
internal static class EnvironmentInfoCommand
{
    public static Command Create()
    {
        var command = new Command("env:info", "Show development environment information and status");

        command.SetHandler(() =>
        {
            DisplayEnvironmentInfo();
        });

        return command;
    }

    private static void DisplayEnvironmentInfo()
    {
        AnsiConsole.Write(new FigletText("Environment Info").Color(Color.Cyan1));
        AnsiConsole.WriteLine();

        // Create panels for different sections
        DisplayConfigurationInfo();
        AnsiConsole.WriteLine();
        
        DisplayServicesStatus();
        AnsiConsole.WriteLine();
        
        DisplaySystemInfo();
        AnsiConsole.WriteLine();
        
        DisplayProjectPaths();
    }

    private static void DisplayConfigurationInfo()
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[yellow]Configuration[/]")
            .AddColumn(new TableColumn("[cyan]Setting[/]").Width(35))
            .AddColumn(new TableColumn("[cyan]Value[/]"));

        // Database
        string dbConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Database") 
            ?? Environment.GetEnvironmentVariable("APPBLUEPRINT_DATABASE_CONNECTIONSTRING")
            ?? "Not set";
        
        // Mask password in connection string for display
        string displayDbConnection = MaskPassword(dbConnectionString);
        
        table.AddRow(
            "Database Connection",
            dbConnectionString == "Not set" ? "[yellow]Not set[/]" : $"[dim]{displayDbConnection}[/]"
        );

        // Authentication
        string authProvider = Environment.GetEnvironmentVariable("Authentication__Provider") ?? "Not configured";
        table.AddRow("Auth Provider", authProvider == "Not configured" ? "[yellow]Not configured[/]" : $"[green]{authProvider}[/]");

        // Environment
        string aspnetEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        table.AddRow("ASP.NET Environment", $"[green]{aspnetEnv}[/]");

        string dotnetEnv = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Not set";
        if (dotnetEnv != "Not set")
        {
            table.AddRow("DOTNET Environment", $"[dim]{dotnetEnv}[/]");
        }

        // Logto Configuration (if exists)
        string logtoEndpoint = Environment.GetEnvironmentVariable("Authentication__Logto__Endpoint") ?? "Not set";
        if (logtoEndpoint != "Not set")
        {
            table.AddRow("Logto Endpoint", $"[dim]{logtoEndpoint}[/]");
        }

        string logtoClientId = Environment.GetEnvironmentVariable("Authentication__Logto__ClientId") ?? "Not set";
        if (logtoClientId != "Not set")
        {
            table.AddRow("Logto Client ID", $"[dim]{logtoClientId[..Math.Min(20, logtoClientId.Length)]}...[/]");
        }

        AnsiConsole.Write(table);
    }

    private static void DisplayServicesStatus()
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[yellow]Services Status[/]")
            .AddColumn(new TableColumn("[cyan]Service[/]").Width(35))
            .AddColumn(new TableColumn("[cyan]Status[/]"));

        // Check Docker
        bool dockerRunning = CheckDockerRunning();
        table.AddRow(
            "Docker Desktop",
            dockerRunning ? "[green]✓ Running[/]" : "[red]✗ Not running[/]"
        );

        // Check PostgreSQL (via Docker or local)
        bool postgresRunning = CheckPostgreSqlRunning();
        table.AddRow(
            "PostgreSQL",
            postgresRunning ? "[green]✓ Running[/]" : "[yellow]⚠ Unable to verify[/]"
        );

        // Check if AppHost is running (check common ports)
        bool appHostRunning = CheckPortInUse(5001) || CheckPortInUse(15001);
        table.AddRow(
            "AppHost (Aspire)",
            appHostRunning ? "[green]✓ Running (port 5001/15001)[/]" : "[dim]Not running[/]"
        );

        // Check API Service
        bool apiRunning = CheckPortInUse(8091);
        table.AddRow(
            "API Service",
            apiRunning ? "[green]✓ Running (port 8091)[/]" : "[dim]Not running[/]"
        );

        // Check Web App
        bool webRunning = CheckPortInUse(8092);
        table.AddRow(
            "Web App (Blazor)",
            webRunning ? "[green]✓ Running (port 8092)[/]" : "[dim]Not running[/]"
        );

        AnsiConsole.Write(table);
    }

    private static void DisplaySystemInfo()
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[yellow]System Information[/]")
            .AddColumn(new TableColumn("[cyan]Component[/]").Width(35))
            .AddColumn(new TableColumn("[cyan]Version/Status[/]"));

        // .NET SDK
        string dotnetVersion = GetDotNetVersion();
        table.AddRow(".NET SDK", $"[green]{dotnetVersion}[/]");

        // Operating System
        table.AddRow("Operating System", $"[dim]{Environment.OSVersion}[/]");

        // Machine Name
        table.AddRow("Machine Name", $"[dim]{Environment.MachineName}[/]");

        // Current User
        table.AddRow("Current User", $"[dim]{Environment.UserName}[/]");

        // Current Directory
        string currentDir = Directory.GetCurrentDirectory();
        table.AddRow("Current Directory", $"[dim]{currentDir}[/]");

        // Docker Version
        string dockerVersion = GetDockerVersion();
        if (dockerVersion != "Not installed")
        {
            table.AddRow("Docker Version", $"[dim]{dockerVersion}[/]");
        }

        AnsiConsole.Write(table);
    }

    private static void DisplayProjectPaths()
    {
        var tree = new Tree("[yellow]Project Paths[/]");

        string currentDir = Directory.GetCurrentDirectory();
        
        // Try to locate key project directories
        var projectRoot = FindProjectRoot(currentDir);
        
        if (projectRoot is not null)
        {
            var rootNode = tree.AddNode($"[green]Root:[/] [dim]{projectRoot}[/]");
            
            // Check for key directories
            CheckAndAddNode(rootNode, projectRoot, "Code/AppBlueprint", "AppBlueprint");
            CheckAndAddNode(rootNode, projectRoot, "Code/DeploymentManager", "DeploymentManager");
            CheckAndAddNode(rootNode, projectRoot, "docs", "Documentation");
            CheckAndAddNode(rootNode, projectRoot, "Writerside", "Writerside Docs");
        }
        else
        {
            tree.AddNode("[yellow]⚠ Not in SaaS Factory project directory[/]");
        }

        AnsiConsole.Write(tree);
    }

    // Helper methods

    private static string MaskPassword(string connectionString)
    {
        if (connectionString == "Not set")
            return connectionString;

        // Mask password in PostgreSQL connection string
        var regex = new System.Text.RegularExpressions.Regex(
            @"Password=([^;]+)",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        
        return regex.Replace(connectionString, "Password=***");
    }

    private static bool CheckDockerRunning()
    {
#pragma warning disable CA1031 // Acceptable for status check - we want to catch all exceptions
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "info",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process is null)
                return false;

            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
#pragma warning restore CA1031
    }

    private static bool CheckPostgreSqlRunning()
    {
#pragma warning disable CA1031 // Acceptable for status check
        try
        {
            // Try to check if PostgreSQL container is running
            var psi = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "ps --filter name=postgres --format \"{{.Names}}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process is null)
                return false;

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            
            return !string.IsNullOrWhiteSpace(output);
        }
        catch
        {
            return false;
        }
#pragma warning restore CA1031
    }

    private static bool CheckPortInUse(int port)
    {
#pragma warning disable CA1031 // Acceptable for status check
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "netstat",
                Arguments = $"-an",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process is null)
                return false;

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            
            // Check if port appears in netstat output
            return output.Contains($":{port}", StringComparison.Ordinal);
        }
        catch
        {
            return false;
        }
#pragma warning restore CA1031
    }

    private static string GetDotNetVersion()
    {
#pragma warning disable CA1031 // Acceptable for version detection
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process is null)
                return "Not found";

            string version = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
            
            return string.IsNullOrWhiteSpace(version) ? "Not found" : version;
        }
        catch
        {
            return "Not found";
        }
#pragma warning restore CA1031
    }

    private static string GetDockerVersion()
    {
#pragma warning disable CA1031 // Acceptable for version detection
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process is null)
                return "Not installed";

            string version = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();
            
            return string.IsNullOrWhiteSpace(version) ? "Not installed" : version;
        }
        catch
        {
            return "Not installed";
        }
#pragma warning restore CA1031
    }

    private static string? FindProjectRoot(string startDirectory)
    {
        DirectoryInfo? directory = new(startDirectory);

        while (directory is not null)
        {
            // Look for indicators this is the project root
            if (Directory.Exists(Path.Combine(directory.FullName, "Code", "AppBlueprint")) ||
                File.Exists(Path.Combine(directory.FullName, "SaaS-Factory.slnx")) ||
                File.Exists(Path.Combine(directory.FullName, "global.json")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }

    private static void CheckAndAddNode(TreeNode parentNode, string rootPath, string relativePath, string label)
    {
        string fullPath = Path.Combine(rootPath, relativePath);
        
        if (Directory.Exists(fullPath))
        {
            parentNode.AddNode($"[green]✓[/] {label}: [dim]{relativePath}[/]");
        }
        else
        {
            parentNode.AddNode($"[red]✗[/] {label}: [dim]{relativePath}[/] [yellow](not found)[/]");
        }
    }
}
