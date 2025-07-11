namespace AppBlueprint.AppGateway.Resources;

/// <summary>
/// Contains constant messages for AppGateway configuration and startup.
/// </summary>
internal static class ConfigurationMessages
{
    public const string DashboardEndpointMessage = "[AppGateway] Using dashboard OTLP endpoint: {0}";
    public const string DefaultEndpointMessage = "[AppGateway] Using default OTLP endpoint: {0}";
    public const string ExistingEndpointMessage = "[AppGateway] Using existing OTLP endpoint: {0}";
}
