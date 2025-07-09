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
        string templateContent = File.ReadAllText(templatePath);

        foreach (var app in saasApps)
        {
            // Replace placeholders
            string workflowContent = templateContent
                .Replace("${{ app_name }}", app.AppName)
                .Replace(
                    "${{ docker_context }}",
                    app.DockerContext)
                .Replace("${{ dockerfile_path }}", app.DockerfilePath);

            // Write the generated workflow file
            string outputPath = ".github/workflows/publish-container-image-github-registry.yml";
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            File.WriteAllText(outputPath, workflowContent);

            Console.WriteLine(
                $"publish-container-image-github-registry Workflow created for {app.AppName} at {outputPath}");
            Console.WriteLine("Remember to commit and push the workflow file to Github remote repository");
        }
    }
}
