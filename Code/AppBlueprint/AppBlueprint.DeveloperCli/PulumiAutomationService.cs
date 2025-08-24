using AppBlueprint.DeveloperCli.Resources;

namespace AppBlueprint.DeveloperCli;

internal sealed class PulumiAutomationService
{
    private PulumiAutomationService() { }

    public static async Task CreateGithubActionWorkflow(string appName)
    {
        string dockerContext = "./Code/App1/";
        string dockerFilePath = "./Code/App1/Dockerfile";

        // Define SaaS app configurations
        var saasApps = new[]
        {
            new
            {
                AppName = appName,
                DockerContext = dockerContext,
                DockerfilePath = dockerFilePath
            }
        };

        // Load the workflow template
        string templatePath = "publish-container-image-github-registry.yml";
        string templateContent = await File.ReadAllTextAsync(templatePath);

        foreach (var app in saasApps)
        {
            // Replace placeholders
            string workflowContent = templateContent
                .Replace("${{ app_name }}", app.AppName, StringComparison.Ordinal)
                .Replace(
                    "${{ docker_context }}",
                    app.DockerContext, StringComparison.Ordinal)
                .Replace("${{ dockerfile_path }}", app.DockerfilePath, StringComparison.Ordinal);

            // Write the generated workflow file
            string outputPath = ".github/workflows/publish-container-image-github-registry.yml";
            string? directoryPath = Path.GetDirectoryName(outputPath);
            if (directoryPath is not null)
            {
                Directory.CreateDirectory(directoryPath);
            }
            await File.WriteAllTextAsync(outputPath, workflowContent);

            Console.WriteLine(
                $"publish-container-image-github-registry Workflow created for {app.AppName} at {outputPath}");
#pragma warning disable CA1303 // Do not pass literals as localized parameters - using resource constant
            Console.WriteLine(Messages.CommitReminder);
#pragma warning restore CA1303
        }
    }
}
