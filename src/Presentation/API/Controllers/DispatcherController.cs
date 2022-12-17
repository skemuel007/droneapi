using Application.DTOs.DronePayload;
using Application.Features.DronePayload.Request.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiVersion("1.0")]
public class DispatcherController : BaseController
{
    private IMediator _mediator;
    public DispatcherController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    /// <summary>
    /// Add payload to drone
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("load_drone_single_item",Name = "LoadDroneWithPayload")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MakeDroneRequest([FromBody] AddDronePayloadDto request)
    {
        var command = new AddDronePayloadCommand() { DronePayloadDto = request };
        var response = await _mediator.Send(command);
        return StatusCode((int)response.StatusCode, response);
    }
}