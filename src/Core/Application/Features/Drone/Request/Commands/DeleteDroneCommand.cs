using Application.Responses;
using MediatR;

namespace Application.Features.Drone.Request.Commands;

public class DeleteDroneCommand : IRequest<BaseCommandResponse>
{
    public Guid Id { get; set; }
}