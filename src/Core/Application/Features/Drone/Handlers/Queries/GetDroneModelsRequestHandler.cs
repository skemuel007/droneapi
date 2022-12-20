using System.Net;
using Application.Features.Drone.Request.Queries;
using Application.Responses;
using Domain.Enums;
using EnumsNET;
using MediatR;

namespace Application.Features.Drone.Handlers.Queries;

public class GetDroneModelsRequestHandler : IRequestHandler<GetDroneModelsRequest, BaseCommandResponse<object>>
{
    public async Task<BaseCommandResponse<object>> Handle(GetDroneModelsRequest request,
        CancellationToken cancellationToken)
    {
        var droneModels = Enum.GetNames(typeof(DroneModel)).Select(name => new
        {
            Id = (int)Enum.Parse(typeof(DroneModel), name),
            Name = ((DroneModel)((int)Enum.Parse(typeof(DroneModel), name))).AsString(EnumFormat.Description),
            Value = name
        }).ToArray();


        return new BaseCommandResponse<object>
        {
            Data = droneModels,
            Success = true,
            Message = "Drone models data fetched",
            StatusCode = HttpStatusCode.OK
        };
    }
}
