using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Infrastructure.Email.Extensions;

public static class MailKitEmailServiceCollectionExtensions
{
    public static IServiceCollection AddMailKitEmailService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        string? host = Environment.GetEnvironmentVariable("SMTP_HOST")
                    ?? configuration["Smtp:Host"];

        if (!string.IsNullOrWhiteSpace(host))
        {
            services.Configure<SmtpOptions>(options =>
            {
                options.Host = host;
                options.Port = int.TryParse(
                    Environment.GetEnvironmentVariable("SMTP_PORT") ?? configuration["Smtp:Port"],
                    out int port) ? port : 587;
                options.Username = Environment.GetEnvironmentVariable("SMTP_USERNAME")
                                ?? configuration["Smtp:Username"]
                                ?? string.Empty;
                options.Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD")
                                ?? configuration["Smtp:Password"]
                                ?? string.Empty;
                options.SenderName = Environment.GetEnvironmentVariable("SMTP_SENDER_NAME")
                                  ?? configuration["Smtp:SenderName"]
                                  ?? "SaaS Factory";
            });

            services.AddScoped<IEmailService, EmailService>();
            Console.WriteLine("[AppBlueprint.Infrastructure] MailKit SMTP email service registered");
        }
        else
        {
            Console.WriteLine("[AppBlueprint.Infrastructure] MailKit SMTP not configured (optional)");
        }

        return services;
    }
}
