using Application.DTOs.DronePayload;
using Application.Responses;
using MediatR;

namespace Application.Features.DronePayload.Request.Commands;

public class AddDronePayloadCommand: IRequest<BaseCommandResponse<AddedDronePayloadResponseDto>>
{
    public AddDronePayloadDto DronePayloadDto { get; set; }
}

public class LoadDroneWithPayloadCommand : IRequest<BaseCommandResponse<LoadedPayloadResponseDto>>
{
    public LoadDroneWithItemDto DronePayloadDto { get; set; }
}