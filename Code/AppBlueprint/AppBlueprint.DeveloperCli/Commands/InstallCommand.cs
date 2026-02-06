using System.Diagnostics;
using AppBlueprint.DeveloperCli.Utilities;

namespace AppBlueprint.DeveloperCli.Commands;

/// <summary>
/// Command to install the CLI globally so it can be accessed from anywhere using the 'saas' alias
/// </summary>
internal static class InstallCommand
{
    public static Command Create()
    {
        var command = new Command("install", $"Install the '{CliConfiguration.AliasName}' CLI globally");

        var forceOption = new Option<bool>(
            ["--force", "-f"],
            getDefaultValue: () => false,
            "Force reinstall even if already installed");
        command.AddOption(forceOption);

        command.SetHandler((bool force) =>
        {
            Execute(force);
        }, forceOption);

        return command;
    }

    private static void Execute(bool force)
    {
        AnsiConsole.Write(new FigletText("SaaS Factory").Color(Color.Cyan1));
        AnsiConsole.MarkupLine("[green]Installing CLI globally...[/]\n");

        // Check if already installed
        if (!force && IsAlreadyInstalled())
        {
            AnsiConsole.MarkupLine($"[yellow]The CLI is already installed![/]");
            AnsiConsole.MarkupLine($"[yellow]You can use '[green]{CliConfiguration.AliasName}[/]' from anywhere.[/]");
            AnsiConsole.MarkupLine($"[yellow]Use --force to reinstall.[/]\n");
            return;
        }

        // Show intro
        ShowIntro();

        // Confirm installation
        if (!AnsiConsole.Confirm($"\n[cyan]Install '[green]{CliConfiguration.AliasName}[/]' CLI globally?[/]", true))
        {
            AnsiConsole.MarkupLine("[yellow]Installation cancelled.[/]");
            return;
        }

        AnsiConsole.WriteLine();

        // Perform installation steps
        AnsiConsole.Status()
            .Start("Installing...", ctx =>
            {
                // Step 1: Build the CLI in Release mode
                ctx.Status("Building CLI in Release mode...");
                ctx.Spinner(Spinner.Known.Dots);

                if (!BuildCli())
                {
                    throw new Exception("Failed to build CLI");
                }

                AnsiConsole.MarkupLine("[green]✓ CLI built successfully[/]");

                // Step 2: Create installation directory
                ctx.Status("Creating installation directory...");

                string installPath = CliConfiguration.GetGlobalInstallPath();
                Directory.CreateDirectory(installPath);

                AnsiConsole.MarkupLine($"[green]✓ Installation directory: {installPath}[/]");

                // Step 3: Copy files to installation directory
                ctx.Status("Copying files...");

                CopyBuildOutput(installPath);

                AnsiConsole.MarkupLine("[green]✓ Files copied[/]");

                // Step 4: Register in PATH or create alias
                ctx.Status("Registering CLI...");

                if (!RegisterCli(installPath))
                {
                    throw new Exception("Failed to register CLI");
                }

                AnsiConsole.MarkupLine("[green]✓ CLI registered successfully[/]");
            });

        // Show success message
        ShowSuccessMessage();
    }

    private static bool IsAlreadyInstalled()
    {
        if (CliConfiguration.IsWindows)
        {
            string installPath = CliConfiguration.GetGlobalInstallPath();
            return PathHelper.IsInPathWindows(installPath);
        }
        else
        {
            // For macOS/Linux, check if alias exists in shell profile
            string? shellProfilePath = GetShellProfilePath();
            if (shellProfilePath == null || !File.Exists(shellProfilePath))
            {
                return false;
            }

            string content = File.ReadAllText(shellProfilePath);
            return content.Contains($"alias {CliConfiguration.AliasName}=", StringComparison.Ordinal);
        }
    }

