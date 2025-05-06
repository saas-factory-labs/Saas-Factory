namespace DeploymentPortal.ApiService.Services.Pulumi.Cloudcostify;

public class PulumiCloudcostifyService
{
    //private string ap

    private async Task<bool> GetCostEstimate()
    {
        string? pulumiOutputJson = string.Empty;

        //HttpClient client = new HttpClient();
        //var response = await client.PostAsync("https://api.cloudcostify.com/api/v1/azure/estimate", new StringContent(pulumiOutputJson));

        return true;
        // call cloudcostify api and get cost estimate of azure resources (currently only supports azure aks)

        // check if cost estimate is within budget
    }
}
