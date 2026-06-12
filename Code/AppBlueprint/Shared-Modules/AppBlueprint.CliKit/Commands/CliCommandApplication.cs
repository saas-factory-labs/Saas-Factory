using System.CommandLine;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.CliKit.Commands;

public sealed class CliCommandApplication
{
    private readonly IEnumerable<ICliCommand> _commands;
    private readonly ILogger<CliCommandApplication> _logger;

    public CliCommandApplication(IEnumerable<ICliCommand> commands, ILogger<CliCommandApplication> logger)
    {
        _commands = commands ?? throw new ArgumentNullException(nameof(commands));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public RootCommand BuildRootCommand(string description)
    {
        var rootCommand = new RootCommand(description);

        foreach (var command in _commands)
        {
            rootCommand.Add(command.Build());
        }

        return rootCommand;
    }

    public async Task<int> InvokeAsync(Command rootCommand, string[] args, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rootCommand);
        ArgumentNullException.ThrowIfNull(args);

        try
        {
            return await rootCommand.Parse(args).InvokeAsync(new InvocationConfiguration(), cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("CLI invocation cancelled.");
            return 130;
        }
    }
}
