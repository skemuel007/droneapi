using System.Net;
using Application.Contracts.Persistence;
using Application.DTOs.Drone.Validators;
using Application.DTOs.DroneRequest;
using Application.DTOs.DroneRequest.Validators;
using Application.Features.Drone.Request.Commands;
using Application.Features.DroneRequest.Request.Commands;
using Application.Models;
using Application.Responses;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Features.DroneRequest.Handlers.Commands;

public class CreateDroneRequestCommandHandler : IRequestHandler<CreateDroneRequestCommand, BaseCommandResponse<CreateDroneRequestResponseDto>>
{
    private readonly IDroneRequestRepository _droneRequestRepository;
    private readonly IDronesRepository _dronesRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private DroneConfiguration _droneConfiguration;

    public CreateDroneRequestCommandHandler(
        IDroneRequestRepository droneRequestRepository,
        IDronesRepository dronesRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IOptions<DroneConfiguration> droneConfigurationOption)
    {
        _droneRequestRepository =
            droneRequestRepository ?? throw new ArgumentNullException(nameof(droneRequestRepository));
        _dronesRepository = dronesRepository ?? throw new ArgumentNullException(nameof(dronesRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _droneConfiguration = droneConfigurationOption.Value;
    }

    public async Task<BaseCommandResponse<CreateDroneRequestResponseDto>> Handle(CreateDroneRequestCommand command,
        CancellationToken cancellationToken)
    {
        var validator = new CreateDroneRequestDtoValidator(_dronesRepository, _droneRequestRepository);
        var validationResult = await validator.ValidateAsync(command.DroneRequestDto);

        if (validationResult.IsValid == false)
        {
            return new BaseCommandResponse<CreateDroneRequestResponseDto>()
            {
                Message = "Drone request creation validation failed",
                Errors = validationResult.Errors.Select(v => v.ErrorMessage).ToList(),
                StatusCode = HttpStatusCode.UnprocessableEntity
            };
        }

        var droneStateCheck = await _dronesRepository.FirstOrDefaultAsync(d => d.Id == command.DroneRequestDto.DroneId, disableTracking: true);
        if (droneStateCheck.State != DroneState.IDLE)
        {
            return new BaseCommandResponse<CreateDroneRequestResponseDto>()
            {
                Message = "Cannot create drone request for drone not in Idle state",
                Errors = validationResult.Errors.Select(v => v.ErrorMessage).ToList(),
                StatusCode = HttpStatusCode.UnprocessableEntity
            };
        }

        if (droneStateCheck.BatteryCapacity < _droneConfiguration.MinBatteryCapacity)
        {
            return new BaseCommandResponse<CreateDroneRequestResponseDto>()
            {
                Message = $"Cannot create drone request for drone with battery percentage less than {_droneConfiguration.MinBatteryCapacity} percent",
                Errors = validationResult.Errors.Select(v => v.ErrorMessage).ToList(),
                StatusCode = HttpStatusCode.UnprocessableEntity
            };
        }
        
        // check if drone is already requested and is engaged
        var droneRequestStatusCheck = await _droneRequestRepository.AnyAsync(d =>
            d.DroneId == command.DroneRequestDto.DroneId && d.Status == DroneRequestState.ENGAGED);

        if (droneRequestStatusCheck)
        {
            return new BaseCommandResponse<CreateDroneRequestResponseDto>()
            {
                Message = $"Drone with id {command.DroneRequestDto.DroneId} is already engaged",
                Errors = validationResult.Errors.Select(v => v.ErrorMessage).ToList(),
                StatusCode = HttpStatusCode.BadRequest
            }; 
        }

        command.DroneRequestDto.Status = DroneRequestState.ENGAGED; // set status to engaged

        var droneRequest = _mapper.Map<Domain.Entities.DroneRequest>(command.DroneRequestDto);
        droneRequest = await _droneRequestRepository.AddAsync(droneRequest);

        droneStateCheck.State = DroneState.LOADING;
        _dronesRepository.Update(droneStateCheck);

        await _unitOfWork.CompleteAsync(); // commit transaction

        var droneRequestResponseDto = _mapper.Map<CreateDroneRequestResponseDto>(droneRequest);


        return new BaseCommandResponse<CreateDroneRequestResponseDto>()
        {
            Message = "Drone request creation Successful",
            StatusCode = HttpStatusCode.Created,
            Data = droneRequestResponseDto,
            Success = true
        };
    }
}