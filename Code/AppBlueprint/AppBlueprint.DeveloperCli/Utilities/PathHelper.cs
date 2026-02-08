using System.Runtime.InteropServices;

namespace AppBlueprint.DeveloperCli.Utilities;

/// <summary>
/// Helper class for managing PATH environment variable and shell configurations
/// </summary>
internal static class PathHelper
{
    /// <summary>
    /// Add a directory to the user's PATH environment variable (Windows)
    /// </summary>
    public static bool AddToPathWindows(string directory)
    {
        try
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Get current user PATH
            string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ?? string.Empty;

            // Check if already in PATH
            string[] paths = currentPath.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (paths.Any(p => p.Equals(directory, StringComparison.OrdinalIgnoreCase)))
            {
                return true; // Already in PATH
            }

            // Add to PATH
            string newPath = string.IsNullOrEmpty(currentPath)
                ? directory
                : $"{currentPath}{Path.PathSeparator}{directory}";

            Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.User);

            // Broadcast WM_SETTINGCHANGE to notify other processes
            BroadcastEnvironmentChange();

            return true;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error adding to PATH: {ex.Message}[/]");
            return false;
        }
    }

    /// <summary>
    /// Remove a directory from the user's PATH environment variable (Windows)
    /// </summary>
    public static bool RemoveFromPathWindows(string directory)
    {
        try
        {
            string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ?? string.Empty;
            string[] paths = currentPath.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

            // Filter out the directory
            string[] newPaths = paths.Where(p => !p.Equals(directory, StringComparison.OrdinalIgnoreCase)).ToArray();

            if (newPaths.Length == paths.Length)
            {
                return true; // Not in PATH, nothing to remove
            }

            string newPath = string.Join(Path.PathSeparator.ToString(), newPaths);
            Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.User);

            BroadcastEnvironmentChange();

            return true;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error removing from PATH: {ex.Message}[/]");
            return false;
        }
    }

    /// <summary>
    /// Check if a directory is in the user's PATH (Windows)
    /// </summary>
    public static bool IsInPathWindows(string directory)
    {
        string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ?? string.Empty;
        string[] paths = currentPath.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
        return paths.Any(p => p.Equals(directory, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Register shell alias for macOS/Linux
    /// </summary>
    public static bool RegisterShellAlias(string aliasName, string targetPath)
    {
        try
        {
            string? shellProfilePath = GetShellProfilePath();
            if (shellProfilePath == null)
            {
                AnsiConsole.MarkupLine("[red]Could not determine shell profile path[/]");
                return false;
            }

            string aliasLine = $"alias {aliasName}='dotnet \"{targetPath}\"'";

            // Read existing content
            List<string> lines = File.Exists(shellProfilePath)
                ? [.. File.ReadAllLines(shellProfilePath)]
                : [];

            // Check if alias already exists
            bool aliasExists = lines.Any(line => line.Contains($"alias {aliasName}=", StringComparison.Ordinal));

            if (!aliasExists)
            {
                // Add alias
                lines.Add("");
                lines.Add($"# SaaS Factory CLI alias (added by installer)");
                lines.Add(aliasLine);

                File.WriteAllLines(shellProfilePath, lines);
            }

            AnsiConsole.MarkupLine($"[green]Alias registered in {shellProfilePath}[/]");
            return true;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error registering shell alias: {ex.Message}[/]");
            return false;
        }
    }

    /// <summary>
    /// Unregister shell alias for macOS/Linux
    /// </summary>
    public static bool UnregisterShellAlias(string aliasName)
    {
        try
        {
            string? shellProfilePath = GetShellProfilePath();
            if (shellProfilePath == null || !File.Exists(shellProfilePath))
            {
                return true; // Nothing to remove
            }

            List<string> lines = [.. File.ReadAllLines(shellProfilePath)];
            List<string> newLines = [];
            bool skipNextLine = false;

            foreach (string line in lines)
            {
                if (skipNextLine)
                {
                    skipNextLine = false;
                    continue;
                }

                if (line.Contains("# SaaS Factory CLI alias", StringComparison.Ordinal))
                {
                    skipNextLine = true;
                    continue;
                }

                if (line.Contains($"alias {aliasName}=", StringComparison.Ordinal))
                {
                    continue;
                }

                newLines.Add(line);
            }

            File.WriteAllLines(shellProfilePath, newLines);
            return true;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error unregistering shell alias: {ex.Message}[/]");
            return false;
        }
    }

    /// <summary>
    /// Get the shell profile path (macOS/Linux)
    /// </summary>
    private static string? GetShellProfilePath()
    {
        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string? shell = Environment.GetEnvironmentVariable("SHELL");

        if (string.IsNullOrEmpty(shell))
        {
            // Try to detect shell
            if (File.Exists(Path.Combine(home, ".zshrc")))
            {
                return Path.Combine(home, ".zshrc");
            }

            if (File.Exists(Path.Combine(home, ".bashrc")))
            {
                return Path.Combine(home, ".bashrc");
            }

            return Path.Combine(home, ".bash_profile");
        }

        // Determine profile file based on shell
        if (shell.Contains("zsh", StringComparison.OrdinalIgnoreCase))
        {
            return Path.Combine(home, ".zshrc");
        }

        if (shell.Contains("bash", StringComparison.OrdinalIgnoreCase))
        {
            string bashrc = Path.Combine(home, ".bashrc");
            return File.Exists(bashrc) ? bashrc : Path.Combine(home, ".bash_profile");
        }

        return Path.Combine(home, ".profile");
    }

    /// <summary>
    /// Broadcast environment change to notify Windows applications
    /// </summary>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern IntPtr SendMessageTimeout(
        IntPtr hWnd,
        uint Msg,
        IntPtr wParam,
        string lParam,
        uint fuFlags,
        uint uTimeout,
        out IntPtr lpdwResult);

    private static void BroadcastEnvironmentChange()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        const uint HWND_BROADCAST = 0xffff;
        const uint WM_SETTINGCHANGE = 0x001a;
        const uint SMTO_ABORTIFHUNG = 0x0002;

        SendMessageTimeout(
            new IntPtr(HWND_BROADCAST),
            WM_SETTINGCHANGE,
            IntPtr.Zero,
            "Environment",
            SMTO_ABORTIFHUNG,
            5000,
            out var result);
    }
}
