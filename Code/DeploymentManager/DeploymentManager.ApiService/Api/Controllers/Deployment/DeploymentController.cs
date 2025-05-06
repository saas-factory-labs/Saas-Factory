using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Deployment;

//  /api/deployment
[ApiController]
[Route("[controller]")]
public class
    DeploymentController : Controller // inherit base class with injected services (ILogger, pulumi automation api service)
{
    [HttpGet]
    public IEnumerable<string> Get()
    {
        Console.WriteLine("Get all deployments across projects");

        return new[] { "value1", "value2" };
    }

    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}

//[HttpPost]
//public async Task CreateProjectDeployment(InputModel inputModel)
//{
//    var result = await pulumiAutomationApiService.CreateOrUpdateProject(inputModel);

//    if (result is null)
//    {

//    }
//}
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
