using System.Linq.Expressions;
using System.Net;
using Application.Contracts.Persistence;
using Application.DTOs.Drone.Validators;
using Application.DTOs.DronePayload;
using Application.DTOs.DronePayload.Validators;
using Application.Features.DronePayload.Request.Commands;
using Application.Models;
using Application.Responses;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;

namespace Application.Features.DronePayload.Handlers.Commands;

public class AddDronePayloadCommandHandler : IRequestHandler<AddDronePayloadCommand, BaseCommandResponse<AddedDronePayloadResponseDto>>
{
    private readonly IMedicationRepository _medicationRepository; 
    private readonly IDroneRequestRepository _droneRequestRepository;
    private readonly IDronePayloadRepository _dronePayloadRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DroneConfiguration _droneConfiguration;
    private readonly ILogger<AddDronePayloadCommandHandler> _logger;

    public AddDronePayloadCommandHandler(
        IDronePayloadRepository dronePayloadRepository, 
        IDroneRequestRepository droneRequestRepository, 
        IMedicationRepository medicationRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IOptions<DroneConfiguration> droneConfigurationOption,
        ILogger<AddDronePayloadCommandHandler> logger)
    {
        _dronePayloadRepository = dronePayloadRepository ?? throw new ArgumentNullException(nameof(dronePayloadRepository)); 
        _droneRequestRepository = droneRequestRepository ?? throw new ArgumentNullException(nameof(droneRequestRepository));
        _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _droneConfiguration = droneConfigurationOption.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<BaseCommandResponse<AddedDronePayloadResponseDto>> Handle(AddDronePayloadCommand command, CancellationToken cancellationToken)
    {
        var validator = new AddDronePayloadDtoValidator(_droneRequestRepository, _medicationRepository);
        var validationResult = await validator.ValidateAsync(command.DronePayloadDto);

        if (validationResult.IsValid == false)
        {
            var errorMessage = "Drone payload creation validation failed";
            var validationErrors = validationResult.Errors.Select(v => v.ErrorMessage).ToList();

            _logger.LogError($"{errorMessage} with validation errors {validationErrors}");

            return new BaseCommandResponse<AddedDronePayloadResponseDto>()
            {
                Message = $"{errorMessage}.",
                Errors = validationErrors,
                StatusCode = HttpStatusCode.UnprocessableEntity
            };
        }

        var droneRequestIncludes = new List<Expression<Func<Domain.Entities.DroneRequest, object>>> { x => x.Drone };

        var droneRequestDetails = await _droneRequestRepository.FirstOrDefaultAsync(
            predicate: (d => d.Id == command.DronePayloadDto.DroneRequestId),
            includes: droneRequestIncludes);
        
        // validate drone battery capacity
        if (droneRequestDetails.Drone.State == DroneState.LOADING &&
                droneRequestDetails.Drone.BatteryCapacity < _droneConfiguration.MinBatteryCapacity)
        {
            var errorMessage = $"Drone battery capacity is less than {_droneConfiguration.MinBatteryCapacity} percent";
            _logger.LogError(errorMessage);

            return new BaseCommandResponse<AddedDronePayloadResponseDto>()
            {
                Message = errorMessage,
                StatusCode = HttpStatusCode.BadRequest
            };
        }
        
        // check drone weight: step 1 get already saved request
        var existingDronePayload = await _dronePayloadRepository.FirstOrDefaultAsync(
            predicate: (d => d.DroneRequestId == command.DronePayloadDto.DroneRequestId 
                             && d.MedicationId == command.DronePayloadDto.MedicationId), disableTracking: false);

        decimal totalPayloadWeight = 0m;
        
        if (existingDronePayload != null)
        {
            existingDronePayload.Quantity += command.DronePayloadDto.Quantity;
            totalPayloadWeight += existingDronePayload.Quantity * existingDronePayload.Medication.Weight;
            
            // validate drone total weight
            if (totalPayloadWeight > droneRequestDetails.Drone.WeightLimit)
            {
                var errorMessage = $"Total payload {totalPayloadWeight} for this request exceeds drone capacity: {droneRequestDetails.Drone.WeightLimit}";
                _logger.LogError(errorMessage);
                return new BaseCommandResponse<AddedDronePayloadResponseDto>()
                {
                    Message = errorMessage,
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
            
            await _unitOfWork.CompleteAsync(); // update drone quantity
            var existingDronePayloadResponse = _mapper.Map<AddedDronePayloadResponseDto>(existingDronePayload);

            _logger.LogInformation("Payload was successfully added to drone");

            return new BaseCommandResponse<AddedDronePayloadResponseDto>()
            {
                Message = "Drone payload Successful",
                StatusCode = HttpStatusCode.Created,
                Data = existingDronePayloadResponse,
                Success = true
            };
        }
        else
        {
            var droneMedication =
                await _medicationRepository.FirstOrDefaultAsync(m => m.Id == command.DronePayloadDto.MedicationId);
            totalPayloadWeight += command.DronePayloadDto.Quantity * droneMedication.Weight;
        }

        // validate drone total weight
        if (totalPayloadWeight > droneRequestDetails.Drone.WeightLimit)
        {
            return new BaseCommandResponse<AddedDronePayloadResponseDto>()
            {
                Message = $"Total payload {totalPayloadWeight} for this request exceeds drone capacity: {droneRequestDetails.Drone.WeightLimit}",
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        var dronePayload = _mapper.Map<Domain.Entities.DronePayload>(command.DronePayloadDto);
        dronePayload = await _dronePayloadRepository.AddAsync(dronePayload);

        var dronePayloadResponse = _mapper.Map<AddedDronePayloadResponseDto>(dronePayload);
        
        await _unitOfWork.CompleteAsync();

        return new BaseCommandResponse<AddedDronePayloadResponseDto>()
        {
            Message = "Drone payload Successful",
            StatusCode = HttpStatusCode.Created,
            Data = dronePayloadResponse,
            Success = true
        };
    }
}