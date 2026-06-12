using Microsoft.Extensions.Configuration;

namespace AppBlueprint.Infrastructure.Services;

public class AlgoliaService
{
    private readonly IConfiguration _configuration;

    public AlgoliaService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task FulltextSearch()
    {
        return Task.CompletedTask;
    }
}
