using Application.Responses;
using MediatR;

namespace Application.Features.Drone.Request.Queries;

public class GetAvailableDronesForLoadingRequest : IRequest<Paginated<Domain.Entities.Drone>>
{

}