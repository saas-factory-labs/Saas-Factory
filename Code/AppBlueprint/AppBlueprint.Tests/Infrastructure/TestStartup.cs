using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Tests.Infrastructure
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add any test-specific services here if needed
        }

        public void Configure(IApplicationBuilder app)
        {
            // Add any test-specific middleware here if needed
        }
    }
}
