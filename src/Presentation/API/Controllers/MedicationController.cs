using System.Net;
using Application.DTOs.Common;
using Application.DTOs.Medication;
using Application.Features.Medication.Request.Commands;
using Application.Features.Medication.Request.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiVersion("1.0")]
public class MedicationController : BaseController
{
    private IMediator _mediator;
    public MedicationController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    /// <summary>
    /// Add a new medication
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost(Name = "AddMedication")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> CreateMedication([FromBody] CreateMedicationDto request)
    {
        var command = new CreateMedicationCommand() { CreateMedication = request };
        var response = await _mediator.Send(command);
        return StatusCode((int)response.StatusCode, response);
    }
    
    /// <summary>
    /// Get Medication
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet(Name = "GetMedication")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetDroneById([FromQuery] MedicationDetailsParamsDto request )
    {

        var query = new GetMedicationDetailRequest()
        {
            MedicationDetailsParams = request
        };
        var response = await _mediator.Send(query);

        return StatusCode((int)response.StatusCode, response);
    }
    
    /// <summary>
    /// Get medication list
    /// </summary>
    /// <param name="queryParams"></param>
    /// <returns></returns>
    [HttpGet(Name = "MedicationList")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Get([FromQuery] PaginatedQueryParams queryParams)
    {
        var response = await _mediator.Send(new GetMedicationListRequest()
        {
            QueryParams = new PaginateQueryRequest<Medication>
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