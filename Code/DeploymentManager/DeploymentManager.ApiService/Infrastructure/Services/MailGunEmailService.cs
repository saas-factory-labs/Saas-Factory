using System.Net.Http.Headers;
using System.Text;
using DeploymentPortal.ApiService.Domain.Interfaces;

namespace DeploymentPortal.ApiService.Infrastructure.Services;

public class MailGunEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public MailGunEmailService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        string? apiKey = _configuration["MailGun:ApiKey"];
        string? domain = _configuration["MailGun:Domain"];
        string? requestUri = $"https://api.mailgun.net/v3/{domain}/messages";

        var requestContent = new StringContent($"from=example@example.com&to={to}&subject={subject}&text={body}",
            Encoding.UTF8, "application/x-www-form-urlencoded");

        var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = requestContent
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{apiKey}")));

        HttpResponseMessage? response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public void SendEmail(string to, string subject, string body)
    {
        // This method is not implemented
    }
}
