using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SaaSFactory.Testing;

public static class CodeQlTestRunner
{
    private const string DefaultCodeQlCliPath = @"D:\Tools\codeql";
    private const string DefaultCodeQlQueriesPath = @"D:\Tools\codeql-queries";

    public static void AssertNoCodeQlViolations(
        string language,
        string targetProjectOrFolder,
        string querySuite = "",
        string externalQueryPath = "")
    {
        string sourceRoot = FindSolutionRoot();
        string normalizedQuerySuite = NormalizeQuerySuite(language, querySuite);
        string? codeQlQueriesPath = ResolveCodeQlQueriesPath(sourceRoot, normalizedQuerySuite, externalQueryPath);
        string fullSuitePath = ResolveQuerySuitePath(sourceRoot, normalizedQuerySuite, codeQlQueriesPath);
        string codeQlExecutablePath = ResolveCodeQlExecutablePath();

        if (!File.Exists(fullSuitePath))
        {
            throw new FileNotFoundException($"Could not find the CodeQL query suite '{fullSuitePath}'.");
        }

        if (!string.IsNullOrWhiteSpace(codeQlQueriesPath))
        {
            UpdateCodeQlQueriesRepository(codeQlQueriesPath);
        }

        string testResultsDir = Path.Combine(sourceRoot, "TestResults", "CodeQL");
        Directory.CreateDirectory(testResultsDir);

        string tempId = Guid.NewGuid().ToString("N")[..8];
        string dbPath = Path.Combine(Path.GetTempPath(), $"codeql_db_{language}_{tempId}");
        string sarifPath = Path.Combine(testResultsDir, $"codeql_result_{language}.sarif");
        string absoluteSarifPath = Path.GetFullPath(sarifPath);
        string configPath = Path.Combine(Path.GetTempPath(), $"codeql_config_{tempId}.yml");

        try
        {
            string yamlConfig =
                "name: \"SaaS Factory CodeQL Filter\"\n" +
                "paths-ignore:\n" +
                "  - 'codeql-queries/**'\n" +
                "  - '**/obj/**'\n" +
                "  - '**/bin/**'\n";

            File.WriteAllText(configPath, yamlConfig);

            string resourceArguments = BuildResourceArguments();
            string workingDir = sourceRoot;
            string createArguments;

            if (string.Equals(language, "csharp", StringComparison.OrdinalIgnoreCase))
            {
                string targetProjectPath = Path.IsPathRooted(targetProjectOrFolder)
                    ? targetProjectOrFolder
                    : Path.GetFullPath(Path.Combine(sourceRoot, targetProjectOrFolder));

                createArguments =
                    $"database create \"{dbPath}\" --language=csharp {resourceArguments} " +
                    $"--command=\"dotnet build \\\"{targetProjectPath}\\\" --no-incremental\" " +
                    $"--codescanning-config=\"{configPath}\" --overwrite --verbose";
            }
            else
            {
                string targetFolderPath = Path.IsPathRooted(targetProjectOrFolder)
                    ? targetProjectOrFolder
                    : Path.GetFullPath(Path.Combine(sourceRoot, targetProjectOrFolder));

                workingDir = targetFolderPath;
                createArguments =
                    $"database create \"{dbPath}\" --language=javascript-typescript " +
                    $"--source-root=\"{targetFolderPath}\" {resourceArguments} " +
                    $"--command=\"cmd /c echo skipping\" --codescanning-config=\"{configPath}\" " +
                    "--overwrite --verbose";
            }

            RunProcess(
                fileName: codeQlExecutablePath,
                arguments: createArguments,
                workingDirectory: workingDir);

            string analyzeArguments =
                $"database analyze \"{dbPath}\" \"{fullSuitePath}\" --format=sarif-latest " +
                $"--output=\"{absoluteSarifPath}\" {resourceArguments} --verbose";

            RunProcess(
                fileName: codeQlExecutablePath,
                arguments: analyzeArguments,
                workingDirectory: sourceRoot);

            FilterSarifResults(absoluteSarifPath);

            LogYellowToLiveConsole(
                "\n=======================================================================================\n" +
                "[CodeQL] Report written to:\n" +
                $"--> {absoluteSarifPath}\n" +
                "=======================================================================================\n");

            EvaluateSarifFile(absoluteSarifPath);
        }
        finally
        {
            if (Directory.Exists(dbPath))
            {
                Directory.Delete(dbPath, recursive: true);
            }

            if (File.Exists(configPath))
            {
                File.Delete(configPath);
            }
        }
    }

    private static string NormalizeQuerySuite(string language, string querySuite)
    {
        if (!string.IsNullOrWhiteSpace(querySuite))
        {
            return querySuite;
        }

        return string.Equals(language, "csharp", StringComparison.OrdinalIgnoreCase)
            ? Path.Combine("codeql-queries", "csharp", "ql", "src", "codeql-suites", "csharp-security-and-quality.qls")
            : Path.Combine("codeql-queries", "javascript", "ql", "src", "codeql-suites", "javascript-security-and-quality.qls");
    }

