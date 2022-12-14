using Application.Responses;
using MediatR;

namespace Application.Features.Medication.Request.Commands;

public class DeleteMedicationCommand: IRequest<BaseCommandResponse>
{
    public Guid Id { get; set; }
}

