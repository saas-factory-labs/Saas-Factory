using AppBlueprint.DeveloperCli.Utilities;

namespace AppBlueprint.DeveloperCli.Commands;

internal static class DatabaseCommand
{
    public static Command Create()
    {
        var command = new Command("migrate-database", "Migrate the database for the SaaS app solution.");

        command.SetHandler(() =>
        {
            string command = @"$env:PATH += "";$env:USERPROFILE\.dotnet\tools""";
            CliUtilities.RunShellCommand(command, "Success", "Error");

            AnsiConsole.MarkupLine("[yellow]🟡 Entering the database migration handler...[/]");

            // Get connection string from env variable or prompt the user
            string connectionString =
                Environment.GetEnvironmentVariable("APPBLUEPRINT_DATABASE_CONNECTIONSTRING",
                    EnvironmentVariableTarget.User)
                ?? AnsiConsole.Ask<string>("[green]Enter the database connection string:[/]");
            AnsiConsole.MarkupLine($"[gray]Using connection string: {connectionString}[/]");

            // Both EF project and startup project paths
            string efProject =
                @"C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure";
            string startupProject =
                @"C:\Development\Development-Projects\SaaS-Factory\Code\AppBlueprint\Shared-Modules\AppBlueprint.Infrastructure";

            // List migrations with explicit context and startup project
            string listCommand =
                $"dotnet ef migrations list --project \"{efProject}\" --startup-project \"{startupProject}\" --context ApplicationDbContext";
            AnsiConsole.MarkupLine($"[yellow]🛠 Running command: {listCommand}[/]");
            bool listResult = CliUtilities.RunShellCommand(listCommand,
                "Current migrations listed.", "Failed to list migrations.", efProject);
            if (!listResult)
            {
                AnsiConsole.MarkupLine(
                    "[red]❌ Error listing migrations. Available DbContexts will be listed below:[/]");
                CliUtilities.RunShellCommand(
                    $"dotnet ef dbcontext list --project \"{efProject}\" --startup-project \"{startupProject}\"",
                    "Available DbContexts listed above.", "Failed to list DbContexts.", efProject);
                return;
            }

            // Ask user for migration name
            string migrationName = AnsiConsole.Ask<string>("[green]Enter the migration name:[/]");
            if (string.IsNullOrWhiteSpace(migrationName))
            {
                AnsiConsole.MarkupLine("[red]❌ Migration name cannot be empty. Exiting.[/]");
                return;
            }

            // Add migration with explicit context and startup project
            string addCommand =
                $"dotnet ef migrations add {migrationName} --project \"{efProject}\" --startup-project \"{startupProject}\" --context ApplicationDbContext";
            AnsiConsole.MarkupLine($"[yellow]🛠 Running command: {addCommand}[/]");
            bool addResult = CliUtilities.RunShellCommand(addCommand,
                "Database migration added.", "Failed to add migration.", efProject);
            if (!addResult)
            {
                AnsiConsole.MarkupLine("[red]❌ Error adding migration.[/]");
                return;
            }

            if (AnsiConsole.Confirm("Apply migration and update database?"))
            {
                string updateCommand =
                    $"dotnet ef database update --project \"{efProject}\" --startup-project \"{startupProject}\" --context ApplicationDbContext --connection \"{connectionString}\"";
                AnsiConsole.MarkupLine($"[yellow]🛠 Running command: {updateCommand}[/]");
                bool updateResult = CliUtilities.RunShellCommand(updateCommand,
                    "Database updated successfully!", "Failed to update database.", efProject);
                if (!updateResult) AnsiConsole.MarkupLine("[red]❌ Error updating database.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]⚠️ Migration was created but not applied.[/]");
            }

            AnsiConsole.MarkupLine("[green]🎉 Migration process completed![/]");
        });

        return command;
    }
}
