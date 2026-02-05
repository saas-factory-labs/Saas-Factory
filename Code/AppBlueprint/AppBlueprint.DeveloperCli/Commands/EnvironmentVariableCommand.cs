namespace AppBlueprint.DeveloperCli.Commands;

internal static class EnvironmentVariableCommand
{
    private const string AppBlueprintPrefix = "";
    private const string ScopeOption = "--scope";
    private const string ScopeDescription = "Scope: User, Machine, or Process";

    public static Command Create()
    {
        var command = new Command("env", "Manage environment variables for AppBlueprint");

        // List subcommand
        Command listCommand = CreateListCommand();
        command.AddCommand(listCommand);

        // Get subcommand
        Command getCommand = CreateGetCommand();
        command.AddCommand(getCommand);

        // Set subcommand
        Command setCommand = CreateSetCommand();
        command.AddCommand(setCommand);

        // Delete subcommand
        Command deleteCommand = CreateDeleteCommand();
        command.AddCommand(deleteCommand);

        return command;
    }

    private static Command CreateListCommand()
    {
        var command = new Command("list", "List environment variables");

        var prefixOption = new Option<string>(
            "--prefix",
            description: "Filter variables by prefix",
            getDefaultValue: () => AppBlueprintPrefix);

        var scopeOption = new Option<string>(
            ScopeOption,
            description: ScopeDescription,
            getDefaultValue: () => "User");

        command.AddOption(prefixOption);
        command.AddOption(scopeOption);

        command.SetHandler((prefix, scopeStr) =>
        {
            if (!TryParseScope(scopeStr, out var scope))
            {
                AnsiConsole.MarkupLine($"[red]Invalid scope: {scopeStr}. Use User, Machine, or Process[/]");
                return;
            }

            ListEnvironmentVariables(prefix, scope);
        }, prefixOption, scopeOption);

        return command;
    }

    private static Command CreateGetCommand()
    {
        var command = new Command("get", "Get an environment variable value");

        var nameArgument = new Argument<string>("name", "Environment variable name");
        var scopeOption = new Option<string>(
            ScopeOption,
            description: ScopeDescription,
            getDefaultValue: () => "User");

        command.AddArgument(nameArgument);
        command.AddOption(scopeOption);

        command.SetHandler((name, scopeStr) =>
        {
            if (!TryParseScope(scopeStr, out var scope))
            {
                AnsiConsole.MarkupLine($"[red]Invalid scope: {scopeStr}. Use User, Machine, or Process[/]");
                return;
            }

            GetEnvironmentVariable(name, scope);
        }, nameArgument, scopeOption);

        return command;
    }

    private static Command CreateSetCommand()
    {
        var command = new Command("set", "Set an environment variable");

        var nameArgument = new Argument<string>("name", "Environment variable name");
        var valueArgument = new Argument<string>("value", "Environment variable value");
        var scopeOption = new Option<string>(
            ScopeOption,
            description: ScopeDescription,
            getDefaultValue: () => "User");

        command.AddArgument(nameArgument);
        command.AddArgument(valueArgument);
        command.AddOption(scopeOption);

        command.SetHandler([System.Runtime.Versioning.SupportedOSPlatform("windows")] (name, value, scopeStr) =>
        {
            if (!TryParseScope(scopeStr, out var scope))
            {
                AnsiConsole.MarkupLine($"[red]Invalid scope: {scopeStr}. Use User, Machine, or Process[/]");
                return;
            }

            SetEnvironmentVariable(name, value, scope);
        }, nameArgument, valueArgument, scopeOption);

        return command;
    }

    private static Command CreateDeleteCommand()
    {
        var command = new Command("delete", "Delete an environment variable");

        var nameArgument = new Argument<string>("name", "Environment variable name");
        var scopeOption = new Option<string>(
            ScopeOption,
            description: ScopeDescription,
            getDefaultValue: () => "User");

        command.AddArgument(nameArgument);
        command.AddOption(scopeOption);

        command.SetHandler([System.Runtime.Versioning.SupportedOSPlatform("windows")] (name, scopeStr) =>
        {
            if (!TryParseScope(scopeStr, out EnvironmentVariableTarget scope))
            {
                AnsiConsole.MarkupLine($"[red]Invalid scope: {scopeStr}. Use User, Machine, or Process[/]");
                return;
            }

            DeleteEnvironmentVariable(name, scope);
        }, nameArgument, scopeOption);

        return command;
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public static void ExecuteInteractive()
    {
        AnsiConsole.Write(new FigletText("Environment Variables").Color(Color.Cyan1));
        AnsiConsole.WriteLine();

        var options = new[]
        {
            "List environment variables",
            "Get environment variable",
            "Set environment variable",
            "Delete environment variable",
            "Back to main menu"
        };

        string choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]What would you like to do?[/]")
                .PageSize(10)
                .AddChoices(options));

