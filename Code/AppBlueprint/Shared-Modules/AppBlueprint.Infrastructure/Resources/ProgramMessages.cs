namespace AppBlueprint.Infrastructure.Resources;

/// <summary>
/// Contains constant messages for program startup and configuration.
/// </summary>
public static class ProgramMessages
{
    public const string AppGatewayUsingDashboardEndpoint = "[AppGateway] Using dashboard OTLP endpoint: {EndpointUrl}";
    public const string AppGatewayUsingDefaultEndpoint = "[AppGateway] Using default OTLP endpoint: {EndpointUrl}";
    public const string AppGatewayUsingExistingEndpoint = "[AppGateway] Using existing OTLP endpoint: {EndpointUrl}";
    public const string WebUsingDefaultEndpoint = "[Web] Using default OTLP endpoint: {EndpointUrl}";
    public const string RememberCommitWorkflow = "Remember to commit and push the workflow file to Github remote repository";
}
