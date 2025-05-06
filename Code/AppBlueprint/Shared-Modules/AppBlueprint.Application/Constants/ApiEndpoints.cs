namespace AppBlueprint.Application.Constants;

public static class ApiEndpoints
{
    private const string ApiBaseUrl = "api/v{version:apiVersion}";

    public static class Admins
    {
        private const string ControllerBaseUrl = ApiBaseUrl + "/admin";
        public const string Create = ControllerBaseUrl;
        public const string GetById = $"{ControllerBaseUrl}/{{id}}";
        public const string GetAll = ControllerBaseUrl;
    }
}
