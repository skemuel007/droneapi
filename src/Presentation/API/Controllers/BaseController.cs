using System.Net;
using Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class BaseController : ControllerBase
{
    protected ActionResult ResolveActionDataResult<T>(T response) where T : BaseCommandResponse<T>
    {
        if (response.StatusCode == HttpStatusCode.OK)
        {
            return Ok(response);
        }
        else if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return BadRequest(response);
        }
        else if (response.StatusCode == HttpStatusCode.Created)
        {
            return StatusCode(StatusCodes.Status201Created, response as T);
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound(response);
        }
        else if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return Unauthorized(response);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}