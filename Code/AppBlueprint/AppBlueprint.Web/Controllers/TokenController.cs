using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
internal class TokenController : ControllerBase
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
