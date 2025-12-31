using System.Diagnostics;
using AppBlueprint.DeveloperCli.Utilities;

namespace AppBlueprint.DeveloperCli.Commands;

/// <summary>
/// Command to run tests with various options (watch mode, coverage, filtering).
/// Provides a better developer experience than raw dotnet test.
/// </summary>
internal static class TestCommand
{
    private const string DefaultTestProjectPath = @"C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint";

    public static Command Create()
    {
        var command = new Command("test", "Run tests with optional coverage and filtering");

        // Options
        var watchOption = new Option<bool>(
            "--watch",
            description: "Run tests in watch mode (auto-rerun on file changes)",
            getDefaultValue: () => false);

        var coverageOption = new Option<bool>(
            "--coverage",
            description: "Generate code coverage report",
            getDefaultValue: () => false);

        var filterOption = new Option<string?>(
            "--filter",
            description: "Filter tests by name or category (e.g., 'FullyQualifiedName~UnitTests')",
            getDefaultValue: () => null);

        var projectOption = new Option<string?>(
            "--project",
            description: "Path to specific test project (default: all test projects)",
            getDefaultValue: () => null);

        var verbosityOption = new Option<string>(
            "--verbosity",
            description: "Logging verbosity (quiet, minimal, normal, detailed, diagnostic)",
            getDefaultValue: () => "normal");

        var noRestoreOption = new Option<bool>(
            "--no-restore",
            description: "Do not restore dependencies before running tests",
            getDefaultValue: () => false);

        var noBuildOption = new Option<bool>(
            "--no-build",
            description: "Do not build the project before running tests",
            getDefaultValue: () => false);

        command.AddOption(watchOption);
        command.AddOption(coverageOption);
        command.AddOption(filterOption);
        command.AddOption(projectOption);
        command.AddOption(verbosityOption);
        command.AddOption(noRestoreOption);
        command.AddOption(noBuildOption);

        command.SetHandler(async (watch, coverage, filter, project, verbosity, noRestore, noBuild) =>
        {
            await ExecuteTests(watch, coverage, filter, project, verbosity, noRestore, noBuild);
        }, watchOption, coverageOption, filterOption, projectOption, verbosityOption, noRestoreOption, noBuildOption);

        return command;
    }

