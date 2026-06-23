using System.CommandLine;
using System.Reflection;

namespace AppBlueprint.DeveloperCli.Commands;

internal static class SystemCommandLineCompatibility
{
    public static void SetHandler(this Command command, Action handler)
        => command.SetAction(_ => handler());

    public static void SetHandler<T1>(this Command command, Action<T1> handler, Option<T1> option1)
        => command.SetAction(parseResult => handler(GetOptionValue(parseResult, option1, handler, 0)));

    public static void SetHandler<T1>(this Command command, Action<T1> handler, Argument<T1> argument1)
        => command.SetAction(parseResult => handler(parseResult.GetRequiredValue(argument1)));

    public static void SetHandler<T1, T2>(this Command command, Action<T1, T2> handler, Option<T1> option1, Option<T2> option2)
        => command.SetAction(parseResult => handler(GetOptionValue(parseResult, option1, handler, 0), GetOptionValue(parseResult, option2, handler, 1)));

    public static void SetHandler<T1, T2>(this Command command, Action<T1, T2> handler, Argument<T1> argument1, Option<T2> option2)
        => command.SetAction(parseResult => handler(parseResult.GetRequiredValue(argument1), GetOptionValue(parseResult, option2, handler, 1)));

    public static void SetHandler<T1, T2, T3>(this Command command, Action<T1, T2, T3> handler, Option<T1> option1, Option<T2> option2, Option<T3> option3)
        => command.SetAction(parseResult => handler(GetOptionValue(parseResult, option1, handler, 0), GetOptionValue(parseResult, option2, handler, 1), GetOptionValue(parseResult, option3, handler, 2)));

    public static void SetHandler<T1, T2, T3>(this Command command, Action<T1, T2, T3> handler, Argument<T1> argument1, Argument<T2> argument2, Option<T3> option3)
        => command.SetAction(parseResult => handler(parseResult.GetRequiredValue(argument1), parseResult.GetRequiredValue(argument2), GetOptionValue(parseResult, option3, handler, 2)));

    public static void SetHandler<T1>(this Command command, Func<T1, Task> handler, Option<T1> option1)
        => command.SetAction(async parseResult => await handler(GetOptionValue(parseResult, option1, handler, 0)));

    public static void SetHandler<T1, T2>(this Command command, Func<T1, T2, Task> handler, Option<T1> option1, Option<T2> option2)
        => command.SetAction(async parseResult => await handler(GetOptionValue(parseResult, option1, handler, 0), GetOptionValue(parseResult, option2, handler, 1)));

    public static void SetHandler<T1, T2, T3>(this Command command, Func<T1, T2, T3, Task> handler, Option<T1> option1, Option<T2> option2, Option<T3> option3)
        => command.SetAction(async parseResult => await handler(GetOptionValue(parseResult, option1, handler, 0), GetOptionValue(parseResult, option2, handler, 1), GetOptionValue(parseResult, option3, handler, 2)));

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
            GetOptionValue(parseResult, option1, handler, 0),
            GetOptionValue(parseResult, option2, handler, 1),
            GetOptionValue(parseResult, option3, handler, 2),
            GetOptionValue(parseResult, option4, handler, 3),
            GetOptionValue(parseResult, option5, handler, 4),
            GetOptionValue(parseResult, option6, handler, 5),
            GetOptionValue(parseResult, option7, handler, 6)));

    private static T GetOptionValue<T>(ParseResult parseResult, Option<T> option, Delegate handler, int parameterIndex)
    {
        var value = parseResult.GetValue(option);
        if (ShouldRejectNull(handler, parameterIndex))
        {
            ArgumentNullException.ThrowIfNull(value, option.Name);
        }

        return value!;
    }

    private static bool ShouldRejectNull(Delegate handler, int parameterIndex)
    {
        ParameterInfo parameter = handler.Method.GetParameters()[parameterIndex];
        if (Nullable.GetUnderlyingType(parameter.ParameterType) is not null)
        {
            return false;
        }

        if (parameter.ParameterType.IsValueType)
        {
            return true;
        }

        var nullabilityInfo = new NullabilityInfoContext().Create(parameter);
        return nullabilityInfo.ReadState == NullabilityState.NotNull;
    }
}
