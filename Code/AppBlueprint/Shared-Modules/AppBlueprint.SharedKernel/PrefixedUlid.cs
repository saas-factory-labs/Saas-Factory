namespace AppBlueprint.SharedKernel;

public static class PrefixedUlid
{
    public static string Generate(string prefix)
    {
        // Use a timestamp-based approach similar to ULID
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var random = Guid.NewGuid().ToString("N")[..10]; // Take first 10 chars for randomness
        return $"{prefix}_{timestamp:X}_{random}";
    }
}
