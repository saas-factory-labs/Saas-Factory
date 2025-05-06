namespace AppBlueprint.DeveloperCli.Utilities;

internal static class ConnectionStringValidator
{
    public static async Task<bool> ValidatePostgreSqlConnection(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        try
        {
            using NpgsqlConnection connection = new(connectionString);
            await connection.OpenAsync();
            return true;
        }
        catch (NpgsqlException)
        {
            return false;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }
}
