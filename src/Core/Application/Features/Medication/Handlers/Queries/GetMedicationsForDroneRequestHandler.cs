using System.Net;
using Application.Contracts.Persistence;
using Application.Features.Medication.Request.Queries;
using Application.Responses;
using MediatR;

namespace Application.Features.Medication.Handlers.Queries;

public class GetMedicationsForDroneRequestHandler : IRequestHandler<GetMedicationsForDroneRequest, BaseCommandResponse<IEnumerable<Domain.Entities.Medication>>>
{
    private readonly IMedicationRepository _medicationRepository;

    public GetMedicationsForDroneRequestHandler(
        IMedicationRepository medicationRepository)
    {
        _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));
    }

    public async Task<BaseCommandResponse<IEnumerable<Domain.Entities.Medication>>> Handle(GetMedicationsForDroneRequest request,
        CancellationToken cancellationToken)
    {
        var medications = await _medicationRepository.GetLoadedItems(request.DroneId);
        if (medications != null)
        {
            return new BaseCommandResponse<IEnumerable<Domain.Entities.Medication>>()
            {
                Success = true,
                StatusCode = HttpStatusCode.OK,
                Data = medications
            };
        }
        else
        {
            return new BaseCommandResponse<IEnumerable<Domain.Entities.Medication>>()
            {
                Success = false,
                StatusCode = HttpStatusCode.NotFound
            }; 
        }
    }
}