    private static void ShowIntro()
    {
        var panel = new Panel(
            new Markup(
                $"[cyan]Welcome to {CliConfiguration.DisplayName}![/]\n\n" +
                $"This will install the '[green]{CliConfiguration.AliasName}[/]' command globally.\n\n" +
                "[yellow]What will happen:[/]\n" +
                "  • CLI will be built in Release mode\n" +
                "  • Files will be copied to a permanent location\n" +
                (CliConfiguration.IsWindows
                    ? "  • Installation directory will be added to your PATH\n"
                    : $"  • Shell alias will be created in your shell profile\n") +
                "\n" +
                $"[green]After installation:[/]\n" +
                $"  • Run '[green]{CliConfiguration.AliasName} run[/]' to start development environment\n" +
                $"  • Run '[green]{CliConfiguration.AliasName} --help[/]' to see all commands\n" +
                $"  • Use '[green]{CliConfiguration.AliasName}[/]' from anywhere on your machine!\n"
            ))
        {
            Header = new PanelHeader("[cyan]Installation Info[/]"),
            Border = BoxBorder.Rounded,
            Padding = new Padding(2, 1)
        };

        AnsiConsole.Write(panel);
    }

    private static bool BuildCli()
    {
        try
        {
            string sourceRoot = CliConfiguration.GetSourceCodeRoot();
            string cliProjectPath = Path.Combine(sourceRoot, "AppBlueprint.DeveloperCli", "AppBlueprint.DeveloperCli.csproj");

            if (!File.Exists(cliProjectPath))
            {
                AnsiConsole.MarkupLine($"[red]Could not find CLI project at: {cliProjectPath}[/]");
                return false;
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"build \"{cliProjectPath}\" -c Release",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = sourceRoot
                }
            };

            process.Start();
            process.WaitForExit();

            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Build failed: {ex.Message}[/]");
            return false;
        }
    }

    private static void CopyBuildOutput(string installPath)
    {
        string sourceRoot = CliConfiguration.GetSourceCodeRoot();
        string buildOutput = Path.Combine(
            sourceRoot,
            "AppBlueprint.DeveloperCli",
            "bin",
            "Release",
            "net10.0");

        if (!Directory.Exists(buildOutput))
        {
            throw new DirectoryNotFoundException($"Build output not found at: {buildOutput}");
        }

        // Copy all files
        foreach (string file in Directory.GetFiles(buildOutput, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(buildOutput, file);
            string destFile = Path.Combine(installPath, relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(destFile) ?? installPath);
            File.Copy(file, destFile, true);
        }
    }

    private static bool RegisterCli(string installPath)
    {
        if (CliConfiguration.IsWindows)
        {
            return PathHelper.AddToPathWindows(installPath);
        }
        else
        {
            // For macOS/Linux, create a shell alias
            string dllPath = Path.Combine(installPath, "AppBlueprint.DeveloperCli.dll");
            return PathHelper.RegisterShellAlias(CliConfiguration.AliasName, dllPath);
        }
    }

    private static void ShowSuccessMessage()
    {
        AnsiConsole.WriteLine();

        var successPanel = new Panel(
            new Markup(
                $"[green]✅ Installation successful![/]\n\n" +
                $"The '[green]{CliConfiguration.AliasName}[/]' command is now available globally.\n\n" +
                "[yellow]Next steps:[/]\n" +
                (CliConfiguration.IsWindows
                    ? "  1. [dim]Restart your terminal[/] (or open a new terminal)\n"
                    : $"  1. Restart your terminal [dim](or run 'source ~/{Path.GetFileName(GetShellProfilePath() ?? ".zshrc")}')[/]\n") +
                $"  2. Run '[green]{CliConfiguration.AliasName} --help[/]' to see all commands\n" +
                $"  3. Run '[green]{CliConfiguration.AliasName} run[/]' to start development!\n\n" +
                $"[cyan]To uninstall:[/] Run '[green]{CliConfiguration.AliasName} uninstall[/]'"
            ))
        {
            Header = new PanelHeader("[green]Success![/]"),
            Border = BoxBorder.Rounded,
            Padding = new Padding(2, 1)
        };

        AnsiConsole.Write(successPanel);
        AnsiConsole.WriteLine();
    }

    private static string? GetShellProfilePath()
    {
        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (File.Exists(Path.Combine(home, ".zshrc")))
        {
            return Path.Combine(home, ".zshrc");
        }

        if (File.Exists(Path.Combine(home, ".bashrc")))
        {
            return Path.Combine(home, ".bashrc");
        }

        return Path.Combine(home, ".bash_profile");
    }

    public static void ExecuteInteractive()
    {
        AnsiConsole.MarkupLine("[yellow]Installing CLI globally...[/]\n");

        bool force = AnsiConsole.Confirm("Force reinstall (if already installed)?", false);

        AnsiConsole.WriteLine();
        Execute(force);
    }
}