    private static string BuildResourceArguments()
    {
        int codeQlThreads = ResolveCodeQlThreads();
        int? codeQlRamMegabytes = ResolveCodeQlRamMegabytes();
        string ramArgument = codeQlRamMegabytes is null ? string.Empty : $" --ram={codeQlRamMegabytes.Value}";

        return $"--threads={codeQlThreads}{ramArgument}";
    }

    private static string ResolveCodeQlExecutablePath()
    {
        string? configuredCodeQlCliPath = Environment.GetEnvironmentVariable("CODEQL_CLI_PATH");
        if (!string.IsNullOrWhiteSpace(configuredCodeQlCliPath))
        {
            return ResolveConfiguredCodeQlExecutablePath(configuredCodeQlCliPath);
        }

        if (TryResolveCodeQlExecutablePath(DefaultCodeQlCliPath, out string? defaultExecutablePath))
        {
            return defaultExecutablePath!;
        }

        return "codeql";
    }

    private static string ResolveConfiguredCodeQlExecutablePath(string configuredCodeQlCliPath)
    {
        if (TryResolveCodeQlExecutablePath(configuredCodeQlCliPath, out string? configuredExecutablePath))
        {
            return configuredExecutablePath!;
        }

        throw new FileNotFoundException($"Could not find the CodeQL CLI executable from '{configuredCodeQlCliPath}'.");
    }

    private static bool TryResolveCodeQlExecutablePath(string configuredCodeQlCliPath, out string? executablePath)
    {
        if (File.Exists(configuredCodeQlCliPath))
        {
            executablePath = Path.GetFullPath(configuredCodeQlCliPath);
            return true;
        }

        if (Directory.Exists(configuredCodeQlCliPath))
        {
            string windowsExecutablePath = Path.Combine(configuredCodeQlCliPath, "codeql.exe");
            if (File.Exists(windowsExecutablePath))
            {
                executablePath = windowsExecutablePath;
                return true;
            }

            string portableExecutablePath = Path.Combine(configuredCodeQlCliPath, "codeql");
            if (File.Exists(portableExecutablePath))
            {
                executablePath = portableExecutablePath;
                return true;
            }
        }

        executablePath = null;
        return false;
    }

    private static string? ResolveCodeQlQueriesPath(string sourceRoot, string querySuite, string externalQueryPath)
    {
        if (!string.IsNullOrWhiteSpace(externalQueryPath))
        {
            string fullExternalQueryPath = Path.GetFullPath(externalQueryPath);
            if (Directory.Exists(fullExternalQueryPath))
            {
                return fullExternalQueryPath;
            }

            throw new DirectoryNotFoundException($"Could not find the CodeQL queries path '{fullExternalQueryPath}'.");
        }

        string sourceRootSuitePath = Path.GetFullPath(Path.Combine(sourceRoot, querySuite));
        if (File.Exists(sourceRootSuitePath))
        {
            return null;
        }

        string? configuredCodeQlQueriesPath = Environment.GetEnvironmentVariable("CODEQL_QUERIES_PATH");
        if (TryResolveCodeQlQueriesPath(configuredCodeQlQueriesPath, out string? configuredQueriesPath))
        {
            return configuredQueriesPath;
        }

        if (TryResolveCodeQlQueriesPath(DefaultCodeQlQueriesPath, out string? defaultQueriesPath))
        {
            return defaultQueriesPath;
        }

        return null;
    }

    private static bool TryResolveCodeQlQueriesPath(string? candidatePath, out string? resolvedPath)
    {
        if (!string.IsNullOrWhiteSpace(candidatePath) && Directory.Exists(candidatePath))
        {
            resolvedPath = Path.GetFullPath(candidatePath);
            return true;
        }

        resolvedPath = null;
        return false;
    }

    private static string ResolveQuerySuitePath(string sourceRoot, string querySuite, string? codeQlQueriesPath)
    {
        if (Path.IsPathRooted(querySuite))
        {
            return Path.GetFullPath(querySuite);
        }

        string sourceRootCandidate = Path.GetFullPath(Path.Combine(sourceRoot, querySuite));
        if (File.Exists(sourceRootCandidate))
        {
            return sourceRootCandidate;
        }

        if (!string.IsNullOrWhiteSpace(codeQlQueriesPath))
        {
            string relativeQuerySuite = TrimLeadingCodeQlQueriesDirectory(querySuite);
            return Path.GetFullPath(Path.Combine(codeQlQueriesPath, relativeQuerySuite));
        }

        return sourceRootCandidate;
    }

    private static string TrimLeadingCodeQlQueriesDirectory(string querySuite)
    {
        string normalizedQuerySuite = querySuite
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);

