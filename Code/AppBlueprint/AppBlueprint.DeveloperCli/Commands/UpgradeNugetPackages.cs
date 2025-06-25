using AppBlueprint.DeveloperCli.Utilities;

namespace AppBlueprint.DeveloperCli.Commands;

internal static class UpgradeNugetPacckagesCommand
{
    public static Command Create()
    {
        // implementer et tool i min developer cli til at opdatere nuget pakker i den centrale pacakges fil hvor den så rent faktisk tjekker version numre og om pakken eksisterer og så ændrer filen først derefter
        var connectionStringOption = new Option<string>("--connection-string", "The connection string for the database.") { IsRequired = true };

        var command = new Command("upgrade-nuget-packages",
            "Upgrade nuget packages for the app solution in the Directory.Packages.props file..")
        {
            connectionStringOption
        };

        command.SetHandler((string connectionString) =>
        {
            AnsiConsole.MarkupLine($"[green]Migrating database with connection string: {connectionString}...[/]");

            CliUtilities.RunShellCommand($"dotnet ef migrations add 'Migration-{Guid.NewGuid().ToString()}",
                "Database migration added but not yet committed to database!", "Failed to add database migration.");

            CliUtilities.RunShellCommand($"dotnet ef database update --connection \"{connectionString}\"",
                "Database migrated successfully!", "Failed to migrate database.");
        }, connectionStringOption);

        return command;
    }

    public static void ExecuteInteractive()
    {
        string connectionString = AnsiConsole.Ask<string>("[green]Enter the database connection string:[/]");
        CliUtilities.RunShellCommand($"dotnet ef database update --connection \"{connectionString}\"",
            "Database migrated successfully!", "Failed to migrate database.");
    }
}
