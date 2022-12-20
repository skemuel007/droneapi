using System.Net;
using Application.DTOs.DroneRequest;
using Application.Features.Drone.Request.Queries;
using Application.Features.DroneRequest.Request.Commands;
using Application.Features.DroneRequest.Request.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiVersion("1.0")]
public class DroneRequestController : BaseController
{
    private IMediator _mediator;

    public DroneRequestController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    /// <summary>
    /// Make drone request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost(Name = "MakeDroneRequest")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> MakeDroneRequest([FromBody] CreateDroneRequestDto request)
    {
        var command = new CreateDroneRequestCommand() { DroneRequestDto = request };
        var response = await _mediator.Send(command);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Get drone request states
    /// </summary>
    /// <returns></returns>
    [HttpGet("drone_request_states", Name = "DroneRequestStates")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetDroneRequestState()
    {
        var response = await _mediator.Send(new GetDroneRequestStateRequest());
        return Ok(response);
    }
}