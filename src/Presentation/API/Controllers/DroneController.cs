using System.Net;
using Application.DTOs.Common;
using Application.DTOs.Drone;
using Application.Features.Drone.Request.Commands;
using Application.Features.Drone.Request.Queries;
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
    /// Registers a new drone
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost(Name = "RegisterDrone")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> Post([FromBody] CreateDroneDto request)
    {
        var command = new CreateDroneCommand() { DroneDto = request };
        var createDroneResponse = await _mediator.Send(command);
        return StatusCode((int)createDroneResponse.StatusCode, createDroneResponse);
    }

    /// <summary>
    /// Get paginated drone list
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    [HttpGet(Name = "DroneList")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Get([FromQuery] PaginatedQueryParams queryParams)
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
    /// Get drone details
    /// </summary>
    /// <param name="droneId"></param>
    /// <returns></returns>
    [HttpGet("{droneId}", Name = "GetDrone")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetDroneById(Guid droneId)
    {

        var query = new GetDroneDetailsRequest()
        {
            Id = droneId
        };
        var response = await _mediator.Send(query);

        return StatusCode((int)response.StatusCode, response);
    }

    /// <summary>
    /// Update drone dto
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