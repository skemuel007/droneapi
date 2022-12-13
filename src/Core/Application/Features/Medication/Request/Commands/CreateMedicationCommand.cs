using Application.DTOs.Drone;
using Application.DTOs.Medication;
using Application.Responses;
using MediatR;

namespace Application.Features.Medication.Request.Commands;

public class CreateMedicationCommand : IRequest<BaseCommandResponse<Domain.Entities.Medication>>
{
    public CreateMedicationDto CreateMedication { get; set; }
}