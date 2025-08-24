using AppBlueprint.Infrastructure.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AppBlueprint.Infrastructure.Services;

public class DawaAddressLookupService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<DawaAddressLookupService> _logger;

    public DawaAddressLookupService(HttpClient httpClient, IConfiguration configuration, ILogger<DawaAddressLookupService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task LookupAddress(string address, CancellationToken cancellationToken = default)
    {
        // Base URL for DAWA API
        string baseUrl = _configuration["DawaApi:BaseUrl"] ??
            "https://api.dataforsyningen.dk/adgangsadresser";

        // Byg forespørgsels-URL
        string url = $"{baseUrl}?q={Uri.EscapeDataString(address)}";

        try
        {
            // Send GET-forespørgsel
            HttpResponseMessage response = await _httpClient.GetAsync(new Uri(url), cancellationToken);
            response.EnsureSuccessStatusCode();

            // Læs responsen som JSON
            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            // Print resultatet
            _logger.LogInformation(AddressLookupMessages.ApiResponse);
            _logger.LogInformation("{ResponseBody}", responseBody);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(AddressLookupMessages.ErrorFetchingData);
            _logger.LogError("{ErrorMessage}", e.Message);
        }
    }
}
