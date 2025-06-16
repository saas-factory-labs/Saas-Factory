using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Message = "Test endpoint working without tenant-id!", Timestamp = DateTime.UtcNow });
    }

    [HttpGet("protected")]
    public IActionResult GetProtected()
    {
        return Ok(new { Message = "This should require tenant-id", Timestamp = DateTime.UtcNow });
    }
}
