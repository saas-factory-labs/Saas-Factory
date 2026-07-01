using AppBlueprint.CliKit.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.DeveloperCli.Commands;

internal sealed class CommandFactory
{
    private readonly IEnumerable<ICliCommand> _commands;

    public CommandFactory(IEnumerable<ICliCommand> commands)
    {
        ArgumentNullException.ThrowIfNull(commands);
        _commands = commands;
    }

    public RootCommand CreateRootCommand()
    {
        var rootCommand = new RootCommand(
            "SaaS Factory - AppBlueprint Developer CLI - A tool to streamline SaaS application development.");

        foreach (var command in _commands)
        {
            rootCommand.Add(command.Build());
        }

        return rootCommand;
    }
}

internal static class DeveloperCliServiceCollectionExtensions
{
    public static IServiceCollection AddDeveloperCliCommands(this IServiceCollection services)
    {
        services.AddTransient<CommandFactory>();
        services.AddTransient<ICliCommand>(_ => new StaticCliCommand(RunCommand.Create));
        services.AddTransient<ICliCommand>(_ => new StaticCliCommand(InstallCommand.Create));
        services.AddTransient<ICliCommand>(_ => new StaticCliCommand(UninstallCommand.Create));
        services.AddTransient<ICliCommand>(_ => new StaticCliCommand(SolutionCommand.Create));
        services.AddTransient<ICliCommand>(_ => new StaticCliCommand(ProjectCommand.Create));
        services.AddTransient<ICliCommand>(_ => new StaticCliCommand(ItemCommand.Create));
        services.AddTransient<ICliCommand>(_ => new StaticCliCommand(DatabaseCommand.Create));
        services.AddTransient<ICliCommand>(_ => new StaticCliCommand(SeedCommand.Create));
        services.AddTransient<ICliCommand>(_ => new StaticCliCommand(GitHubCommand.Create));
        services.AddTransient<ICliCommand>(_ => new StaticCliCommand(GithubActionWorkflowCommand.Create));
        services.AddTransient<ICliCommand>(_ => new StaticCliCommand(RouteCommand.Create));
        services.AddTransient<ICliCommand>(_ => new StaticCliCommand(JwtTokenCommand.Create));
        services.AddTransient<ICliCommand>(_ => new StaticCliCommand(EnvironmentVariableCommand.Create));
        services.AddTransient<ICliCommand>(_ => new StaticCliCommand(EnvironmentInfoCommand.Create));
        services.AddTransient<ICliCommand>(_ => new StaticCliCommand(TestCommand.Create));
        services.AddTransient<ICliCommand>(_ => new StaticCliCommand(InfraCommand.Create));

        return services;
    }
}

internal sealed class StaticCliCommand : ICliCommand
{
    private readonly Func<Command> _factory;

    public StaticCliCommand(Func<Command> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _factory = factory;
    }

    public Command Build() => _factory();
}
