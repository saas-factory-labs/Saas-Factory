using System.Runtime.CompilerServices;

namespace DeploymentManager.Tests;

internal sealed class DashboardResponsiveLayoutTests
{
    [Test]
    public async Task Dashboard_ShouldUseScopedResponsiveLayoutHooks()
    {
        string markup = await File.ReadAllTextAsync(GetRepoPath(
            "Code",
            "DeploymentManager",
            "DeploymentManager.Web",
            "Components",
            "Pages",
            "Dashboard.razor"));

        await Assert.That(markup).Contains("dashboard-metrics-grid");
        await Assert.That(markup).Contains("dashboard-metric-card");
        await Assert.That(markup).Contains("dashboard-health-grid");
        await Assert.That(markup).Contains("dashboard-health-card");
        await Assert.That(markup).Contains("dashboard-health-summary");
        await Assert.That(markup).Contains("dashboard-health-badge");
    }

    [Test]
    public async Task DashboardScopedCss_ShouldDefineMobileFriendlyGridAndOverflowRules()
    {
        string css = await File.ReadAllTextAsync(GetRepoPath(
            "Code",
            "DeploymentManager",
            "DeploymentManager.Web",
            "Components",
            "Pages",
            "Dashboard.razor.css"));

        await Assert.That(css).Contains("grid-template-columns: repeat(auto-fit, minmax(16rem, 1fr));");
        await Assert.That(css).Contains("grid-template-columns: repeat(auto-fit, minmax(18rem, 1fr));");
        await Assert.That(css).Contains("overflow-wrap: anywhere;");
        await Assert.That(css).Contains("@media (max-width: 39.99875rem)");
    }

    private static string GetRepoPath(
        string path1,
        string path2,
        string path3,
        string path4,
        string path5,
        string path6,
        [CallerFilePath] string sourceFilePath = "")
    {
        string testsDirectory = Path.GetDirectoryName(sourceFilePath)
            ?? throw new InvalidOperationException("Unable to resolve the test source directory.");

        return Path.GetFullPath(Path.Combine(testsDirectory, "..", "..", "..", path1, path2, path3, path4, path5, path6));
    }
}
