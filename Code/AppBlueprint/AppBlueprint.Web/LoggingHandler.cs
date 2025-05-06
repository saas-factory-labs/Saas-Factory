public class LoggingHandler : DelegatingHandler
{
    public LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler)
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Log Request
        Console.WriteLine("Request:");
        Console.WriteLine(request);

        if (request.Content is not null)
        {
            string? requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"Request Body: {requestBody}");
        }

        HttpResponseMessage? response = await base.SendAsync(request, cancellationToken);

        // Log Response
        Console.WriteLine("Response:");
        Console.WriteLine(response);

        string? responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        Console.WriteLine($"Response Body: {responseBody}");

        return response;
    }
}
