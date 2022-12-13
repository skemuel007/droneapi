using Application.DTOs.Medication;
using Application.Responses;
using MediatR;

namespace Application.Features.Medication.Request.Queries;

public class GetMedicationDetailRequest : IRequest<BaseCommandResponse<MedicationDto>>
{
    public MedicationDetailsParamsDto MedicationDetailsParams { get; set; }
}