        switch (choice)
        {
            case "List environment variables":
                ExecuteListInteractive();
                break;
            case "Get environment variable":
                ExecuteGetInteractive();
                break;
            case "Set environment variable":
                ExecuteSetInteractive();
                break;
            case "Delete environment variable":
                ExecuteDeleteInteractive();
                break;
            case "Back to main menu":
                return;
        }
    }

    private static void ExecuteListInteractive()
    {
        var scope = PromptForScope();
        var usePrefix = AnsiConsole.Confirm($"Filter by prefix '{AppBlueprintPrefix}'?", true);
        var prefix = usePrefix ? AppBlueprintPrefix : string.Empty;

        ListEnvironmentVariables(prefix, scope);
    }

    private static void ExecuteGetInteractive()
    {
        var scope = PromptForScope();
        var name = AnsiConsole.Ask<string>("[green]Enter variable name:[/]");

        GetEnvironmentVariable(name, scope);
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static void ExecuteSetInteractive()
    {
        var scope = PromptForScope();

        if (scope == EnvironmentVariableTarget.Machine && !IsAdministrator())
        {
            AnsiConsole.MarkupLine("[red]Setting Machine-level variables requires administrator privileges.[/]");
            AnsiConsole.MarkupLine("[yellow]Please run the CLI as administrator or use User scope.[/]");
            return;
        }

        var name = AnsiConsole.Ask<string>("[green]Enter variable name:[/]");
        var value = AnsiConsole.Ask<string>("[green]Enter variable value:[/]");

        SetEnvironmentVariable(name, value, scope);
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static void ExecuteDeleteInteractive()
    {
        var scope = PromptForScope();

        if (scope == EnvironmentVariableTarget.Machine && !IsAdministrator())
        {
            AnsiConsole.MarkupLine("[red]Deleting Machine-level variables requires administrator privileges.[/]");
            AnsiConsole.MarkupLine("[yellow]Please run the CLI as administrator or use User scope.[/]");
            return;
        }

        var name = AnsiConsole.Ask<string>("[green]Enter variable name to delete:[/]");

        DeleteEnvironmentVariable(name, scope);
    }

    private static EnvironmentVariableTarget PromptForScope()
    {
        var scopeChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Select scope:[/]")
                .AddChoices("User", "Machine", "Process"));

        return scopeChoice switch
        {
            "User" => EnvironmentVariableTarget.User,
            "Machine" => EnvironmentVariableTarget.Machine,
            "Process" => EnvironmentVariableTarget.Process,
            _ => EnvironmentVariableTarget.User
        };
    }

    private static void ListEnvironmentVariables(string prefix, EnvironmentVariableTarget scope)
    {
#pragma warning disable CA1031 // Generic catch acceptable for user-facing CLI error handling
        try
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[cyan]Listing environment variables (Scope: {scope})[/]");

            if (!string.IsNullOrEmpty(prefix))
            {
                AnsiConsole.MarkupLine($"[dim]Filter: Variables starting with '{prefix}'[/]");
            }

            AnsiConsole.WriteLine();

            var variables = Environment.GetEnvironmentVariables(scope);
            var filteredVars = new SortedDictionary<string, string>();

            foreach (System.Collections.DictionaryEntry entry in variables)
            {
                string key = entry.Key?.ToString() ?? string.Empty;
                string value = entry.Value?.ToString() ?? string.Empty;

                if (string.IsNullOrEmpty(prefix) || key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    filteredVars[key] = value;
                }
            }

            if (filteredVars.Count is 0)
            {
                AnsiConsole.MarkupLine("[yellow]No environment variables found.[/]");
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn(new TableColumn("[yellow]Name[/]").Width(40))
                .AddColumn(new TableColumn("[yellow]Value[/]"));

            foreach (var kvp in filteredVars)
            {
                // Truncate long values for display
                var displayValue = kvp.Value.Length > 80
                    ? kvp.Value[..77] + "..."
                    : kvp.Value;

                table.AddRow(
                    $"[green]{kvp.Key}[/]",
                    $"[dim]{displayValue.EscapeMarkup()}[/]");
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[green]Found {filteredVars.Count} variable(s)[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error listing environment variables:[/] {ex.Message}");
        }
#pragma warning restore CA1031
    }

    private static void GetEnvironmentVariable(string name, EnvironmentVariableTarget scope)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

#pragma warning disable CA1031 // Generic catch acceptable for user-facing CLI error handling
        try
        {
            var value = Environment.GetEnvironmentVariable(name, scope);

            if (value is null)
            {
                AnsiConsole.MarkupLine($"[yellow]Environment variable '{name}' not found in {scope} scope.[/]");
                return;
            }

            AnsiConsole.WriteLine();

            var panel = new Panel(new Markup($"[green]{value.EscapeMarkup()}[/]"))
            {
                Header = new PanelHeader($"[cyan]{name}[/] ({scope})"),
                Border = BoxBorder.Rounded,
                Padding = new Padding(2, 1)
            };

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error getting environment variable:[/] {ex.Message}");
        }
#pragma warning restore CA1031
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static void SetEnvironmentVariable(string name, string value, EnvironmentVariableTarget scope)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(value);

#pragma warning disable CA1031 // Generic catch acceptable for user-facing CLI error handling
        try
        {
            if (scope == EnvironmentVariableTarget.Machine && !IsAdministrator())
            {
                AnsiConsole.MarkupLine("[red]Setting Machine-level variables requires administrator privileges.[/]");
                AnsiConsole.MarkupLine("[yellow]Please run the CLI as administrator or use --scope User[/]");
                return;
            }

            // Show what we're about to do
            var existingValue = Environment.GetEnvironmentVariable(name, scope);

            if (existingValue is not null)
            {
                AnsiConsole.MarkupLine($"[yellow]Variable '{name}' already exists with value:[/]");
                AnsiConsole.MarkupLine($"[dim]{existingValue.EscapeMarkup()}[/]");
                AnsiConsole.WriteLine();

                if (!AnsiConsole.Confirm("Overwrite?", false))
                {
                    AnsiConsole.MarkupLine("[yellow]Operation cancelled.[/]");
                    return;
                }
            }

            Environment.SetEnvironmentVariable(name, value, scope);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[green]✓ Environment variable '{name}' set successfully![/]");
            AnsiConsole.MarkupLine($"[dim]Scope: {scope}[/]");
            AnsiConsole.MarkupLine($"[dim]Value: {value.EscapeMarkup()}[/]");
            AnsiConsole.WriteLine();

            if (scope is not EnvironmentVariableTarget.Process)
            {
                AnsiConsole.MarkupLine("[yellow]Note: You may need to restart applications or terminals for the change to take effect.[/]");
            }
        }
        catch (System.Security.SecurityException)
        {
            AnsiConsole.MarkupLine("[red]Access denied. Administrator privileges required for Machine scope.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error setting environment variable:[/] {ex.Message}");
        }
#pragma warning restore CA1031
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static void DeleteEnvironmentVariable(string name, EnvironmentVariableTarget scope)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

#pragma warning disable CA1031 // Generic catch acceptable for user-facing CLI error handling
        try
        {
            if (scope == EnvironmentVariableTarget.Machine && !IsAdministrator())
            {
                AnsiConsole.MarkupLine("[red]Deleting Machine-level variables requires administrator privileges.[/]");
                AnsiConsole.MarkupLine("[yellow]Please run the CLI as administrator or use --scope User[/]");
                return;
            }

            var existingValue = Environment.GetEnvironmentVariable(name, scope);

            if (existingValue is null)
            {
                AnsiConsole.MarkupLine($"[yellow]Environment variable '{name}' not found in {scope} scope.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[yellow]Variable '{name}' current value:[/]");
            AnsiConsole.MarkupLine($"[dim]{existingValue.EscapeMarkup()}[/]");
            AnsiConsole.WriteLine();

            if (!AnsiConsole.Confirm($"Are you sure you want to delete this variable from {scope} scope?", false))
            {
                AnsiConsole.MarkupLine("[yellow]Operation cancelled.[/]");
                return;
            }

            Environment.SetEnvironmentVariable(name, null, scope);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[green]✓ Environment variable '{name}' deleted successfully![/]");
            AnsiConsole.MarkupLine($"[dim]Scope: {scope}[/]");
            AnsiConsole.WriteLine();

            if (scope is not EnvironmentVariableTarget.Process)
            {
                AnsiConsole.MarkupLine("[yellow]Note: You may need to restart applications or terminals for the change to take effect.[/]");
            }
        }
        catch (System.Security.SecurityException)
        {
            AnsiConsole.MarkupLine("[red]Access denied. Administrator privileges required for Machine scope.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error deleting environment variable:[/] {ex.Message}");
        }
#pragma warning restore CA1031
    }

    private static bool TryParseScope(string scopeStr, out EnvironmentVariableTarget scope)
    {
        return Enum.TryParse(scopeStr, ignoreCase: true, out scope);
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static bool IsAdministrator()
    {
#pragma warning disable CA1031 // Generic catch acceptable for platform-specific functionality fallback
        try
        {
            using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
#pragma warning restore CA1031
    }
}
