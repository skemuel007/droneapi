using System.Net;
using Application.Contracts.Persistence;
using Application.DTOs.Drone;
using Application.Features.Drone.Request.Queries;
using Application.Responses;
using AutoMapper;
using MediatR;

namespace Application.Features.Drone.Handlers.Queries;

public class GetDroneDetailsRequestHandler : IRequestHandler<GetDroneDetailsRequest, BaseCommandResponse<DroneDto>>
{
    private readonly IDronesRepository _dronesRepository;
    private readonly IMapper _mapper;

    public GetDroneDetailsRequestHandler(IDronesRepository dronesRepository,
        IMapper mapper)
    {
        _dronesRepository = dronesRepository ?? throw new ArgumentNullException(nameof(dronesRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<BaseCommandResponse<DroneDto>> Handle(GetDroneDetailsRequest request,
        CancellationToken cancellationToken)
    {
        var drone = await _dronesRepository.GetByIdAsync(request.Id);

        if (drone == null)
        {
            return new BaseCommandResponse<DroneDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Success = false,
                Message = "No such drone record"
            };
        }

        var droneDto = _mapper.Map<DroneDto>(drone);

        return new BaseCommandResponse<DroneDto>()
        {
            Success = true,
            Data = droneDto,
            Message = "Drone record found",
            StatusCode = HttpStatusCode.OK
        };
    }

}