        string prefix = $"codeql-queries{Path.DirectorySeparatorChar}";
        return normalizedQuerySuite.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? normalizedQuerySuite[prefix.Length..]
            : normalizedQuerySuite;
    }

    private static void UpdateCodeQlQueriesRepository(string codeQlQueriesPath)
    {
        string gitDirectoryPath = Path.Combine(codeQlQueriesPath, ".git");
        if (!Directory.Exists(gitDirectoryPath) && !File.Exists(gitDirectoryPath))
        {
            throw new DirectoryNotFoundException($"The CodeQL queries directory '{codeQlQueriesPath}' is not a Git repository.");
        }

        RunProcess(
            fileName: "git",
            arguments: "pull --ff-only",
            workingDirectory: codeQlQueriesPath,
            environmentVariables: new Dictionary<string, string>
            {
                ["GIT_TERMINAL_PROMPT"] = "0",
                ["GCM_INTERACTIVE"] = "Never"
            });
    }

    private static int ResolveCodeQlThreads()
    {
        string? configuredCodeQlThreads = Environment.GetEnvironmentVariable("CODEQL_THREADS");
        if (string.IsNullOrWhiteSpace(configuredCodeQlThreads))
        {
            return 0;
        }

        if (int.TryParse(configuredCodeQlThreads, out int codeQlThreads))
        {
            return codeQlThreads;
        }

        throw new InvalidOperationException($"The CODEQL_THREADS value '{configuredCodeQlThreads}' is not a valid integer.");
    }

    private static int? ResolveCodeQlRamMegabytes()
    {
        string? configuredCodeQlRamMegabytes = Environment.GetEnvironmentVariable("CODEQL_RAM_MB");
        if (string.IsNullOrWhiteSpace(configuredCodeQlRamMegabytes))
        {
            return null;
        }

        if (!int.TryParse(configuredCodeQlRamMegabytes, out int codeQlRamMegabytes))
        {
            throw new InvalidOperationException($"The CODEQL_RAM_MB value '{configuredCodeQlRamMegabytes}' is not a valid integer.");
        }

        if (codeQlRamMegabytes < 2048)
        {
            throw new InvalidOperationException("The CODEQL_RAM_MB value must be at least 2048.");
        }

        return codeQlRamMegabytes;
    }

    private static string FindSolutionRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "SaaS-Factory.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new DirectoryNotFoundException("Could not locate the SaaS-Factory solution root.");
    }

    private static void RunProcess(
        string fileName,
        string arguments,
        string workingDirectory,
        IReadOnlyDictionary<string, string>? environmentVariables = null)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        if (environmentVariables is not null)
        {
            foreach ((string key, string value) in environmentVariables)
            {
                startInfo.Environment[key] = value;
            }
        }

        using var process = new Process { StartInfo = startInfo };
        var standardErrorBuilder = new StringBuilder();
        var outputLock = new object();

        process.OutputDataReceived += (_, args) =>
        {
            if (string.IsNullOrEmpty(args.Data))
            {
                return;
            }

            lock (outputLock)
            {
                LogToLiveConsole(args.Data);
            }
        };

        process.ErrorDataReceived += (_, args) =>
        {
            if (string.IsNullOrEmpty(args.Data))
            {
                return;
            }

            lock (outputLock)
            {
                LogToLiveConsole(args.Data);
                standardErrorBuilder.AppendLine(args.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"The command '{fileName} {arguments}' failed with exit code {process.ExitCode}.{Environment.NewLine}{standardErrorBuilder}");
        }
    }

    private static void LogToLiveConsole(string message)
    {
        using var standardOutputWriter = new StreamWriter(Console.OpenStandardOutput(), Encoding.UTF8) { AutoFlush = true };
        standardOutputWriter.WriteLine($"[CodeQL] {message}");
    }

    private static void LogYellowToLiveConsole(string message)
    {
        using var standardOutputWriter = new StreamWriter(Console.OpenStandardOutput(), Encoding.UTF8) { AutoFlush = true };
        standardOutputWriter.WriteLine($"[1;33m{message}[0m");
    }

    private static void EvaluateSarifFile(string sarifPath)
    {
        if (!File.Exists(sarifPath))
        {
            throw new FileNotFoundException($"CodeQL did not generate the expected SARIF report '{sarifPath}'.");
        }

        using var jsonDocument = JsonDocument.Parse(File.ReadAllText(sarifPath));
        JsonElement rootElement = jsonDocument.RootElement;

        if (rootElement.TryGetProperty("runs", out JsonElement runs)
            && runs.GetArrayLength() > 0
            && runs[0].TryGetProperty("results", out JsonElement results))
        {
            int alertCount = results.GetArrayLength();
            if (alertCount > 0)
            {
                throw new InvalidOperationException(
                    $"CodeQL found {alertCount} security findings. Review '{sarifPath}' for details.");
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
