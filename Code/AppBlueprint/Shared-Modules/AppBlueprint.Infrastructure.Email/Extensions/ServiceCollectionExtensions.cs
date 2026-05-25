using System.Reflection;
using AppBlueprint.Application.Interfaces.Email;
using AppBlueprint.Application.Options;
using AppBlueprint.Infrastructure.Services;
using AppBlueprint.Infrastructure.Services.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RazorLight;
using Resend;

namespace AppBlueprint.Infrastructure.Email.Extensions;

/// <summary>
/// Extension methods for registering AppBlueprint email services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AppBlueprint email services including Resend client, TransactionEmailService,
    /// and RazorLight-based email template rendering.
    /// Services are only registered if their configuration is present.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="customTemplatesPath">Optional path to custom templates folder in the deployed application.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAppBlueprintEmail(
        this IServiceCollection services,
        IConfiguration configuration,
        string? customTemplatesPath = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Try multiple environment variable naming conventions (in priority order)
        string? apiKey = Environment.GetEnvironmentVariable("RESEND_APIKEY")
                      ?? Environment.GetEnvironmentVariable("RESEND_API_KEY")
                      ?? configuration["Resend:ApiKey"];

        string? fromEmail = Environment.GetEnvironmentVariable("RESEND_FROMEMAIL")
                         ?? Environment.GetEnvironmentVariable("RESEND_FROM_EMAIL")
                         ?? configuration["Resend:FromEmail"];

        string? fromName = Environment.GetEnvironmentVariable("RESEND_FROMNAME")
                        ?? Environment.GetEnvironmentVariable("RESEND_FROM_NAME")
                        ?? configuration["Resend:FromName"];

        if (!string.IsNullOrWhiteSpace(apiKey) && !string.IsNullOrWhiteSpace(fromEmail))
        {
            var resendOptions = new ResendEmailOptions
            {
                ApiKey = apiKey,
                FromEmail = fromEmail,
                FromName = fromName,
                BaseUrl = configuration["Resend:BaseUrl"] ?? "https://api.resend.com",
                TimeoutSeconds = int.TryParse(configuration["Resend:TimeoutSeconds"], out int timeout) ? timeout : 30
            };

            services.AddSingleton<IOptions<ResendEmailOptions>>(new OptionsWrapper<ResendEmailOptions>(resendOptions));

            services.AddOptions();
            services.AddHttpClient<ResendClient>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri(resendOptions.BaseUrl, UriKind.Absolute);
                    client.Timeout = TimeSpan.FromSeconds(resendOptions.TimeoutSeconds);
                });
            services.Configure<ResendClientOptions>(o =>
            {
                o.ApiToken = resendOptions.ApiKey;
            });
            services.AddTransient<IResend, ResendClient>();

            services.AddScoped<TransactionEmailService>();
            Console.WriteLine("[AppBlueprint.Infrastructure.Email] Resend email service registered");
        }
        else
        {
            Console.WriteLine("[AppBlueprint.Infrastructure.Email] Resend not configured (optional)");
        }

        // Register RazorLight email template service
        IRazorLightEngine razorEngine = BuildRazorLightEngine(customTemplatesPath);
        services.AddSingleton(razorEngine);
        services.AddScoped<IEmailTemplateService>(sp =>
        {
            IResend? resend = sp.GetService<IResend>();
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<RazorEmailTemplateService>>();
            return new RazorEmailTemplateService(razorEngine, resend, logger);
        });

        return services;
    }

    private static IRazorLightEngine BuildRazorLightEngine(string? customTemplatesPath)
    {
        var builder = new RazorLightEngineBuilder();

        if (!string.IsNullOrEmpty(customTemplatesPath) && Directory.Exists(customTemplatesPath))
        {
            builder.UseFileSystemProject(customTemplatesPath);
        }
        else
        {
            Assembly emailAssembly = typeof(RazorEmailTemplateService).Assembly;
            const string templateNamespace = "AppBlueprint.Infrastructure.Services.Email.Templates";
            builder.UseEmbeddedResourcesProject(emailAssembly, templateNamespace);
        }

        builder.UseMemoryCachingProvider();

        return builder.Build();
    }
}
