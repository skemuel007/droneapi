using System.Net;
using Application.Contracts.Persistence;
using Application.DTOs.Common;
using Application.Features.Drone.Request.Queries;
using Application.Responses;
using Domain.Entities;
using MediatR;

namespace Application.Features.Drone.Handlers.Queries;

public class GetAvailableDronesForLoadingRequestHandler : IRequestHandler<GetAvailableDronesForLoadingRequest, Paginated<Domain.Entities.Drone>>
{
    private readonly IDronesRepository _dronesRepository;
    public GetAvailableDronesForLoadingRequestHandler(
        IDronesRepository dronesRepository)
    {
        _dronesRepository = dronesRepository ?? throw new ArgumentNullException(nameof(dronesRepository));
    }

    public async Task<Paginated<Domain.Entities.Drone>> Handle(GetAvailableDronesForLoadingRequest request,
        CancellationToken cancellationToken)
    {

        var drones = await _dronesRepository.GetPaginated(new PaginateQueryRequestNew<Domain.Entities.Drone>
        {
            Predicate = (d => d.State == Domain.Enums.DroneState.IDLE || d.State == Domain.Enums.DroneState.LOADING)
        });

        if (drones.TotalCount > 0)
        {
            drones.Success = true;
            drones.StatusCode = HttpStatusCode.OK;
        }

        return drones;
    }
}
