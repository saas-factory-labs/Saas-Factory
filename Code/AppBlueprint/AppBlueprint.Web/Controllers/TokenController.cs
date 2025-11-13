using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetToken()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        if (accessToken == null)
        {
            return Unauthorized();
        }

        return Ok(new { accessToken });
    }
}
