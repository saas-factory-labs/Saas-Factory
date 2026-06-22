using System.Runtime.CompilerServices;

namespace AppBlueprint.AdminPortalKernel.Tests;

internal sealed class DashboardStatCardResponsiveLayoutTests
{
    [Test]
    public async Task DashboardStatCard_ShouldUseScopedResponsiveLayoutClass()
    {
        string markup = await File.ReadAllTextAsync(GetRepoPath(
            "Code",
            "AppBlueprint",
            "Shared-Modules",
            "AppBlueprint.AdminPortalKernel",
            "Components",
            "Shared",
            "DashboardStatCard.razor"));

        await Assert.That(markup).Contains("admin-dashboard-stat-card");
        await Assert.That(markup).DoesNotContain("col-span-12");
    }

    [Test]
    public async Task DashboardStatCardScopedCss_ShouldDefineMobileAndDesktopGridSpans()
    {
        string css = await File.ReadAllTextAsync(GetRepoPath(
            "Code",
            "AppBlueprint",
            "Shared-Modules",
            "AppBlueprint.AdminPortalKernel",
            "Components",
            "Shared",
            "DashboardStatCard.razor.css"));

        await Assert.That(css).Contains("grid-column: 1 / -1;");
        await Assert.That(css).Contains("@media (min-width: 40rem)");
        await Assert.That(css).Contains("@media (min-width: 80rem)");
        await Assert.That(css).Contains("overflow-wrap: anywhere;");
    }

    private static string GetRepoPath(
        string path1,
        string path2,
        string path3,
        string path4,
        string path5,
        string path6,
        string path7,
        [CallerFilePath] string sourceFilePath = "")
    {
        string testsDirectory = Path.GetDirectoryName(sourceFilePath)
            ?? throw new InvalidOperationException("Unable to resolve the test source directory.");

        return Path.GetFullPath(Path.Combine(
            testsDirectory,
            "..",
            "..",
            "..",
            "..",
            path1,
            path2,
            path3,
            path4,
            path5,
            path6,
            path7));
    }
}
