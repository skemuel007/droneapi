using Application.DTOs.Drone;
using System.Net;
using Application.DTOs.DronePayload;
using Application.Features.Drone.Request.Commands;
using Application.Features.DronePayload.Request.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Drone.Request.Queries;
using Application.DTOs.Common;
using Domain.Entities;
using Application.DTOs.Medication;
using Application.Features.Medication.Request.Queries;
using Application.Responses;
using Application.Features.DroneRequest.Request.Commands;
using Application.DTOs.DroneRequest;

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
    /// Make drone request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("drone/request", Name = "DroneRequest")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> MakeDroneRequest([FromBody] CreateDroneRequestDto request)
    {
        var command = new CreateDroneRequestCommand() { DroneRequestDto = request };
        var response = await _mediator.Send(command);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Add payload to drone, Note: make drone request before loading
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("drone/load",Name = "LoadDroneWithPayload")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddPayloadToDrone([FromBody] AddDronePayloadDto request)
    {
        /*var command = new CreateDroneRequestCommand()
        {
            DroneRequestDto = new Application.DTOs.DroneRequest.CreateDroneRequestDto()
            {
                Destination = request.Destination,
                DroneId= request.DroneId,
                DroneRequestCode= request.DroneRequestCode,
                Origin = request.Origin,
                Status = request.Status,
            }
        };
        var createDroneRequestCommandResponse = await _mediator.Send(command);

        if (createDroneRequestCommandResponse == null )
        {
            return StatusCode((int)createDroneRequestCommandResponse.StatusCode, createDroneRequestCommandResponse);
        }
        
        if ( createDroneRequestCommandResponse.StatusCode != HttpStatusCode.Created)
        {
            return StatusCode((int)createDroneRequestCommandResponse.StatusCode, createDroneRequestCommandResponse);
        }*/

        var addDronePayloadCommand = new AddDronePayloadCommand() { DronePayloadDto = request};
        var response = await _mediator.Send(addDronePayloadCommand);
        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Get drone details - i.e, battery level
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("drone/battery_level", Name = "GetDroneDetails")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetDroneBatteryLevel([FromQuery] DroneDetailsRequestDto request)
    {

        var query = new GetDroneDetailsRequest()
        {
            DroneDetailsRequestDto = request

        };
        var response = await _mediator.Send(query);

        return StatusCode((int)response.StatusCode, response);
    }


    /// <summary>
    /// Get Available drones
    /// </summary>
    /// <returns></returns>
    [HttpGet("drone/available", Name = "AvailableDroneList")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAvailableDrones()
    {
        var response = await _mediator.Send(new GetAvailableDronesForLoadingRequest());
        return Ok(response);
    }

    /// <summary>
    /// Registers a new drone
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("drone/register", Name = "RegisterDrone")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> RegisterDrone([FromBody] CreateDroneDto request)
    {
        var command = new CreateDroneCommand() { DroneDto = request };
        var createDroneResponse = await _mediator.Send(command);
        return StatusCode((int)createDroneResponse.StatusCode, createDroneResponse);
    }

    
    [HttpGet("medications/drone/{droneId}", Name = "GetDroneMedications")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseCommandResponse<Medication>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseCommandResponse))]
    public async Task<IActionResult> GetDroneMedications(Guid droneId)
    {

        var query = new GetMedicationsForDroneRequest()
        {
            DroneId = droneId
        };
        var response = await _mediator.Send(query);

        return StatusCode((int)response.StatusCode, response);
    }
}