using AppBlueprint.CliKit;
using AppBlueprint.CliKit.Commands;
using AppBlueprint.DeveloperCli.Commands;
using AppBlueprint.DeveloperCli.Menus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppBlueprint.DeveloperCli;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddCliKit();
        builder.Services.AddDeveloperCliCommands();

        using var host = builder.Build();

        if (args.Length is 0)
        {
            MainMenu.Show();
            return 0;
        }

        var commandFactory = host.Services.GetRequiredService<CommandFactory>();
        var rootCommand = commandFactory.CreateRootCommand();
        var app = host.Services.GetRequiredService<CliCommandApplication>();

        return await app.InvokeAsync(rootCommand, args);
    }
}
