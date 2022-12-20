using System.Net;
using Application.Features.Drone.Request.Queries;
using Application.Responses;
using Domain.Enums;
using EnumsNET;
using MediatR;

namespace Application.Features.Drone.Handlers.Queries;

public class GetDroneStatesRequestHandler : IRequestHandler<GetDroneStatesRequest, BaseCommandResponse<object>>
{
    public async Task<BaseCommandResponse<object>> Handle(GetDroneStatesRequest request,
        CancellationToken cancellationToken)
    {
        var droneStates = Enum.GetNames(typeof(DroneState)).Select(name => new
        {
            Id = (int)Enum.Parse(typeof(DroneState), name),
            Name = ((DroneState)((int)Enum.Parse(typeof(DroneState), name))).AsString(EnumFormat.Description),
            Value = name
        }).ToArray();


        return new BaseCommandResponse<object>
        {
            Data = droneStates,
            Success = true,
            Message = "Drone state data fetched",
            StatusCode = HttpStatusCode.OK
        };
    }
}
