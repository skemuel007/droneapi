using System.Net;
using Application.Contracts.Persistence;
using Application.DTOs.Drone;
using Application.DTOs.Drone.Validators;
using Application.Features.Drone.Request.Queries;
using Application.Responses;
using AutoMapper;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
        var validator = new DroneDetailsRequestDtoValidator(_dronesRepository);
        var validationResult = await validator.ValidateAsync(request.DroneDetailsRequestDto);

        if (validationResult.IsValid == false)
        {
            return new BaseCommandResponse<DroneDto>()
            {
                Message = "Drone creation validation failed",
                Errors = validationResult.Errors.Select(v => v.ErrorMessage).ToList(),
                StatusCode = HttpStatusCode.UnprocessableEntity
            };
        }
        var drone = await _dronesRepository.FirstOrDefaultAsync(d => d.SerialNumber == request.DroneDetailsRequestDto.SerialNumber || d.Id == request.DroneDetailsRequestDto.Id);

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