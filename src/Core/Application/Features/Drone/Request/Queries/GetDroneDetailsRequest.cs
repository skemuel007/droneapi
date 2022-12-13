using Application.DTOs.Drone;
using Application.Responses;
using MediatR;

namespace Application.Features.Drone.Request.Queries;

public class GetDroneDetailsRequest : IRequest<BaseCommandResponse<DroneDto>>
{
    public Guid Id { get; set; }
}