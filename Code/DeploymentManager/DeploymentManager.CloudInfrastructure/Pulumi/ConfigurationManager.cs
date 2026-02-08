using Pulumi;

namespace DeploymentManager.CloudInfrastructure;

public enum SettingType
{
    ProjectName,
    StackName,
    TenantId,
    Region,
    RegionShortCode,
    CloudProvider,
    SubscriptionId,
    ClientId,
    ClientSecret,
    ContainerRegistryAdminUsername,
    ContainerRegistryAdminPassword,
    ResourcePrefixName,
    DatabaseAdminLogin
}

public class ConfigurationManager
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _cloudProvider;
    private readonly Config _config;
    private readonly string _containerRegistryAdminPassword;
    private readonly string _containerRegistryAdminUsername;
    private readonly string _projectName;
    private readonly string _region;

    private readonly string _resourcePrefixName;
    private readonly string _stackName;
    private readonly string _subscriptionId;
    private readonly string _tenantId;

    public ConfigurationManager()
    {
        _config = new Config();

        string? projectName = _config.Require("projectName"); // saasdplmgr
        string? stackName = _config.Require("stackName"); // prod (environment name) 

        _tenantId = _config.Require("tenantId"); // 066136d6-59c1-47a0-9f59-e9874c06ae75";
        _region = _config.Require("region");
        _cloudProvider = _config.Require("cloudProvider");
        _subscriptionId = _config.Require("subscriptionId");
        _clientId = _config.Require("clientId");
        _clientSecret = _config.RequireSecret("clientSecret").ToString();

        _resourcePrefixName = projectName + stackName;
    }

    public string GetSetting(SettingType settingType)
    {
        return settingType switch
        {
            SettingType.ProjectName => _projectName,
            SettingType.StackName => _stackName,
            SettingType.TenantId => _tenantId,
            SettingType.Region => _region,
            SettingType.CloudProvider => _cloudProvider,
            SettingType.SubscriptionId => _subscriptionId,
            SettingType.ClientId => _clientId,
            SettingType.ClientSecret => _clientSecret,
            SettingType.ContainerRegistryAdminUsername => _containerRegistryAdminUsername,
            SettingType.ContainerRegistryAdminPassword => _containerRegistryAdminPassword,
            SettingType.ResourcePrefixName => _resourcePrefixName,
            _ => "",
        };
    }
}