    private static async Task ExecuteTests(
        bool watch,
        bool coverage,
        string? filter,
        string? project,
        string verbosity,
        bool noRestore,
        bool noBuild)
    {
        AnsiConsole.Write(new FigletText("Test Runner").Color(Color.Cyan1));
        AnsiConsole.WriteLine();

        // Determine working directory (not the project file path)
        string workingDirectory = GetDefaultTestProjectPath();

        // Build dotnet test command
        var args = new List<string>();

        if (watch)
        {
            args.Add("watch");
        }

        args.Add("test");

        // Default to AppBlueprint.Tests project if no project specified
        if (!string.IsNullOrEmpty(project))
        {
            args.Add($"\"{project}\"");
        }
        else
        {
            // Run tests only on the test project, not the entire solution (avoids docker-compose.dcproj issues)
            string testProject = Path.Combine(workingDirectory, "AppBlueprint.Tests", "AppBlueprint.Tests.csproj");
            if (File.Exists(testProject))
            {
                args.Add($"\"{testProject}\"");
            }
        }

        if (!string.IsNullOrEmpty(filter))
        {
            args.Add($"--filter");
            args.Add($"\"{filter}\"");
        }

        if (coverage)
        {
            args.Add("/p:CollectCoverage=true");
            args.Add("/p:CoverletOutputFormat=opencover");
            args.Add("/p:CoverletOutput=./coverage/");
        }

        args.Add($"--verbosity");
        args.Add(verbosity);

        if (noRestore)
        {
            args.Add("--no-restore");
        }

        if (noBuild)
        {
            args.Add("--no-build");
        }

        // Display configuration
        var configTable = new Table()
            .Border(TableBorder.Rounded)
            .Title("[yellow]Test Configuration[/]")
            .AddColumn(new TableColumn("[cyan]Setting[/]"))
            .AddColumn(new TableColumn("[cyan]Value[/]"));

        configTable.AddRow("Watch Mode", watch ? "[green]✓ Enabled[/]" : "[dim]Disabled[/]");
        configTable.AddRow("Coverage", coverage ? "[green]✓ Enabled[/]" : "[dim]Disabled[/]");
        configTable.AddRow("Filter", !string.IsNullOrEmpty(filter) ? $"[yellow]{filter}[/]" : "[dim]None[/]");
        configTable.AddRow("Project", project ?? "[dim]All test projects[/]");
        configTable.AddRow("Verbosity", verbosity);

        AnsiConsole.Write(configTable);
        AnsiConsole.WriteLine();

        // Execute tests
        string command = $"dotnet {string.Join(" ", args)}";

        AnsiConsole.MarkupLine($"[dim]Command: {command.EscapeMarkup()}[/]");
        AnsiConsole.WriteLine();

        if (watch)
        {
            // For watch mode, show a message and run directly
            AnsiConsole.MarkupLine("[yellow]⚡ Running tests in watch mode. Press Ctrl+C to exit.[/]");
            AnsiConsole.WriteLine();

            await RunTestProcess(command, workingDirectory, watch: true);
        }
        else
        {
            // For normal mode, show a status spinner
            await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync("[yellow]Running tests...[/]", async ctx =>
                {
                    await RunTestProcess(command, workingDirectory, watch: false);
                });
        }

        if (coverage && !watch)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[cyan]📊 Coverage report generated in: ./coverage/[/]");
        }
    }

    private static async Task RunTestProcess(string command, string workingDirectory, bool watch)
    {
        ProcessStartInfo psi = new()
        {
            FileName = "powershell.exe",
            Arguments = $"-NoProfile -Command \"{command}\"",
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = !watch,
            RedirectStandardError = !watch,
            CreateNoWindow = false
        };

        using Process? process = Process.Start(psi);

        if (process is null)
        {
            AnsiConsole.MarkupLine("[red]✗ Failed to start test process[/]");
            return;
        }

        if (watch)
        {
            // In watch mode, let the process run interactively
            await process.WaitForExitAsync();
        }
        else
        {
            // In normal mode, capture output
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            // Display output
            if (!string.IsNullOrWhiteSpace(output))
            {
                AnsiConsole.WriteLine(output);
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                AnsiConsole.MarkupLine($"[red]{error.EscapeMarkup()}[/]");
            }

            // Show result
            AnsiConsole.WriteLine();
            if (process.ExitCode == 0)
            {
                var panel = new Panel("[green]✓ All tests passed![/]")
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = new Style(Color.Green)
                };
                AnsiConsole.Write(panel);
            }
            else
            {
                var panel = new Panel($"[red]✗ Tests failed with exit code: {process.ExitCode}[/]")
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = new Style(Color.Red)
                };
                AnsiConsole.Write(panel);
            }
        }
    }

    private static string GetDefaultTestProjectPath()
    {
        // Try to find the test project from current directory
        string currentDir = Directory.GetCurrentDirectory();

        // Look for AppBlueprint directory
        DirectoryInfo? directory = new(currentDir);

        while (directory is not null)
        {
            string appBlueprintPath = Path.Combine(directory.FullName, "Code", "AppBlueprint");
            if (Directory.Exists(appBlueprintPath))
            {
                return appBlueprintPath;
            }

            directory = directory.Parent;
        }

        // Fallback to hardcoded path
        return DefaultTestProjectPath;
    }

    public static void ExecuteInteractive()
    {
        AnsiConsole.Write(new FigletText("Test Runner").Color(Color.Cyan1));
        AnsiConsole.WriteLine();

        var testOptions = new[]
        {
            "Run all tests",
            "Run tests with coverage",
            "Run tests in watch mode",
            "Run specific test project",
            "Run filtered tests",
            "Back to main menu"
        };

        string choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]What would you like to do?[/]")
                .PageSize(10)
                .AddChoices(testOptions));

        switch (choice)
        {
            case "Run all tests":
                ExecuteTests(watch: false, coverage: false, filter: null, project: null, verbosity: "normal", noRestore: false, noBuild: false).Wait();
                break;

            case "Run tests with coverage":
                ExecuteTests(watch: false, coverage: true, filter: null, project: null, verbosity: "normal", noRestore: false, noBuild: false).Wait();
                break;

            case "Run tests in watch mode":
                ExecuteTests(watch: true, coverage: false, filter: null, project: null, verbosity: "minimal", noRestore: false, noBuild: false).Wait();
                break;

            case "Run specific test project":
                string projectPath = AnsiConsole.Ask<string>("[green]Enter test project path:[/]");
                ExecuteTests(watch: false, coverage: false, filter: null, project: projectPath, verbosity: "normal", noRestore: false, noBuild: false).Wait();
                break;

            case "Run filtered tests":
                string filter = AnsiConsole.Ask<string>("[green]Enter test filter (e.g., 'FullyQualifiedName~UnitTests'):[/]");
                ExecuteTests(watch: false, coverage: false, filter: filter, project: null, verbosity: "normal", noRestore: false, noBuild: false).Wait();
                break;

            case "Back to main menu":
                return;
        }
    }
}
