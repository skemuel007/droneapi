using System;
using Application.Responses;
using MediatR;

namespace Application.Features.DroneRequest.Request.Queries
{
	public class GetDroneRequestStateRequest : IRequest<BaseCommandResponse<object>>
	{
	}
}

