using AppBlueprint.Application.Options;
using AppBlueprint.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Resend;

namespace AppBlueprint.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering the Resend transactional email service.
/// </summary>
public static class ResendEmailServiceCollectionExtensions
{
    /// <summary>
    /// Registers Resend email service if API key is configured.
    /// Supports multiple environment variable naming conventions:
    /// 1. RESEND_APIKEY (standard UPPERCASE format)
    /// 2. RESEND_API_KEY (legacy with underscores)
    /// </summary>
    public static IServiceCollection AddResendEmailService(
        this IServiceCollection services,
        IConfiguration configuration)
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

            // Configure Resend according to official documentation
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
            Console.WriteLine("[AppBlueprint.Infrastructure] Resend email service registered");
        }
        else
        {
            Console.WriteLine("[AppBlueprint.Infrastructure] Resend not configured (optional)");
        }

        return services;
    }
}
