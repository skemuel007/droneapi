using Application.DTOs.DroneRequest;
using Application.Responses;
using MediatR;

namespace Application.Features.DroneRequest.Request.Commands;

public class CreateDroneRequestCommand : IRequest<BaseCommandResponse<CreateDroneRequestResponseDto>>
{ 
    public CreateDroneRequestDto DroneDto { get; set; }
}