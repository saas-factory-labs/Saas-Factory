using Microsoft.Extensions.Configuration;

namespace AppBlueprint.Infrastructure.Services;

public class LogSnagService
{
    private readonly IConfiguration _configuration;

    public LogSnagService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Notify()
    {
    }
}
