using AppBlueprint.Infrastructure.Core;
using AppBlueprint.Infrastructure.Core.Abstractions;
using AppBlueprint.Infrastructure.Core.Configuration;
using AppBlueprint.Infrastructure.Core.Providers.Cloudflare;
using AppBlueprint.Infrastructure.Core.Providers.Cloudflare.Wrangler;
using Pulumi.Automation;

namespace AppBlueprint.DeveloperCli.Commands;

/// <summary>
/// Drives <c>infra up</c>: loads the app's <c>infra.json</c>, bridges any existing <c>wrangler.toml</c>,
/// and provisions the stack through the Pulumi Automation API, streaming engine output to the console.
/// </summary>
internal static class InfraUpRunner
{
    public static async Task RunAsync(string environment, bool dryRun = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(environment);

        string appDir = Directory.GetCurrentDirectory();

        // 1. Load + validate infra.json (throws a clear error if missing or invalid).
        AppInfrastructureConfig config = AppInfrastructureConfigLoader.LoadFromDirectory(appDir);
        AnsiConsole.MarkupLine(
            $"[green]Loaded[/] [cyan]{AppInfrastructureConfigLoader.DefaultFileName}[/] for [bold]{Markup.Escape(config.AppId)}[/] " +
            $"(provider: {config.Provider}, env: {Markup.Escape(environment)}{(dryRun ? ", dry-run" : string.Empty)}).");

        // 2. Bridge an existing wrangler.toml so legacy Cloudflare config is respected.
        BridgeWrangler(appDir);

        // 3. Build the inline Pulumi program from the provider-agnostic factory.
        var program = PulumiFn.Create(() =>
        {
            var factory = new AppStackFactory(new IInfrastructureProvider[] { new CloudflareProvider() });
            AppDeploymentOutputs outputs = factory.Deploy(config);
            return outputs.ToExportDictionary();
        });

        var args = new InlineProgramArgs(config.AppId, environment, program);

        AnsiConsole.MarkupLine(
            $"[grey]Selecting stack[/] [cyan]{Markup.Escape(config.AppId)}/{Markup.Escape(environment)}[/]...");
        WorkspaceStack stack = await LocalWorkspace.CreateOrSelectStackAsync(args);

        // 4a. Dry run: preview the changes (pulumi preview) without applying anything.
        if (dryRun)
        {
            var previewOptions = new PreviewOptions
            {
                OnStandardOutput = Console.WriteLine,
                OnStandardError = Console.Error.WriteLine
            };

            PreviewResult preview = await stack.PreviewAsync(previewOptions, CancellationToken.None);

            AnsiConsole.MarkupLine("[green]Dry run (preview) complete — no changes were applied.[/]");
            PrintChangeSummary(preview);
            return;
        }

        // 4b. Deploy, streaming the Pulumi engine's stdout/stderr straight to the console.
        var upOptions = new UpOptions
        {
            OnStandardOutput = Console.WriteLine,
            OnStandardError = Console.Error.WriteLine
        };

        UpResult result = await stack.UpAsync(upOptions, CancellationToken.None);

        AnsiConsole.MarkupLine("[green]Deployment complete.[/]");
        PrintOutputs(result);
    }

    private static void BridgeWrangler(string appDir)
    {
        WranglerConfig? wrangler = WranglerConfigParser.TryLoadFromDirectory(appDir);
        if (wrangler is null)
        {
            return;
        }

        AnsiConsole.MarkupLine(
            $"[grey]Bridged {WranglerConfigParser.DefaultFileName}:[/] name={Markup.Escape(wrangler.Name ?? "-")}, " +
            $"compat={Markup.Escape(wrangler.CompatibilityDate ?? "-")}, r2={wrangler.R2Buckets.Count}, " +
            $"hyperdrive={wrangler.HyperdriveBindings.Count}, vars={wrangler.Vars.Count}");

        // Supply the Cloudflare account id from wrangler.toml when it is not already configured.
        if (!string.IsNullOrWhiteSpace(wrangler.AccountId)
            && string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CLOUDFLARE_ACCOUNT_ID")))
        {
            Environment.SetEnvironmentVariable("CLOUDFLARE_ACCOUNT_ID", wrangler.AccountId);
            AnsiConsole.MarkupLine("[grey]Using Cloudflare account id from wrangler.toml.[/]");
        }
    }

    private static void PrintChangeSummary(PreviewResult preview)
    {
        if (preview.ChangeSummary.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey]No changes planned.[/]");
            return;
        }

        AnsiConsole.MarkupLine("[bold]Planned changes:[/]");
        foreach (KeyValuePair<Pulumi.Automation.OperationType, int> change in preview.ChangeSummary)
        {
            AnsiConsole.MarkupLine($"  [cyan]{Markup.Escape(change.Key.ToString())}[/]: {change.Value}");
        }
    }

    private static void PrintOutputs(UpResult result)
    {
        if (result.Outputs.Count == 0)
        {
            return;
        }

        AnsiConsole.MarkupLine("[bold]Outputs:[/]");
        foreach (KeyValuePair<string, OutputValue> output in result.Outputs)
        {
            string value = output.Value.IsSecret ? "[secret]" : output.Value.Value?.ToString() ?? string.Empty;
            AnsiConsole.MarkupLine($"  [cyan]{Markup.Escape(output.Key)}[/] = {Markup.Escape(value)}");
        }
    }
}
