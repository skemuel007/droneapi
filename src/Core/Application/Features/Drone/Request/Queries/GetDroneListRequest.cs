using Application.DTOs.Common;
using Application.Responses;
using MediatR;

namespace Application.Features.Drone.Request.Queries;

public class GetDroneListRequest : IRequest<Paginated<Domain.Entities.Drone>>
{
    public PaginateQueryRequest<Domain.Entities.Drone> QueryParams { get; set; }
}