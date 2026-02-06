using AppBlueprint.DeveloperCli.Utilities;

namespace AppBlueprint.DeveloperCli.Commands;

/// <summary>
/// Command to uninstall the CLI and remove the global 'saas' alias
/// </summary>
internal static class UninstallCommand
{
    public static Command Create()
    {
        var command = new Command("uninstall", $"Uninstall the '{CliConfiguration.AliasName}' CLI and remove global access");

        command.SetHandler(() =>
        {
            Execute();
        });

        return command;
    }

    private static void Execute()
    {
        AnsiConsole.Write(new FigletText("Uninstall").Color(Color.Red));
        AnsiConsole.MarkupLine($"[yellow]Uninstalling '{CliConfiguration.AliasName}' CLI...[/]\n");

        // Check if installed
        if (!IsInstalled())
        {
            AnsiConsole.MarkupLine($"[yellow]The CLI is not currently installed globally.[/]");
            AnsiConsole.MarkupLine($"[dim]Nothing to uninstall.[/]\n");
            return;
        }

        // Confirm uninstallation
        if (!AnsiConsole.Confirm($"\n[red]Remove '{CliConfiguration.AliasName}' CLI from your system?[/]", false))
        {
            AnsiConsole.MarkupLine("[yellow]Uninstallation cancelled.[/]");
            return;
        }

        AnsiConsole.WriteLine();

        // Perform uninstallation
        AnsiConsole.Status()
            .Start("Uninstalling...", ctx =>
            {
                // Step 1: Unregister from PATH or remove alias
                ctx.Status("Removing CLI registration...");
                ctx.Spinner(Spinner.Known.Dots);

                if (!UnregisterCli())
                {
                    throw new Exception("Failed to unregister CLI");
                }

                AnsiConsole.MarkupLine("[green]✓ CLI unregistered[/]");

                // Step 2: Delete installation directory
                ctx.Status("Removing installation files...");

                string installPath = CliConfiguration.GetGlobalInstallPath();
                if (Directory.Exists(installPath))
                {
                    try
                    {
                        Directory.Delete(installPath, true);
                        AnsiConsole.MarkupLine($"[green]✓ Removed: {installPath}[/]");
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"[yellow]⚠ Could not delete directory: {ex.Message}[/]");
                        AnsiConsole.MarkupLine($"[dim]You may need to manually delete: {installPath}[/]");
                    }
                }
            });

        // Show success message
        ShowSuccessMessage();
    }

    private static bool IsInstalled()
    {
        if (CliConfiguration.IsWindows)
        {
            string installPath = CliConfiguration.GetGlobalInstallPath();
            return PathHelper.IsInPathWindows(installPath);
        }
        else
        {
            string? shellProfilePath = GetShellProfilePath();
            if (shellProfilePath == null || !File.Exists(shellProfilePath))
            {
                return false;
            }

            string content = File.ReadAllText(shellProfilePath);
            return content.Contains($"alias {CliConfiguration.AliasName}=", StringComparison.Ordinal);
        }
    }

    private static bool UnregisterCli()
    {
        if (CliConfiguration.IsWindows)
        {
            string installPath = CliConfiguration.GetGlobalInstallPath();
            return PathHelper.RemoveFromPathWindows(installPath);
        }
        else
        {
            return PathHelper.UnregisterShellAlias(CliConfiguration.AliasName);
        }
    }

    private static void ShowSuccessMessage()
    {
        AnsiConsole.WriteLine();

        var successPanel = new Panel(
            new Markup(
                "[green]✅ Uninstallation complete![/]\n\n" +
                $"The '[red]{CliConfiguration.AliasName}[/]' command has been removed.\n\n" +
                "[yellow]Note:[/]\n" +
                (CliConfiguration.IsWindows
                    ? "  • Restart your terminal for changes to take effect\n"
                    : "  • Restart your terminal or reload your shell profile\n") +
                $"  • You can still run the CLI from the project directory:\n" +
                $"    [dim]cd Code/AppBlueprint/AppBlueprint.DeveloperCli[/]\n" +
                $"    [dim]dotnet run[/]\n\n" +
                $"[cyan]To reinstall:[/] Run '[green]dotnet run -- install[/]' from the CLI project"
            ))
        {
            Header = new PanelHeader("[green]Uninstalled[/]"),
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
        AnsiConsole.MarkupLine("[yellow]Uninstalling CLI...[/]\n");
        Execute();
    }
}
