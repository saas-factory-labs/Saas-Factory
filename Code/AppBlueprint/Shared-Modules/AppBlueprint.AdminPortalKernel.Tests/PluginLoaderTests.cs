using AppBlueprint.AdminPortalKernel.PluginLoading;
using AppBlueprint.AdminPortalKernel.Tests.Fixtures;
using FluentAssertions;

namespace AppBlueprint.AdminPortalKernel.Tests;

internal sealed class PluginLoaderTests
{
    private static string CreateTempPluginsFolder()
    {
        string path = Path.Combine(Path.GetTempPath(), "adminportal-plugins-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    private static void CopyTestAssemblyTo(string pluginsFolder)
    {
        string source = typeof(FixtureAdminModule).Assembly.Location;
        File.Copy(source, Path.Combine(pluginsFolder, Path.GetFileName(source)), overwrite: true);
    }

    [Test]
    public async Task LoadFrom_FolderWithModuleAssembly_DiscoversModules()
    {
        string pluginsFolder = CreateTempPluginsFolder();
        try
        {
            CopyTestAssemblyTo(pluginsFolder);

            AdminPortalPluginLoadResult result = AdminPortalPluginLoader.LoadFrom(pluginsFolder);

            result.FolderFound.Should().BeTrue();
            result.Modules.Should().Contain(m => m.Slug == "fixture-app");
            result.Assemblies.Should().Contain(a => a.GetName().Name == typeof(FixtureAdminModule).Assembly.GetName().Name);
        }
        finally
        {
            Directory.Delete(pluginsFolder, recursive: true);
        }

        await Task.CompletedTask;
    }

    [Test]
    public async Task LoadFrom_MissingFolder_ReturnsEmptyResultWithoutThrowing()
    {
        string missing = Path.Combine(Path.GetTempPath(), "adminportal-missing-" + Guid.NewGuid().ToString("N"));

        AdminPortalPluginLoadResult result = AdminPortalPluginLoader.LoadFrom(missing);

        result.FolderFound.Should().BeFalse();
        result.Modules.Should().BeEmpty();
        result.Assemblies.Should().BeEmpty();
        await Task.CompletedTask;
    }

    [Test]
    public async Task LoadFrom_EmptyOrNullPath_ReturnsEmptyResult()
    {
        AdminPortalPluginLoader.LoadFrom(null).Modules.Should().BeEmpty();
        AdminPortalPluginLoader.LoadFrom(string.Empty).Modules.Should().BeEmpty();
        AdminPortalPluginLoader.LoadFrom("   ").Modules.Should().BeEmpty();
        await Task.CompletedTask;
    }

    [Test]
    public async Task LoadFrom_AssemblyWithoutModules_IsLoadedButYieldsNoModules()
    {
        string pluginsFolder = CreateTempPluginsFolder();
        try
        {
            // FluentAssertions.dll contains no IAdminPortalModule implementations.
            string source = typeof(FluentAssertions.AssertionExtensions).Assembly.Location;
            File.Copy(source, Path.Combine(pluginsFolder, Path.GetFileName(source)), overwrite: true);

            AdminPortalPluginLoadResult result = AdminPortalPluginLoader.LoadFrom(pluginsFolder);

            result.Modules.Should().BeEmpty();
        }
        finally
        {
            Directory.Delete(pluginsFolder, recursive: true);
        }

        await Task.CompletedTask;
    }

    [Test]
    public async Task LoadFrom_NonAssemblyFile_IsSkippedWithoutThrowing()
    {
        string pluginsFolder = CreateTempPluginsFolder();
        try
        {
            await File.WriteAllTextAsync(Path.Combine(pluginsFolder, "not-an-assembly.dll"), "this is not IL");

            AdminPortalPluginLoadResult result = AdminPortalPluginLoader.LoadFrom(pluginsFolder);

            result.Modules.Should().BeEmpty();
            result.SkippedFiles.Should().ContainSingle(s => s.Contains("not-an-assembly.dll", StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            Directory.Delete(pluginsFolder, recursive: true);
        }
    }
}
