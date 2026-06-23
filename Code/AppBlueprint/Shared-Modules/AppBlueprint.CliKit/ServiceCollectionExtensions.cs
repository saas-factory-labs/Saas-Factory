using AppBlueprint.CliKit.Ci;
using AppBlueprint.CliKit.Commands;
using AppBlueprint.CliKit.Configuration;
using AppBlueprint.CliKit.Console;
using AppBlueprint.CliKit.Processes;
using AppBlueprint.CliKit.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.CliKit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCliKit(this IServiceCollection services)
    {
        services.AddSingleton<ICliConsole, SpectreCliConsole>();
        services.AddSingleton<IGitHubActionsOutputWriter, GitHubActionsOutputWriter>();
        services.AddSingleton<ICliProcessRunner, CliProcessRunner>();
        services.AddSingleton<IDotNetSdkPreflight, DotNetSdkPreflight>();
        services.AddSingleton<CliCommandApplication>();
        services.AddSingleton(typeof(LocalJsonConfigLoader<>));

        return services;
    }
}
