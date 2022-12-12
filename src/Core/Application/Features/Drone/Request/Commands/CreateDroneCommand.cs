using Application.DTOs.Drone;
using Application.Responses;
using MediatR;

namespace Application.Features.Drone.Request.Commands;

public class CreateDroneCommand : IRequest<BaseCommandResponse<Domain.Entities.Drone>>
{
    public CreateDroneDto DroneDto { get; set; }
}