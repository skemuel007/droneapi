using System.Net;
using Application.DTOs.Common;
using Application.DTOs.Drone;
using Application.DTOs.DronePayload;
using Application.Features.Drone.Request.Commands;
using Application.Features.Drone.Request.Queries;
using Application.Features.DronePayload.Request.Commands;
using Application.Responses;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiVersion("1.0")]
public class DroneController : BaseController
{
    private IMediator _mediator;
    public DroneController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Get paginated drone list
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    [HttpGet(Name = "DroneList")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetDrones([FromQuery] PaginatedQueryParams queryParams)
    {
        var response = await _mediator.Send(new GetDroneListRequest
        {
            QueryParams = new PaginateQueryRequest<Drone>
            {
                FilterColumn = queryParams.FilterColumn,
                FilterQuery = queryParams.FilterQuery,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                SortColumn = queryParams.SortColumn,
                SortOrder = queryParams.SortOrder
            }
        });

        return Ok(response);
    }

    /// <summary>
    /// Get drone states
    /// </summary>
    /// <returns></returns>
    [HttpGet("drone_states", Name = "DroneStates")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetDroneStates()
    {
        var response = await _mediator.Send(new GetDroneStatesRequest());
        return Ok(response);
    }

    /// <summary>
    /// Get drone models
    /// </summary>
    /// <returns></returns>
    [HttpGet("drone_models", Name = "DroneModels")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetDroneModels()
    {
        var response = await _mediator.Send(new GetDroneModelsRequest());
        return Ok(response);
    }

    /// <summary>
    /// Update drone
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut(Name = "UpdateDrone")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateDrone([FromBody] UpdateDroneDto request)
    {
        var response = await _mediator.Send(new UpdateDroneCommand()
        {
            UpdateDroneDto = request
        });

        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Delete drone
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{droneId}", Name = "DeleteDrone")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteDrone(Guid droneId)
    {
        var command = new DeleteDroneCommand() { Id = droneId };
        var response = await _mediator.Send(command);

        return StatusCode((int)response.StatusCode, response);
    }

}