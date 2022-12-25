using System.Net;
using Application.DTOs.Common;
using Application.DTOs.Medication;
using Application.Features.Medication.Request.Commands;
using Application.Features.Medication.Request.Queries;
using Application.Responses;
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
    /// <returns>A newly created medication</returns>
    /// <remarks>
    /// Sample request :
    ///
    ///     POST /api/v1/medication
    ///     {
    ///         "name": "",
    ///         "weight": 0,
    ///         "code": "",
    ///         "image": ""
    ///     }
    /// </remarks>
    /// <returns code="201">Returns a newly created medication</returns>
    /// <returns code="500">Internal server error</returns>
    [HttpPost(Name = "AddMedication")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// <returns>Medication details by Id</returns>
    /// <returns code="200">Medication details by Id</returns>
    /// <returns code="404">Medication details not found</returns>
    [HttpGet(Name = "GetMedication")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseCommandResponse<Medication>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseCommandResponse))]
    public async Task<IActionResult> GetMedicationId([FromQuery] MedicationDetailsParamsDto request )
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
    [HttpGet("list", Name = "MedicationList")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetMedications([FromQuery] PaginatedQueryParams queryParams)
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
    
    /// <summary>
    /// Update medication
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut(Name = "UpdateMedication")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateMedication([FromBody] UpdateMedicationDto request)
    {
        var response = await _mediator.Send(new UpdateMedicationCommand()
        {
            UpdateMedicationDto = request
        });

        return StatusCode((int)response.StatusCode, response);
    }
    
    /// <summary>
    /// Delete medication
    /// </summary>
    /// <param name="medicationId"></param>
    /// <returns></returns>
    [HttpDelete("{medicationId}", Name = "DeleteMedication")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMedication(Guid medicationId)
    {
        var command = new DeleteMedicationCommand() { Id = medicationId };
        var response = await _mediator.Send(command);

        return StatusCode((int)response.StatusCode, response);
    }
}