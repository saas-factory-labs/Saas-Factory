using Microsoft.AspNetCore.Mvc;

namespace DeploymentManager.ApiService.Api.Controllers.Authentication;

//  /api/deployment
[Route("[controller]")]
[ApiController]
public class
    AuthenticationController : ControllerBase // inherit base class with injected services (ILogger, pulumi automation api service)
{
    //InputModel inputModel
    // create login method
    [HttpPost]
    public async Task Login()
    {
        //var result = await pulumiAutomationApiService.Login(inputModel);

        //if (result is null)
        //{
        //    // return error
        //}
    }


    // [HttpGet]
    // public async Task GetDeployments()
    // {
    //     System.Console.WriteLine("Get all deployments across projects");
    // }

    // [HttpPost]
    // public async Task CreateProjectDeployment(InputModel inputModel)
    // {
    //     var result = await pulumiAutomationApiService.CreateOrUpdateProject(inputModel);

    //     if (result is null)
    //     {

    //     }
    // }
}

//using DeploymentPortal.ApiService.Models;
/*
- get pulumi automation api service
- create or update project
- create or update stack
- create or update stack config
- create or update stack secrets
- create or update stack tags
- create or update stack policy
- create azure infrastructure using stack
- create postgresql database
- deploy application to project infrastructure
*/
