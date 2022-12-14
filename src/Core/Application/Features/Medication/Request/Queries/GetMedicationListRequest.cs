using Application.DTOs.Common;
using Application.Responses;
using MediatR;

namespace Application.Features.Medication.Request.Queries;

public class GetMedicationListRequest : IRequest<Paginated<Domain.Entities.Medication>>
{
    public PaginateQueryRequest<Domain.Entities.Medication> QueryParams { get; set; }
}