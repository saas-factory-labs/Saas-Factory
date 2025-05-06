namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class SessionEntity
{
    public int Id { get; set; }

    public string SessionKey { get; set; }
    public string SessionData { get; set; }
    public DateTime ExpireDate { get; set; }
}
