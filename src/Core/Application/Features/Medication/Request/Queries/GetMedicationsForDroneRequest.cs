using Application.Responses;
using MediatR;

namespace Application.Features.Medication.Request.Queries;

public class GetMedicationsForDroneRequest : IRequest<BaseCommandResponse<IEnumerable<Domain.Entities.Medication>>>
{
    public Guid DroneId { get; set; }
}