using System.CommandLine;

namespace AppBlueprint.DeveloperCli.Commands;

internal static class SystemCommandLineCompatibility
{
    public static void AddCommand(this Command command, Command subcommand) => command.Add(subcommand);
    public static void AddOption(this Command command, Option option) => command.Add(option);
    public static void AddArgument(this Command command, Argument argument) => command.Add(argument);

    public static void SetHandler(this Command command, Action handler)
        => command.SetAction(_ => handler());

    public static void SetHandler<T1>(this Command command, Action<T1> handler, Option<T1> option1)
        => command.SetAction(parseResult => handler(parseResult.GetValue(option1)!));

    public static void SetHandler<T1>(this Command command, Action<T1> handler, Argument<T1> argument1)
        => command.SetAction(parseResult => handler(parseResult.GetRequiredValue(argument1)));

    public static void SetHandler<T1, T2>(this Command command, Action<T1, T2> handler, Option<T1> option1, Option<T2> option2)
        => command.SetAction(parseResult => handler(parseResult.GetValue(option1)!, parseResult.GetValue(option2)!));

    public static void SetHandler<T1, T2>(this Command command, Action<T1, T2> handler, Argument<T1> argument1, Option<T2> option2)
        => command.SetAction(parseResult => handler(parseResult.GetRequiredValue(argument1), parseResult.GetValue(option2)!));

    public static void SetHandler<T1, T2, T3>(this Command command, Action<T1, T2, T3> handler, Option<T1> option1, Option<T2> option2, Option<T3> option3)
        => command.SetAction(parseResult => handler(parseResult.GetValue(option1)!, parseResult.GetValue(option2)!, parseResult.GetValue(option3)!));

    public static void SetHandler<T1, T2, T3>(this Command command, Action<T1, T2, T3> handler, Argument<T1> argument1, Argument<T2> argument2, Option<T3> option3)
        => command.SetAction(parseResult => handler(parseResult.GetRequiredValue(argument1), parseResult.GetRequiredValue(argument2), parseResult.GetValue(option3)!));

    public static void SetHandler<T1>(this Command command, Func<T1, Task> handler, Option<T1> option1)
        => command.SetAction(async parseResult => await handler(parseResult.GetValue(option1)!));

    public static void SetHandler<T1, T2>(this Command command, Func<T1, T2, Task> handler, Option<T1> option1, Option<T2> option2)
        => command.SetAction(async parseResult => await handler(parseResult.GetValue(option1)!, parseResult.GetValue(option2)!));

    public static void SetHandler<T1, T2, T3>(this Command command, Func<T1, T2, T3, Task> handler, Option<T1> option1, Option<T2> option2, Option<T3> option3)
        => command.SetAction(async parseResult => await handler(parseResult.GetValue(option1)!, parseResult.GetValue(option2)!, parseResult.GetValue(option3)!));

    public static void SetHandler<T1, T2, T3, T4, T5, T6, T7>(
        this Command command,
        Func<T1, T2, T3, T4, T5, T6, T7, Task> handler,
        Option<T1> option1,
        Option<T2> option2,
        Option<T3> option3,
        Option<T4> option4,
        Option<T5> option5,
        Option<T6> option6,
        Option<T7> option7)
        => command.SetAction(async parseResult => await handler(
            parseResult.GetValue(option1)!,
            parseResult.GetValue(option2)!,
            parseResult.GetValue(option3)!,
            parseResult.GetValue(option4)!,
            parseResult.GetValue(option5)!,
            parseResult.GetValue(option6)!,
            parseResult.GetValue(option7)!));
}
