using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SaaSFactory.SharedTestKernel;

public static class CodeQlTestRunner
{
    public static void AssertNoCodeQlViolations(string language, string targetProjectOrFolder, string querySuite = "")
    {
        string sourceRoot = FindSolutionRoot();

        string testResultsDir = Path.Combine(sourceRoot, "TestResults", "CodeQL");
        Directory.CreateDirectory(testResultsDir);

        string tempId = Guid.NewGuid().ToString("N").Substring(0, 8);
        string dbPath = Path.Combine(Path.GetTempPath(), $"codeql_db_{language}_{tempId}");
        string sarifPath = Path.Combine(testResultsDir, $"codeql_result_{language}.sarif");
        string absoluteSarifPath = Path.GetFullPath(sarifPath);

        // Opret en midlertidig CodeQL-konfigurationsfil til at ignorere query-mappen
        string configPath = Path.Combine(Path.GetTempPath(), $"codeql_config_{tempId}.yml");

        if (string.IsNullOrEmpty(querySuite))
        {
            querySuite = language == "csharp"
                ? "codeql-queries\\csharp\\ql\\src\\codeql-suites\\csharp-security-and-quality.qls"
                : "codeql-queries\\javascript\\ql\\src\\codeql-suites\\javascript-security-and-quality.qls";
        }

        string fullSuitePath = Path.IsPathRooted(querySuite)
            ? querySuite
            : Path.GetFullPath(Path.Combine(sourceRoot, querySuite));

        if (!File.Exists(fullSuitePath))
        {
            throw new FileNotFoundException(
                $"[CodeQL Fejl] Kunne ikke finde Query Suite-filen!\nLeder efter den her: {fullSuitePath}");
        }

        try
        {
            // Generer YAML-konfiguration, der permanent ignorerer codeql-queries mappen og unødig støj
            string yamlConfig =
                "name: \"SaaS Factory CodeQL Filter\"\n" +
                "paths-ignore:\n" +
                "  - 'codeql-queries/**'\n" +
                "  - '**/obj/**'\n" +
                "  - '**/bin/**'\n";

            File.WriteAllText(configPath, yamlConfig);

            string workingDir = sourceRoot;
            string createCmd;

            if (language == "csharp")
            {
                // Brug --codescanning-config i stedet for det forældede --config
                createCmd = $"database create \"{dbPath}\" --language=csharp --command=\"dotnet build \"{targetProjectOrFolder}\" --no-incremental\" --codescanning-config=\"{configPath}\" --overwrite --verbose";
            }
            else
            {
                string fullTargetFolder = Path.IsPathRooted(targetProjectOrFolder)
                    ? targetProjectOrFolder
                    : Path.GetFullPath(Path.Combine(sourceRoot, targetProjectOrFolder));

                workingDir = fullTargetFolder;
                // Brug --codescanning-config i stedet for det forældede --config
                createCmd = $"database create \"{dbPath}\" --language=javascript-typescript --source-root=\"{fullTargetFolder}\" --command=\"cmd /c echo skipping\" --codescanning-config=\"{configPath}\" --overwrite --verbose";
            }

            // 1. Opret CodeQL Database (Nu med path exclusion sendt korrekt med)
            RunCodeQlCli(createCmd, workingDir);

            // 2. Analysér databasen
            var analyzeCmd = $"database analyze \"{dbPath}\" \"{fullSuitePath}\" --format=sarif-latest --output=\"{absoluteSarifPath}\" --verbose";
            RunCodeQlCli(analyzeCmd, sourceRoot);
            FilterSarifResults(absoluteSarifPath);

            // 3. Udskriv den færdige rapport-sti med TYDELIG GUL SKRIFT
            LogYellowToLiveConsole(
                "\n=======================================================================================\n" +
                $"[CodeQL] RAPPORT GEMT SIKKERT! Inspicer SARIF-filen her:\n" +
                $"--> {absoluteSarifPath}\n" +
                "=======================================================================================\n");

            // 4. Tjek resultatet
            EvaluateSarifFile(absoluteSarifPath);
        }
        finally
        {
            // Ryd op i temp-filer og mapper
            if (Directory.Exists(dbPath)) Directory.Delete(dbPath, true);
            if (File.Exists(configPath)) File.Delete(configPath);
        }
    }

    private static string FindSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "SaaS-Factory.sln")))
        {
            dir = dir.Parent;
        }
        return dir?.FullName ?? throw new DirectoryNotFoundException("Could not locate solution root folder (SaaS-Factory.sln).");
    }

    private static void RunCodeQlCli(string arguments, string workingDirectory)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "codeql",
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };
        var errorBuilder = new StringBuilder();
        var errorLock = new object();

        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data)) lock (errorLock) LogToLiveConsole(e.Data);
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                lock (errorLock)
                {
                    LogToLiveConsole(e.Data);
                    errorBuilder.AppendLine(e.Data);
                }
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception($"CodeQL CLI fejlede med exit-kode {process.ExitCode}. Fejllog:\n{errorBuilder}");
        }
    }

    private static void LogToLiveConsole(string message)
    {
        using var realStdOut = new StreamWriter(Console.OpenStandardOutput(), Encoding.UTF8) { AutoFlush = true };
        realStdOut.WriteLine($"[CodeQL] {message}");
    }

    private static void LogYellowToLiveConsole(string message)
    {
        using var realStdOut = new StreamWriter(Console.OpenStandardOutput(), Encoding.UTF8) { AutoFlush = true };
        realStdOut.WriteLine($"\u001b[1;33m{message}\u001b[0m");
    }

    private static void EvaluateSarifFile(string sarifPath)
    {
        if (!File.Exists(sarifPath))
            throw new FileNotFoundException($"CodeQL genererede ikke en SARIF-rapport på den forventede sti: {sarifPath}");

        using var jsonDoc = JsonDocument.Parse(File.ReadAllText(sarifPath));
        var root = jsonDoc.RootElement;

        if (root.TryGetProperty("runs", out var runs) && runs.GetArrayLength() > 0)
        {
            if (runs[0].TryGetProperty("results", out var results))
            {
                int alertCount = results.GetArrayLength();
                if (alertCount > 0)
                {
                    throw new Exception($"CodeQL Sikkerhedsalert: Fandt {alertCount} sårbarheder i koden!\nFilen er gemt og kan åbnes her: {sarifPath}");
                }
            }
        }
    }

    private static void FilterSarifResults(string sarifPath)
    {
        if (!File.Exists(sarifPath))
        {
            return;
        }

        JsonNode? rootNode = JsonNode.Parse(File.ReadAllText(sarifPath));
        if (rootNode?["runs"] is not JsonArray runs)
        {
            return;
        }

        foreach (JsonNode? runNode in runs)
        {
            if (runNode?["results"] is not JsonArray results)
            {
                continue;
            }

            var filteredResults = results
                .Where(result => !IsExcludedResult(result))
                .ToList();

            results.Clear();
            foreach (JsonNode? filteredResult in filteredResults)
            {
                results.Add(filteredResult);
            }
        }

        File.WriteAllText(
            sarifPath,
            rootNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }

    private static bool IsExcludedResult(JsonNode? resultNode)
    {
        string? uri = resultNode?["locations"]?[0]?["physicalLocation"]?["artifactLocation"]?["uri"]?.GetValue<string>();
        if (string.IsNullOrWhiteSpace(uri))
        {
            return false;
        }

        string normalizedUri = uri.Replace('\\', '/');
        return normalizedUri.StartsWith("codeql-queries/", StringComparison.OrdinalIgnoreCase);
    }
}
