using System.Reflection;
using AppBlueprint.Application.Constants;
using AppBlueprint.Presentation.ApiModule.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using AuthenticationController = AppBlueprint.Presentation.ApiModule.Controllers.Baseline.AuthenticationController;

namespace AppBlueprint.Tests.Infrastructure;

internal sealed class AuthorizationConfigurationSecurityTests
{
    private static readonly System.Reflection.Assembly PresentationAssembly = typeof(AuthenticationController).Assembly;
    private static readonly System.Reflection.Assembly ApiServiceAssembly = System.Reflection.Assembly.Load("AppBlueprint.ApiService");

    [Test]
    public void AuthorizePoliciesUsedByApiControllers_ShouldBeRegistered()
    {
        AuthorizationOptions authorizationOptions = BuildAuthorizationOptions();

        MissingPolicy[] missingPolicies = GetAuthorizeAttributes()
            .Where(attribute => !string.IsNullOrWhiteSpace(attribute.Attribute.Policy))
            .Select(attribute => new MissingPolicy(attribute.Attribute.Policy!, attribute.Location))
            .Concat(GetPublicStringConstants(typeof(AuthorizationPolicyNames))
                .Select(policyName => new MissingPolicy(policyName, nameof(AuthorizationPolicyNames))))
            .Where(policy => authorizationOptions.GetPolicy(policy.Name) is null)
            .Distinct()
            .OrderBy(policy => policy.Name, StringComparer.Ordinal)
            .ThenBy(policy => policy.Location, StringComparer.Ordinal)
            .ToArray();

        missingPolicies.Should().BeEmpty(
            because: "every policy used by API [Authorize] attributes, and every canonical policy name, must be registered in AddJwtAuthentication");
    }

    [Test]
    public void AuthorizeRolesUsedByApiControllersAndPolicies_ShouldBeCanonicalRoles()
    {
        AuthorizationOptions authorizationOptions = BuildAuthorizationOptions();

        HashSet<string> canonicalRoles = GetPublicStringConstants(typeof(Roles))
            .ToHashSet(StringComparer.Ordinal);

        MissingRole[] unknownRoles = GetAuthorizeAttributes()
            .SelectMany(attribute => SplitRoles(attribute.Attribute.Roles)
                .Select(role => new MissingRole(role, attribute.Location)))
            .Concat(GetRegisteredPolicyRoles(authorizationOptions)
                .Select(role => new MissingRole(role, "registered authorization policy")))
            .Where(role => !canonicalRoles.Contains(role.Name))
            .Distinct()
            .OrderBy(role => role.Name, StringComparer.Ordinal)
            .ThenBy(role => role.Location, StringComparer.Ordinal)
            .ToArray();

        unknownRoles.Should().BeEmpty(
            because: "every API role requirement should be declared in AppBlueprint.Application.Constants.Roles");
    }

    private static AuthorizationOptions BuildAuthorizationOptions()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Provider"] = "Logto",
                ["Authentication:Logto:Endpoint"] = "https://issuer.example.test",
                ["Authentication:Logto:ClientId"] = "test-client",
                ["Authentication:Logto:ApiResource"] = "https://api.example.test"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddJwtAuthentication(configuration, new TestWebHostEnvironment());

        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;
    }

    private static IEnumerable<AuthorizeAttributeLocation> GetAuthorizeAttributes()
    {
        foreach (Type controllerType in GetControllerTypes())
        {
            foreach (AuthorizeAttribute attribute in controllerType.GetCustomAttributes<AuthorizeAttribute>(inherit: true))
            {
                yield return new AuthorizeAttributeLocation($"{controllerType.FullName}", attribute);
            }

            MethodInfo[] methods = controllerType.GetMethods(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly);

            foreach (MethodInfo method in methods)
            {
                foreach (AuthorizeAttribute attribute in method.GetCustomAttributes<AuthorizeAttribute>(inherit: true))
                {
                    yield return new AuthorizeAttributeLocation($"{controllerType.FullName}.{method.Name}", attribute);
                }
            }
        }
    }

    private static IEnumerable<Type> GetControllerTypes()
    {
        return new[] { PresentationAssembly, ApiServiceAssembly }
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => !type.IsAbstract)
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type));
    }

    private static IEnumerable<string> SplitRoles(string? roles)
    {
        if (string.IsNullOrWhiteSpace(roles))
            yield break;

        foreach (string role in roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            yield return role;
    }

    private static IEnumerable<string> GetRegisteredPolicyRoles(AuthorizationOptions authorizationOptions)
    {
        foreach (AuthorizationPolicy policy in GetRegisteredPolicies(authorizationOptions))
        {
            foreach (RolesAuthorizationRequirement requirement in policy.Requirements.OfType<RolesAuthorizationRequirement>())
            {
                foreach (string role in requirement.AllowedRoles)
                    yield return role;
            }
        }
    }

    private static IEnumerable<AuthorizationPolicy> GetRegisteredPolicies(AuthorizationOptions authorizationOptions)
    {
        foreach (string policyName in GetPublicStringConstants(typeof(AuthorizationPolicyNames)))
        {
            AuthorizationPolicy? policy = authorizationOptions.GetPolicy(policyName);
            if (policy is not null)
                yield return policy;
        }
    }

    private static IEnumerable<string> GetPublicStringConstants(Type type)
    {
        return type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(field => field is { IsLiteral: true, IsInitOnly: false })
            .Where(field => field.FieldType == typeof(string))
            .Select(field => (string)field.GetRawConstantValue()!);
    }

    private sealed record AuthorizeAttributeLocation(string Location, AuthorizeAttribute Attribute);
    private sealed record MissingPolicy(string Name, string Location);
    private sealed record MissingRole(string Name, string Location);

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "AppBlueprint.Tests";
        public string WebRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
