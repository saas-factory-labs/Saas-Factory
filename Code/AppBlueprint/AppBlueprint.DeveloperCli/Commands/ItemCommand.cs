using AppBlueprint.DeveloperCli.Utilities;

namespace AppBlueprint.DeveloperCli.Commands;

internal static class ItemCommand
{
    public static Command Create()
    {
        var command = new Command("create-item", "Create a new item in the SaaS app solution.")
        {
            new Option<string>("--name", "The name of the item."),
            new Option<string>("--template", "The item template to use."),
            new Option<string>("--location", "The location to create the item.")
        };

        command.Handler = CommandHandler.Create<string, string, string>(
            (name, template, location) =>
            {
                AnsiConsole.MarkupLine($"[green]Creating item: {name} at {location} using template {template}...[/]");
                CliUtilities.RunShellCommand($"dotnet new {template} -o {location}/{name}",
                    "Item created successfully!", "Failed to create item.");
            });

        return command;
    }

    public static void ExecuteInteractive()
    {
        string name = AnsiConsole.Ask<string>("[green]Enter the item name:[/]");
        string template = AnsiConsole.Ask<string>("[green]Enter the template to use (e.g., api-controller, dto):[/]");
        string location = AnsiConsole.Ask<string>("[green]Enter the location for the item:[/]");
        CliUtilities.RunShellCommand($"dotnet new {template} -o {location}/{name}", "Item created successfully!",
            "Failed to create item.");
    }
}
