namespace AppBlueprint.Application.Constants;

public static class ApiEndpoints
{
    private const string ApiBaseUrl = "api/v{version:apiVersion}";

#pragma warning disable CA1034 // Nested types should not be visible - Admins class is intentionally nested for API organization
    public static class Admins
    {
        private const string ControllerBaseUrl = ApiBaseUrl + "/admin";
        public const string Create = ControllerBaseUrl;
        public const string GetById = $"{ControllerBaseUrl}/{{id}}";
        public const string GetAll = ControllerBaseUrl;
    }
#pragma warning restore CA1034
}
