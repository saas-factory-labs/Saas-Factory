using System.Diagnostics;
using System.Net.Sockets;

namespace AppBlueprint.DeveloperCli.Commands;

internal static class RunCommand
{
    private const int DefaultDashboardPort = 18888;

    public static Command Create()
    {
        var command = new Command("run", "Start the development environment (AppHost with all services)");

        var portOption = new Option<int>(
            ["--port", "-p"],
            getDefaultValue: () => DefaultDashboardPort,
            "Port for the Aspire dashboard");
        command.AddOption(portOption);

        var watchOption = new Option<bool>(
            ["--watch", "-w"],
            getDefaultValue: () => false,
            "Enable hot reload (watch mode)");
        command.AddOption(watchOption);

        command.SetHandler((int port, bool watch) =>
        {
            ExecuteServe(port, watch);
        }, portOption, watchOption);

        return command;
    }

    private static void ExecuteServe(int port, bool watch)
    {
        AnsiConsole.Write(new FigletText("SaaS Factory").Color(Color.Cyan1));
        AnsiConsole.MarkupLine("[green]🚀 Starting development environment...[/]\n");

        // Find AppHost project
        string currentDir = Directory.GetCurrentDirectory();
        string? appHostPath = FindAppHostProject(currentDir);

        if (string.IsNullOrEmpty(appHostPath))
        {
            AnsiConsole.MarkupLine("[red]❌ AppHost project not found![/]");
            AnsiConsole.MarkupLine("[yellow]💡 Please run this command from the AppBlueprint directory[/]");
            AnsiConsole.MarkupLine("[dim]   or any subdirectory within the project[/]\n");
            return;
        }

        string appHostDir = Path.GetDirectoryName(appHostPath) ?? string.Empty;
        string appHostName = Path.GetFileNameWithoutExtension(appHostPath);

        AnsiConsole.MarkupLine($"[blue]📁 AppHost:[/] {appHostName}");
        AnsiConsole.MarkupLine($"[blue]📂 Location:[/] {appHostDir}");
        AnsiConsole.MarkupLine($"[blue]🎛️  Dashboard:[/] http://localhost:{port}\n");

        // Check if dashboard port is already in use
        if (IsPortInUse(port))
        {
            AnsiConsole.MarkupLine($"[yellow]⚠️  Port {port} is already in use![/]");
            AnsiConsole.MarkupLine("[yellow]   AppHost might already be running.[/]\n");

            if (!AnsiConsole.Confirm("Do you want to continue anyway?", false))
            {
                AnsiConsole.MarkupLine("[yellow]Cancelled. Use --port to specify a different port.[/]");
                return;
            }

            AnsiConsole.WriteLine();
        }

        // Display what will be started
        var servicesPanel = new Panel(
            new Markup(
                "[green]✓[/] PostgreSQL Database\n" +
                "[green]✓[/] API Service\n" +
                "[green]✓[/] Web UI (Blazor)\n" +
                "[green]✓[/] Background Workers\n" +
                "[green]✓[/] Service Discovery\n" +
                "[green]✓[/] OpenTelemetry Dashboard"
            ))
        {
            Header = new PanelHeader("[cyan]Services Starting[/]"),
            Border = BoxBorder.Rounded,
            Padding = new Padding(2, 0)
        };
        AnsiConsole.Write(servicesPanel);
        AnsiConsole.WriteLine();

        // Prepare the command
        string arguments = watch ? "watch --project" : "run --project";
        arguments += $" \"{appHostPath}\"";

        AnsiConsole.MarkupLine($"[dim]Executing: dotnet {arguments}[/]\n");

        // Start AppHost process
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = appHostDir
            }
        };

        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                // Highlight important messages
                if (e.Data.Contains("Now listening on", StringComparison.OrdinalIgnoreCase) ||
                    e.Data.Contains("Application started", StringComparison.OrdinalIgnoreCase) ||
                    e.Data.Contains("dashboard", StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine($"[green]{Markup.Escape(e.Data)}[/]");
                }
                else if (e.Data.Contains("error", StringComparison.OrdinalIgnoreCase) ||
                         e.Data.Contains("fail", StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine($"[red]{Markup.Escape(e.Data)}[/]");
                }
                else if (e.Data.Contains("warn", StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine($"[yellow]{Markup.Escape(e.Data)}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[dim]{Markup.Escape(e.Data)}[/]");
                }
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                AnsiConsole.MarkupLine($"[red]{Markup.Escape(e.Data)}[/]");
            }
        };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            AnsiConsole.MarkupLine("[green]✅ Development environment starting...[/]");
            AnsiConsole.MarkupLine($"[blue]🌐 Dashboard will be available at:[/] http://localhost:{port}");
            AnsiConsole.MarkupLine($"[blue]🔌 Services will start automatically[/]\n");

            if (watch)
            {
                AnsiConsole.MarkupLine("[yellow]🔥 Hot reload enabled - changes will be applied automatically[/]\n");
            }

            AnsiConsole.MarkupLine("[dim]Press Ctrl+C to stop all services[/]\n");
            AnsiConsole.Write(new Rule("[grey]Service Logs[/]") { Justification = Justify.Left });
            AnsiConsole.WriteLine();

            // Wait for process to exit
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                AnsiConsole.MarkupLine($"\n[red]❌ Process exited with code: {process.ExitCode}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("\n[green]✅ Development environment stopped[/]");
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"\n[red]❌ Failed to start development environment:[/]");
            AnsiConsole.WriteException(ex);
        }
    }

    private static string? FindAppHostProject(string startPath)
    {
        // Search patterns for AppHost project
        string[] patterns = ["*.AppHost.csproj", "AppHost.csproj"];

        // Search current directory and parent directories
        string? currentPath = startPath;

        while (currentPath != null)
        {
            foreach (string pattern in patterns)
            {
                string[] found = Directory.GetFiles(currentPath, pattern, SearchOption.AllDirectories);
                if (found.Length > 0)
                {
                    // If multiple found, prefer the one with "AppBlueprint.AppHost"
                    string? preferred = found.FirstOrDefault(f =>
                        f.Contains("AppBlueprint.AppHost", StringComparison.OrdinalIgnoreCase));

                    return preferred ?? found[0];
                }
            }

            // Move up one directory
            DirectoryInfo? parent = Directory.GetParent(currentPath);
            currentPath = parent?.FullName;

            // Stop at the Code directory or root
            if (currentPath != null &&
                (Path.GetFileName(currentPath).Equals("Code", StringComparison.OrdinalIgnoreCase) ||
                 parent == null))
            {
                break;
            }
        }

        return null;
    }

    private static bool IsPortInUse(int port)
    {
        try
        {
            using var tcpClient = new TcpClient();
            tcpClient.Connect("localhost", port);
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
        catch
        {
            // If any other error, assume port is available
            return false;
        }
    }

    public static void ExecuteInteractive()
    {
        AnsiConsole.MarkupLine("[yellow]🚀 Starting development environment in interactive mode...[/]\n");

        bool enableWatch = AnsiConsole.Confirm("Enable [green]hot reload[/] (watch mode)?", false);

        int port = DefaultDashboardPort;
        if (AnsiConsole.Confirm($"Use default dashboard port [green]{DefaultDashboardPort}[/]?", true))
        {
            port = DefaultDashboardPort;
        }
        else
        {
            port = AnsiConsole.Ask("Enter dashboard port:", DefaultDashboardPort);
        }

        AnsiConsole.WriteLine();
        ExecuteServe(port, enableWatch);
    }
}
