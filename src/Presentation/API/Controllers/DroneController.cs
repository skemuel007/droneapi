using Application.DTOs.Common;
using Application.DTOs.Drone;
using Application.Features.Drone.Request.Commands;
using Application.Features.Drone.Request.Queries;
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

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateDroneDto request)
    {
        var command = new CreateDroneCommand() { DroneDto = request };
        var createDroneResponse = await _mediator.Send(command);
        return StatusCode((int)createDroneResponse.StatusCode, createDroneResponse);
    }

    [HttpGet]
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
}