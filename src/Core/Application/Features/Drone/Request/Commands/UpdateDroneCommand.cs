using Application.DTOs.Drone;
using Application.Responses;
using MediatR;

namespace Application.Features.Drone.Request.Commands;

public class UpdateDroneCommand : IRequest<BaseCommandResponse>
{
    public UpdateDroneDto UpdateDroneDto { get; set; }
}