using System;
using Application.Features.Drone.Request.Queries;
using Application.Responses;
using Domain.Enums;
using EnumsNET;
using MediatR;
using System.Net;
using Application.Features.DroneRequest.Request.Queries;

namespace Application.Features.DroneRequest.Handlers.Queries
{
	public class GetDroneRequestStateRequestHandler : IRequestHandler<GetDroneRequestStateRequest, BaseCommandResponse<object>>
    {
        public async Task<BaseCommandResponse<object>> Handle(GetDroneRequestStateRequest request,
            CancellationToken cancellationToken)
        {
            var droneRequestState = Enum.GetNames(typeof(DroneRequestState)).Select(name => new
            {
                Id = (int)Enum.Parse(typeof(DroneRequestState), name),
                Name = ((DroneRequestState)((int)Enum.Parse(typeof(DroneRequestState), name))).AsString(EnumFormat.Description),
                Value = name
            }).ToArray();


            return new BaseCommandResponse<object>
            {
                Data = droneRequestState,
                Success = true,
                Message = "Drone request state data fetched",
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}

