using AppBlueprint.DeveloperCli.Commands.DataSeeding;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.B2B;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.DeveloperCli.Commands;

internal static class SeedCommand
{
    private const string DatabaseConnectionVariable = "DATABASE_CONNECTIONSTRING";

    public static Command Create()
    {
        var command = new Command("seed-database", "Seed the database with test/development data");
        command.SetHandler(async () =>
        {
            await ExecuteAsync(CancellationToken.None);
        });
        return command;
    }

    public static async Task ExecuteInteractive()
    {
        AnsiConsole.MarkupLine("[bold gold1]Seed Database[/]");

        string? connectionString =
            Environment.GetEnvironmentVariable(DatabaseConnectionVariable) ??
            Environment.GetEnvironmentVariable(DatabaseConnectionVariable, EnvironmentVariableTarget.User);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            AnsiConsole.MarkupLine("[lightsalmon1]DATABASE_CONNECTIONSTRING is not set.[/]");
            connectionString = await AnsiConsole.AskAsync<string>("[gold1]Enter PostgreSQL connection string:[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[grey62]Using stored DATABASE_CONNECTIONSTRING.[/]");
        }

        bool confirmed = await AnsiConsole.ConfirmAsync(
            "[gold1]This will TRUNCATE all tables and re-seed. Continue?[/]",
            defaultValue: false);

        if (!confirmed) return;

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("cyan1"))
            .StartAsync("[gold1]Seeding database...[/]", async _ =>
            {
                await RunSeedAsync(connectionString, CancellationToken.None);
            });
    }

    private static async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        string? connectionString =
            Environment.GetEnvironmentVariable(DatabaseConnectionVariable) ??
            Environment.GetEnvironmentVariable(DatabaseConnectionVariable, EnvironmentVariableTarget.User);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            AnsiConsole.MarkupLine("[lightsalmon1]DATABASE_CONNECTIONSTRING environment variable is not set.[/]");
            return;
        }

        await RunSeedAsync(connectionString, cancellationToken);
    }

    private static async Task RunSeedAsync(string connectionString, CancellationToken cancellationToken)
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        builder.Services.AddDbContext<B2BDbContext>(options =>
            options.UseNpgsql(connectionString));
        builder.Services.AddScoped<DataSeeder>();

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        using IHost host = builder.Build();
        using IServiceScope scope = host.Services.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await seeder.SeedDatabaseAsync(cancellationToken);

        AnsiConsole.MarkupLine("[cyan1]Database seeded successfully.[/]");
    }
}
