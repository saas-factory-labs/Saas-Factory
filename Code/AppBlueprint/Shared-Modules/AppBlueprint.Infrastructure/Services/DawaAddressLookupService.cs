using Microsoft.Extensions.Configuration;

namespace AppBlueprint.Infrastructure.Services;

public class DawaAddressLookupService
{
    private readonly IConfiguration _configuration;

    public DawaAddressLookupService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task LookupAddress(string address)
    {
        // Base URL for DAWA API
        string baseUrl = "https://api.dataforsyningen.dk/adgangsadresser";

        // Brugers input
        string searchQuery = "Hovedgaden 2";

        // Byg forespørgsels-URL
        string url = $"{baseUrl}?q={Uri.EscapeDataString(searchQuery)}";

        // HTTP klient
        using (var client = new HttpClient())
        {
            try
            {
                // Send GET-forespørgsel
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Læs responsen som JSON
                string responseBody = await response.Content.ReadAsStringAsync();

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
}
