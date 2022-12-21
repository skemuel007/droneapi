using System.Net;
using Application.Contracts.Persistence;
using Application.Features.Drone.Request.Queries;
using Application.Features.Medication.Request.Queries;
using Application.Responses;
using MediatR;

namespace Application.Features.Medication.Handlers.Queries;
public class GetMedicationListRequestHandler : IRequestHandler<GetMedicationListRequest, Paginated<Domain.Entities.Medication>>
{
    private readonly IMedicationRepository _medicationRepository;

    public GetMedicationListRequestHandler(
        IMedicationRepository medicationRepository)
    {
        _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));
    }

    public async Task<Paginated<Domain.Entities.Medication>> Handle(GetMedicationListRequest request,
        CancellationToken cancellationToken)
    {
        var medications = await _medicationRepository.GetWherePaginated(request.QueryParams);
        if (medications.TotalCount > 0)
        {
            medications.Success = true;
            medications.StatusCode = HttpStatusCode.OK;
        }
        else
        {
            medications.StatusCode = HttpStatusCode.NotFound;
        }

        return medications;
    }

}