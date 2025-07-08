namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public sealed class ApiLogEntity
{
    public required SessionEntity SessionEntity { get; set; } = new()
    {
        SessionKey = string.Empty,
        SessionData = string.Empty
    };

    public int Id { get; set; }
    public required string ApiKeyId { get; set; }
    public required string SessionId { get; set; }
    public required string RequestPath { get; set; }
    public int StatusCode { get; set; }
    public required string StatusMessage { get; set; }

    // GET, POST, PUT, PATCH, DELETE (Rest/GrahQL)
    public required string RequestMethod { get; set; }
    public required string SourceIp { get; set; }

    // msec
    public int ResponseLatency { get; set; }
}
