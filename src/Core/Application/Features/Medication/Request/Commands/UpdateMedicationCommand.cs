using Application.DTOs.Drone;
using Application.DTOs.Medication;
using Application.Responses;
using MediatR;

namespace Application.Features.Medication.Request.Commands;

public class UpdateMedicationCommand : IRequest<BaseCommandResponse>
{
    public UpdateMedicationDto UpdateMedicationDto { get; set; }
}