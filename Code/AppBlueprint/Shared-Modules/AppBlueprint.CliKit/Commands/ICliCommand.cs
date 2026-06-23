using System.CommandLine;

namespace AppBlueprint.CliKit.Commands;

public interface ICliCommand
{
    Command Build();
}
