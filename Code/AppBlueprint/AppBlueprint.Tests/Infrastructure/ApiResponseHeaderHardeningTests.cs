using System.Net;
using System.Text.Encodings.Web;
using AppBlueprint.Presentation.ApiModule.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppBlueprint.Tests.Infrastructure;

internal sealed class ApiResponseHeaderHardeningTests
{
    [Test]
    public async Task UseCustomMiddlewares_ShouldStripTechnologyHeadersBeforeSendingResponse()
    {
        using IHost host = await BuildHostAsync();
        using HttpClient client = host.GetTestClient();

        HttpResponseMessage response = await client.GetAsync(new Uri("/", UriKind.Relative));

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response.Headers.Contains("Server")).IsFalse();
        await Assert.That(response.Headers.Contains("X-Powered-By")).IsFalse();
        await Assert.That(response.Headers.Contains("X-AspNet-Version")).IsFalse();
        await Assert.That(response.Headers.GetValues("X-Content-Type-Options").Single()).IsEqualTo("nosniff");
        await Assert.That(response.Headers.GetValues("Strict-Transport-Security").Single()).IsEqualTo("max-age=31536000; includeSubDomains");
    }

    private static Task<IHost> BuildHostAsync()
    {
        return new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder.UseEnvironment(Environments.Production);
                webBuilder.UseTestServer();
                webBuilder.ConfigureServices(services =>
                {
                    services.AddLogging();
                    services.AddRouting();
                    services.AddAuthentication(TestAuthenticationHandler.SchemeName)
                        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                            TestAuthenticationHandler.SchemeName,
                            _ => { });
                    services.AddAuthorization();
                });
                webBuilder.Configure(app =>
                {
                    app.UseCustomMiddlewares();
                    app.Run(async context =>
                    {
                        context.Response.Headers["Server"] = "Apache/2.4.62 (Ubuntu)";
                        context.Response.Headers["X-Powered-By"] = "PHP/8.2.28";
                        context.Response.Headers["X-AspNet-Version"] = "4.0.30319";

                        await context.Response.WriteAsync("ok");
                    });
                });
            })
            .StartAsync();
    }

    private sealed class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "Test";

        public TestAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            => Task.FromResult(AuthenticateResult.NoResult());
    }
}
