using System.Runtime.InteropServices;

namespace AppBlueprint.DeveloperCli.Utilities;

/// <summary>
/// Configuration settings for the Developer CLI
/// </summary>
internal static class CliConfiguration
{
    /// <summary>
    /// The global alias name for the CLI (e.g., 'saas')
    /// </summary>
    public const string AliasName = "saas";

    /// <summary>
    /// Display name of the CLI tool
    /// </summary>
    public const string DisplayName = "SaaS Factory";

    /// <summary>
    /// Check if running on Windows
    /// </summary>
    public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    /// <summary>
    /// Check if running on macOS
    /// </summary>
    public static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    /// <summary>
    /// Check if running on Linux
    /// </summary>
    public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    /// <summary>
    /// Get the directory where the CLI executable will be published
    /// </summary>
    public static string GetPublishDirectory()
    {
        string baseDir = AppContext.BaseDirectory;
        string publishDir = Path.Combine(baseDir, "publish");
        return publishDir;
    }

    /// <summary>
    /// Get the path where the CLI will be installed globally
    /// </summary>
    public static string GetGlobalInstallPath()
    {
        if (IsWindows)
        {
            // Windows: Use AppData\Local\SaaSFactory
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(localAppData, "SaaSFactory", "cli");
        }
        else
        {
            // macOS/Linux: Use ~/.saas-factory/cli
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(home, ".saas-factory", "cli");
        }
    }

    /// <summary>
    /// Get the source code root directory
    /// </summary>
    public static string GetSourceCodeRoot()
    {
        string currentDir = AppContext.BaseDirectory;

        // Navigate up to find Code/AppBlueprint directory
        while (!string.IsNullOrEmpty(currentDir))
        {
            if (Directory.Exists(Path.Combine(currentDir, "AppBlueprint.DeveloperCli")) ||
                Path.GetFileName(currentDir).Equals("AppBlueprint", StringComparison.OrdinalIgnoreCase))
            {
                return currentDir;
            }

            DirectoryInfo? parent = Directory.GetParent(currentDir);
            if (parent == null) break;
            currentDir = parent.FullName;
        }

        return AppContext.BaseDirectory;
    }
}
