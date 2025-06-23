using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AppBlueprint.Infrastructure.Services;

public class DawaAddressLookupService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public DawaAddressLookupService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
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
            HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            // Læs responsen som JSON
            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            // Print resultatet
            Console.WriteLine("API Response:");
            Console.WriteLine(responseBody);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("Error fetching data:");
            Console.WriteLine(e.Message);
        }
    }
}
