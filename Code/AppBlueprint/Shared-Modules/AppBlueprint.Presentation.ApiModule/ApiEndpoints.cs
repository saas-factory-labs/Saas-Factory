namespace AppBlueprint.Presentation.ApiModule;

public static class ApiEndpoints
{

    public static class Accounts
    {
        private const string ControllerBaseUrl = "accounts";
        public const string Create = ControllerBaseUrl;
        public const string GetById = $"{ControllerBaseUrl}/{{idOrSlug}}";
        public const string GetAll = ControllerBaseUrl;
        public const string UpdateById = $"{ControllerBaseUrl}/{{id}}";
        public const string DeleteById = $"{ControllerBaseUrl}/{{id}}";
    }

    public static class AuditLogs
    {
        private const string ControllerBaseUrl = "audit-logs";
        public const string Create = ControllerBaseUrl;
        public const string GetById = $"{ControllerBaseUrl}/{{id}}";
        public const string GetAll = ControllerBaseUrl;
        public const string UpdateById = $"{ControllerBaseUrl}/{{id}}";
        public const string DeleteById = $"{ControllerBaseUrl}/{{id}}";
    }

    public static class Authentication
    {
        private const string ControllerBaseUrl = "authentication";
        public const string Login = ControllerBaseUrl + "/login";
        public const string Logout = ControllerBaseUrl + "/logout";
        public const string Register = ControllerBaseUrl + "/register";
        public const string RefreshToken = ControllerBaseUrl + "/refresh-token";
        public const string SetupMfa = ControllerBaseUrl + "/setup-mfa";
        public const string VerifyMfa = ControllerBaseUrl + "/verify-mfa";
        public const string PasswordReset = ControllerBaseUrl + "/password-reset";
        public const string PasswordResetConfirm = ControllerBaseUrl + "/password-reset-confirm";
        public const string CreateProfile = "profile/create";
        public const string DeactivateProfile = "profile/deactivate";
    }

    public static class DataExports
    {
        private const string ControllerBaseUrl = "data-exports";
        public const string Create = ControllerBaseUrl;
        public const string GetById = $"{ControllerBaseUrl}/{{id}}";
        public const string GetAll = ControllerBaseUrl;
        public const string UpdateById = $"{ControllerBaseUrl}/{{id}}";
        public const string DeleteById = $"{ControllerBaseUrl}/{{id}}";
    }

    public static class Files
    {
        private const string ControllerBaseUrl = "files";
        public const string Create = ControllerBaseUrl;
        public const string GetById = $"{ControllerBaseUrl}/{{id}}";
        public const string GetAll = ControllerBaseUrl;
        public const string UpdateById = $"{ControllerBaseUrl}/{{id}}";
        public const string DeleteById = $"{ControllerBaseUrl}/{{id}}";
    }

    public static class Integrations
    {
        private const string ControllerBaseUrl = "integrations";
        public const string Create = ControllerBaseUrl;
        public const string GetById = $"{ControllerBaseUrl}/{{id}}";
        public const string GetAll = ControllerBaseUrl;
        public const string UpdateById = $"{ControllerBaseUrl}/{{id}}";
        public const string DeleteById = $"{ControllerBaseUrl}/{{id}}";
    }

    public static class Roles
    {
        private const string ControllerBaseUrl = "roles";
        public const string Create = ControllerBaseUrl;
        public const string GetById = $"{ControllerBaseUrl}/{{id}}";
        public const string GetAll = ControllerBaseUrl;
        public const string UpdateById = $"{ControllerBaseUrl}/{{id}}";
        public const string DeleteById = $"{ControllerBaseUrl}/{{id}}";
    }

    public static class Subscriptions
    {
        private const string ControllerBaseUrl = "subscriptions";
        public const string Create = ControllerBaseUrl;
        public const string GetById = $"{ControllerBaseUrl}/{{id}}";
        public const string GetAll = ControllerBaseUrl;
        public const string UpdateById = $"{ControllerBaseUrl}/{{id}}";
        public const string DeleteById = $"{ControllerBaseUrl}/{{id}}";
    }

    public static class Users
    {
        private const string ControllerBaseUrl = "users";
        public const string Create = ControllerBaseUrl;
        public const string GetById = $"{ControllerBaseUrl}/{{id}}";
        public const string GetAll = ControllerBaseUrl;
        public const string UpdateById = $"{ControllerBaseUrl}/{{id}}";
        public const string DeleteById = $"{ControllerBaseUrl}/{{id}}";
    }

    public static class ApiKeys
    {
        private const string ControllerBaseUrl = "api-keys";
        public const string Create = ControllerBaseUrl;
        public const string GetById = $"{ControllerBaseUrl}/{{id}}";
        public const string GetAll = ControllerBaseUrl;
        public const string UpdateById = $"{ControllerBaseUrl}/{{id}}";
        public const string DeleteById = $"{ControllerBaseUrl}/{{id}}";
    }

    public static class Demos
    {
        private const string ControllerBaseUrl = "demos";
        public const string Create = ControllerBaseUrl;
        public const string GetById = $"{ControllerBaseUrl}/{{id}}";
        public const string GetAll = ControllerBaseUrl;
        public const string UpdateById = $"{ControllerBaseUrl}/{{id}}";
        public const string DeleteById = $"{ControllerBaseUrl}/{{id}}";
    }

    public static class Organizations
    {
        private const string ControllerBaseUrl = "organizations";
        public const string Create = ControllerBaseUrl;
        public const string GetById = $"{ControllerBaseUrl}/{{id}}";
        public const string GetAll = ControllerBaseUrl;
        public const string UpdateById = $"{ControllerBaseUrl}/{{id}}";
        public const string DeleteById = $"{ControllerBaseUrl}/{{id}}";
    }

    public static class Teams
    {
        private const string ControllerBaseUrl = "teams";
        public const string Create = ControllerBaseUrl;
        public const string GetById = $"{ControllerBaseUrl}/{{id}}";
        public const string GetAll = ControllerBaseUrl;
        public const string UpdateById = $"{ControllerBaseUrl}/{{id}}";
        public const string DeleteById = $"{ControllerBaseUrl}/{{id}}";
    }

    public static class Tenants
    {
        private const string ControllerBaseUrl = "tenants";
        public const string Create = ControllerBaseUrl;
        public const string GetById = $"{ControllerBaseUrl}/{{id}}";
        public const string GetAll = ControllerBaseUrl;
        public const string UpdateById = $"{ControllerBaseUrl}/{{id}}";
        public const string DeleteById = $"{ControllerBaseUrl}/{{id}}";
    }

    public static class Admins
    {
        private const string ControllerBaseUrl = "admins";
        public const string Create = ControllerBaseUrl;
        public const string GetById = $"{ControllerBaseUrl}/{{id}}";
        public const string GetAll = ControllerBaseUrl;
        public const string UpdateById = $"{ControllerBaseUrl}/{{id}}";
        public const string DeleteById = $"{ControllerBaseUrl}/{{id}}";
    }
}
