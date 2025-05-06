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

        rootCommand.AddCommand(SolutionCommand.Create());
        rootCommand.AddCommand(ProjectCommand.Create());
        rootCommand.AddCommand(ItemCommand.Create());
        rootCommand.AddCommand(DatabaseCommand.Create());
        rootCommand.AddCommand(GitHubCommand.Create());
        rootCommand.AddCommand(GithubActionWorkflowCommand.Create());

        return rootCommand;
    }
}
