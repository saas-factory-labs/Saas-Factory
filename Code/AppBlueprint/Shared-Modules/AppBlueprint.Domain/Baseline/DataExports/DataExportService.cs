namespace AppBlueprint.Domain.Baseline.DataExports;

public static class DataExportService
{   
    public static Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data, string[] columnNames)
    {
        // Implementation pending
        throw new NotImplementedException();
    }

    public static Task<byte[]> ExportToJsonAsync<T>(IEnumerable<T> data)
    {
        // Implementation pending
        throw new NotImplementedException();
    }

    public static Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName)
    {
        // Implementation pending
        throw new NotImplementedException();
    }

    public static Task<string> ScheduleExportAsync(string exportType, object parameters, string userId)
    {
        // Implementation pending
        throw new NotImplementedException();
    }
}
