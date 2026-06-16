using System.Reflection;
using AppBlueprint.AdminPortalKernel.Modules;

namespace AppBlueprint.AdminPortalKernel.PluginLoading;

/// <summary>Outcome of scanning a plugins folder for admin portal modules.</summary>
public sealed class AdminPortalPluginLoadResult
{
    internal AdminPortalPluginLoadResult(
        bool folderFound,
        IReadOnlyList<Assembly> assemblies,
        IReadOnlyList<IAdminPortalModule> modules,
        IReadOnlyList<string> skippedFiles)
    {
        FolderFound = folderFound;
        Assemblies = assemblies;
        Modules = modules;
        SkippedFiles = skippedFiles;
    }

    internal static AdminPortalPluginLoadResult Empty(bool folderFound) =>
        new(folderFound, [], [], []);

    /// <summary>False when the configured plugins folder does not exist (or no path was configured).</summary>
    public bool FolderFound { get; }

    /// <summary>Every successfully loaded plugin assembly, including those without modules.</summary>
    public IReadOnlyList<Assembly> Assemblies { get; }

    /// <summary>Module instances discovered across all loaded assemblies.</summary>
    public IReadOnlyList<IAdminPortalModule> Modules { get; }

    /// <summary>Files that could not be loaded as .NET assemblies, with the reason appended.</summary>
    public IReadOnlyList<string> SkippedFiles { get; }
}
