namespace AppBlueprint.DeveloperCli.Commands;

internal static class CommandFactory
{
    public static RootCommand CreateRootCommand()
    {
        var rootCommand = new RootCommand
        {
            Description =
                "SaaS Factory - AppBlueprint Developer Cli - A tool to streamline SaaS application development."
        };

        rootCommand.AddCommand(RunCommand.Create());
        rootCommand.AddCommand(InstallCommand.Create());
        rootCommand.AddCommand(UninstallCommand.Create());
        rootCommand.AddCommand(SolutionCommand.Create());
        rootCommand.AddCommand(ProjectCommand.Create());
        rootCommand.AddCommand(ItemCommand.Create());
        rootCommand.AddCommand(DatabaseCommand.Create());
        rootCommand.AddCommand(GitHubCommand.Create());
        rootCommand.AddCommand(GithubActionWorkflowCommand.Create());
        rootCommand.AddCommand(RouteCommand.Create());
        rootCommand.AddCommand(JwtTokenCommand.Create());
        rootCommand.AddCommand(EnvironmentVariableCommand.Create());
        rootCommand.AddCommand(EnvironmentInfoCommand.Create());
        rootCommand.AddCommand(TestCommand.Create());

        return rootCommand;
    }
}
