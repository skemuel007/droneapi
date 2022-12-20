using Application.DTOs.Drone;
using System.Net;
using Application.DTOs.DronePayload;
using Application.Features.Drone.Request.Commands;
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
    /// Add payload to drone, Note: make drone request before loading
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("drone/load",Name = "LoadDroneWithPayload")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MakeDroneRequest([FromBody] AddDronePayloadDto request)
    {
        var command = new AddDronePayloadCommand() { DronePayloadDto = request };
        var response = await _mediator.Send(command);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Registers a new drone
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("drone/register", Name = "RegisterDrone")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> Post([FromBody] CreateDroneDto request)
    {
        var command = new CreateDroneCommand() { DroneDto = request };
        var createDroneResponse = await _mediator.Send(command);
        return StatusCode((int)createDroneResponse.StatusCode, createDroneResponse);
    }
}