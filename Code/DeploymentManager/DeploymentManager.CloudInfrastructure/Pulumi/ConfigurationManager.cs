using Pulumi;

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
        switch (settingType)
        {
            case SettingType.ProjectName:
                return _projectName;
            case SettingType.StackName:
                return _stackName;
            case SettingType.TenantId:
                return _tenantId;
            case SettingType.Region:
                return _region;
            case SettingType.CloudProvider:
                return _cloudProvider;
            case SettingType.SubscriptionId:
                return _subscriptionId;
            case SettingType.ClientId:
                return _clientId;
            case SettingType.ClientSecret:
                return _clientSecret;
            case SettingType.ContainerRegistryAdminUsername:
                return _containerRegistryAdminUsername;
            case SettingType.ContainerRegistryAdminPassword:
                return _containerRegistryAdminPassword;
            case SettingType.ResourcePrefixName:
                return _resourcePrefixName;
            default:
                return "";
        }
    }
}
