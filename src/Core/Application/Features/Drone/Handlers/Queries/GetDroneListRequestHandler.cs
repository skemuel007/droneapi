using System.Net;
using Application.Contracts.Persistence;
using Application.Features.Drone.Request.Queries;
using Application.Responses;
using AutoMapper;
using MediatR;

namespace Application.Features.Drone.Handlers.Queries;
public class GetDroneListRequestHandler : IRequestHandler<GetDroneListRequest, Paginated<Domain.Entities.Drone>>
{

    private readonly IDronesRepository _dronesRepository;
    public GetDroneListRequestHandler(
        IDronesRepository dronesRepository)
    {
        _dronesRepository = dronesRepository ?? throw new ArgumentNullException(nameof(dronesRepository));
    }

    public async Task<Paginated<Domain.Entities.Drone>> Handle(GetDroneListRequest request,
        CancellationToken cancellationToken)
    {
        var drones = await _dronesRepository.GetWherePaginated(request.QueryParams);
        if (drones.TotalCount > 0)
        {
            drones.Success = true;
            drones.StatusCode = HttpStatusCode.OK;
        }

        return drones;
    }

}