using AppBlueprint.DeveloperCli.Utilities;

namespace AppBlueprint.DeveloperCli.Commands;

internal static class RouteCommand
{
    public static Command Create()
    {
        var routeCommand = new Command("route", "Inspect application routing");

        var listCommand = new Command("list", "List all route endpoints from the running API");
        
        var urlOption = new Option<string>(
            "--url", 
            () => "https://localhost:7001", 
            "Base URL of the API to scan for routes");
        
        listCommand.AddOption(urlOption);
        
        listCommand.SetHandler(async (string baseUrl) =>
        {
            AnsiConsole.MarkupLine($"[blue]Scanning routes from: {baseUrl}[/]");
            AnsiConsole.MarkupLine("[gray]Fetching routes from /dev/routes endpoint...[/]");
            
            var endpoints = await RouteScanner.GetAllRoutesAsync(baseUrl);
            
            if (endpoints.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No routes found.[/]");
                return;
            }
            
            var table = new Table()
                .AddColumn("[green]Method[/]")
                .AddColumn("[blue]Path[/]")
                .AddColumn("[yellow]Handler[/]");

            foreach (var route in endpoints.OrderBy(r => r.Path).ThenBy(r => r.Method))
            {
                table.AddRow(
                    $"[bold {GetMethodColor(route.Method)}]{route.Method}[/]", 
                    route.Path, 
                    route.Controller);
            }

            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine($"[green]Found {endpoints.Count} route(s)[/]");
            
        }, urlOption);

        routeCommand.AddCommand(listCommand);

        return routeCommand;
    }
    
    private static string GetMethodColor(string method)
    {
        return method.ToUpperInvariant() switch
        {
            "GET" => "green",
            "POST" => "blue",
            "PUT" => "yellow",
            "DELETE" => "red",
            "PATCH" => "purple",
            _ => "gray"
        };
    }

    public static void ExecuteInteractive()
    {
        string baseUrl = AnsiConsole.Ask<string>("[green]Enter the base URL of your API:[/]", "https://localhost:7001");
        
        AnsiConsole.MarkupLine($"[blue]Scanning routes from: {baseUrl}[/]");
        
        var endpoints = RouteScanner.GetAllRoutes(baseUrl);
        
        if (endpoints.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No routes found.[/]");
            return;
        }
        
        var table = new Table()
            .AddColumn("[green]Method[/]")
            .AddColumn("[blue]Path[/]")
            .AddColumn("[yellow]Handler[/]");

        foreach (var route in endpoints.OrderBy(r => r.Path).ThenBy(r => r.Method))
        {
            table.AddRow(
                $"[bold {GetMethodColor(route.Method)}]{route.Method}[/]", 
                route.Path, 
                route.Controller);
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"[green]Found {endpoints.Count} route(s)[/]");
    }
}

