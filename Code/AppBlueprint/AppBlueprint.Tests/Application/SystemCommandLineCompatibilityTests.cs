using System.CommandLine;
using System.Reflection;
using FluentAssertions;

namespace AppBlueprint.Tests.Application;

internal sealed class SystemCommandLineCompatibilityTests
{
    [Test]
    public void GetOptionValue_WhenNonNullableHandlerParameterHasNoOptionValue_ThrowsArgumentNullException()
    {
        System.Reflection.Assembly developerCliAssembly = System.Reflection.Assembly.Load("AppBlueprint.DeveloperCli");
        Type compatibilityType = developerCliAssembly.GetType("AppBlueprint.DeveloperCli.Commands.SystemCommandLineCompatibility")!;

        MethodInfo getOptionValueMethod = compatibilityType
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(method => method.Name == "GetOptionValue" && method.IsGenericMethodDefinition)
            .MakeGenericMethod(typeof(string));

        var command = new Command("test");
        var option = new Option<string?>("--connection-string");
        command.Add(option);

        ParseResult parseResult = command.Parse([]);
        Action<string> handler = static _ => { };

        Action act = () => getOptionValueMethod.Invoke(null, [parseResult, option, handler, 0]);

        TargetInvocationException exception = act.Should()
            .Throw<TargetInvocationException>()
            .Which;

        exception.InnerException.Should().BeOfType<ArgumentNullException>()
            .Which.ParamName.Should().Be(option.Name);
    }
}
