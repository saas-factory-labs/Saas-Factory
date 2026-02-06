using System.Net.Http.Headers;
using System.Text;

namespace DeploymentManager.CloudInfrastructure;

public class CostEstimation
{
    public async Task GetCostEstimate()
    {
        // call Cloudcostify API to get cost estimation

        var client = new HttpClient();
        client.BaseAddress = new Uri("https://api.cloudcostify.com");
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("x-api-key", "key");

        HttpResponseMessage? response = await client.PostAsync(
            new Uri("https://api.cloudcostify.com/api/v1/cost-estimation"), new StringContent(@"{
        ""cloudProvider"": ""azure"",
        ""region"": ""eastus"",
        ""services"": [
            {
                ""service"": ""app-service"",
                ""sku"": ""F1"",
                ""quantity"": 1
            },
            {
                ""service"": ""container-registry"",
                ""sku"": ""Basic"",
                ""quantity"": 1
            },
            {
                ""service"": ""postgres"",
                ""sku"": ""B_Gen5_1"",
                ""quantity"": 1
            },
            {
                ""service"": ""postgres-storage"",
                ""sku"": ""5GB"",
                ""quantity"": 1
            },
            {
                ""service"": ""log-analytics"",
                ""sku"": ""PerGB2018"",
                ""quantity"": 1
            }
        ]
    }", Encoding.UTF8, "application/json"));

        string? result = await response.Content.ReadAsStringAsync();
        Console.WriteLine(result);
    }

    // return cost estimation
}