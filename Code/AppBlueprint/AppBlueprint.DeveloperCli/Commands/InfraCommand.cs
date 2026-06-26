namespace AppBlueprint.DeveloperCli.Commands;

/// <summary>
/// The <c>infra</c> command group: schema-driven, provider-agnostic infrastructure for AppBlueprint
/// SaaS apps. Subcommands are scaffolded here; their implementations are added in later steps
/// (<c>generate-schema</c> via NJsonSchema, <c>up</c> via the Pulumi Automation API).
/// </summary>
internal static class InfraCommand
{
    public static Command Create()
    {
        var command = new Command(
            "infra",
            "Schema-driven infrastructure for AppBlueprint apps (Cloudflare primary).");

        command.Add(CreateGenerateSchemaCommand());
        command.Add(CreateUpCommand());

        return command;
    }

    private static Command CreateGenerateSchemaCommand()
    {
        var outputOption = new Option<string>("--output")
        {
            Description =
                $"Path to write the schema (default: {InfraSchemaGenerator.DefaultSchemaFileName} in the current directory).",
            Required = false
        };

        var command = new Command(
            "generate-schema",
            "Generate app-infra-schema.json from the AppInfrastructureConfig contract.")
        {
            outputOption
        };

        command.SetHandler((string? output) =>
        {
            string path = string.IsNullOrWhiteSpace(output)
                ? Path.Combine(Directory.GetCurrentDirectory(), InfraSchemaGenerator.DefaultSchemaFileName)
                : output;

            string json = InfraSchemaGenerator.GenerateJson();
            File.WriteAllText(path, json);

            AnsiConsole.MarkupLine($"[green]Wrote infra schema to[/] [cyan]{Markup.Escape(path)}[/]");
        }, outputOption);

        return command;
    }

    private static Command CreateUpCommand()
    {
        var envOption = new Option<string>("--env")
        {
            Description = "Target environment to deploy (e.g. dev, prod).",
            Required = true
        };

        var dryRunOption = new Option<bool>("--dry-run")
        {
            Description = "Preview the changes without applying them (pulumi preview)."
        };

        var command = new Command(
            "up",
            "Provision infrastructure for the SaaS app in the current directory.")
        {
            envOption,
            dryRunOption
        };

        command.SetHandler(async (string env, bool dryRun) =>
        {
#pragma warning disable CA1031 // Surface any deploy failure as a clean CLI error rather than a stack trace.
            try
            {
                await InfraUpRunner.RunAsync(env, dryRun);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]infra up failed:[/] {Markup.Escape(ex.Message)}");
                // Hard-exit non-zero: the command pipeline would otherwise return 0 for a handled error,
                // which would hide deploy failures from CI.
                Environment.Exit(1);
            }
#pragma warning restore CA1031
        }, envOption, dryRunOption);

        return command;
    }

    /// <summary>Interactive entry point (dashboard): generates the schema into the current directory.</summary>
    public static void ExecuteGenerateSchemaInteractive()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), InfraSchemaGenerator.DefaultSchemaFileName);
        File.WriteAllText(path, InfraSchemaGenerator.GenerateJson());
        AnsiConsole.MarkupLine($"[green]Wrote infra schema to[/] [cyan]{Markup.Escape(path)}[/]");
    }

    /// <summary>
    /// Interactive entry point (dashboard): prompts for an environment and deploys. A real deploy
    /// requires confirmation; a dry run (preview) is safe and runs without confirmation.
    /// </summary>
    public static async Task ExecuteUpInteractive(bool dryRun = false)
    {
        string env = await AnsiConsole.AskAsync("[green]Target environment (e.g. dev, prod):[/]", "dev");

        if (!dryRun &&
            !await AnsiConsole.ConfirmAsync($"[yellow]Deploy infrastructure to '{Markup.Escape(env)}'?[/]", false))
        {
            AnsiConsole.MarkupLine("[grey]Cancelled.[/]");
            return;
        }

        await InfraUpRunner.RunAsync(env, dryRun);
    }
}
