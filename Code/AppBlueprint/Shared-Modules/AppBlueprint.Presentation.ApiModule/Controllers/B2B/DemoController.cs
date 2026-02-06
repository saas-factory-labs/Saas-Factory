using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.B2B;

// [Authorize (Roles = Roles)]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/demo")]
[Produces("application/json")]
public class DemoController : BaseController
{
    private readonly IConfiguration _configuration;
    private static readonly string[] DemoValues = { "value1", "value2" };

    public DemoController(IConfiguration configuration) : base(configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    ///     Gets a list of demo values.
    /// </summary>
    /// <returns>An array of demo values.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<string>> Get()
    {
        return Ok(DemoValues);
    }

    /// <summary>
    ///     Gets a specific demo value by ID.
    /// </summary>
    /// <param name="id">The ID of the value to retrieve.</param>
    /// <returns>A single demo value.</returns>
    [HttpGet(ApiEndpoints.Demos.GetById)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<string> Get(int id)
    {
        if (id <= 0) return NotFound(new { Message = "Invalid ID provided." });
        return Ok("value");
    }

    /// <summary>
    ///     Creates a new demo value.
    /// </summary>
    /// <param name="value">The value to create.</param>
    [HttpPost(ApiEndpoints.Demos.Create)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult Post([FromBody] string value)
    {
        if (string.IsNullOrEmpty(value)) return BadRequest(new { Message = "Value cannot be empty." });
        return CreatedAtAction(nameof(Get), new { id = 1 }, value); // Placeholder ID for demonstration
    }

    /// <summary>
    ///     Updates an existing demo value by ID.
    /// </summary>
    /// <param name="id">The ID of the value to update.</param>
    /// <param name="value">The new value.</param>
    [HttpPut(ApiEndpoints.Demos.UpdateById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Put(int id, [FromBody] string value)
    {
        if (id <= 0) return NotFound(new { Message = "Invalid ID provided." });
        if (string.IsNullOrEmpty(value)) return BadRequest(new { Message = "Value cannot be empty." });
        return NoContent();
    }

    /// <summary>
    ///     Deletes a demo value by ID.
    /// </summary>
    /// <param name="id">The ID of the value to delete.</param>
    [HttpDelete(ApiEndpoints.Demos.DeleteById)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Delete(int id)
    {
        if (id <= 0) return NotFound(new { Message = "Invalid ID provided." });
        return NoContent();
    }
}
