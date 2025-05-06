using Microsoft.AspNetCore.Mvc;

namespace DeploymentManager.ApiService.Api.Controllers.AppController;

[Route("api/[controller]")]
[ApiController]
public class AppController : ControllerBase
{
    // GET: api/<AppController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new[] { "value1", "value2" };
    }

    // GET api/<AppController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<AppController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<AppController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<AppController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
