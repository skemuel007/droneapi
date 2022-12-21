using System.Linq.Expressions;
using System.Net;
using Application.Contracts.Persistence;
using Application.DTOs.Drone.Validators;
using Application.DTOs.DronePayload;
using Application.DTOs.DronePayload.Validators;
using Application.DTOs.DroneRequest;
using Application.Features.DronePayload.Request.Commands;
using Application.Models;
using Application.Responses;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        if (command.DronePayloadDto.MedicationId.HasValue &&
            command.DronePayloadDto.Payload?.Count > 0)
        {
            var errorMessage = $"Either load a single item or multiple item but not both";
            _logger.LogError(errorMessage);
            return new BaseCommandResponse<AddedDronePayloadResponseDto>()
            {
                Message = errorMessage,
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        if ( command.DronePayloadDto.MedicationId.HasValue )
        {
            return await DoSinglePayloadAdd(command, droneRequestDetails);
        }

        if(command.DronePayloadDto.Payload?.Count > 0) {

            return await DoMultiPayloadAdd(command, droneRequestDetails);
        }

        var message = "Invalid operation";
        _logger.LogError(message);
        return new BaseCommandResponse<AddedDronePayloadResponseDto>()
        {
            Message = message,
            StatusCode = HttpStatusCode.InternalServerError,
            Data = null,
            Success = true
        };


    }

    public async Task<BaseCommandResponse<AddedDronePayloadResponseDto>> DoSinglePayloadAdd(AddDronePayloadCommand command, Domain.Entities.DroneRequest droneRequestDetails)
    {
        _logger.LogInformation($"Adding single payload for drone with details {command.DronePayloadDto}");
        // check drone weight: step 1 get already saved request

        var medicationIncludes = new List<Expression<Func<Domain.Entities.DronePayload, object>>> { x => x.Medication };

        var existingDronePayload = await _dronePayloadRepository.FirstOrDefaultAsync(
            predicate: (d => d.DroneRequestId == command.DronePayloadDto.DroneRequestId
                             && d.MedicationId == command.DronePayloadDto.MedicationId), includes: medicationIncludes, disableTracking: false);

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
            Message = "Drone payload added Successfully",
            StatusCode = HttpStatusCode.Created,
            Data = dronePayloadResponse,
            Success = true
        };
    }

    public async Task<BaseCommandResponse<AddedDronePayloadResponseDto>> DoMultiPayloadAdd(AddDronePayloadCommand command, Domain.Entities.DroneRequest droneRequestDetails)
    {
        _logger.LogInformation($"Adding multiple payloads for drone with details {command.DronePayloadDto}");
        // calculate drone weight
        decimal totalPayloadWeight = 0.0m;

        

        var medicationIncludes = new List<Expression<Func<Domain.Entities.DronePayload, object>>> { x => x.Medication };

        var existingDronePayloads = await _dronePayloadRepository.GetAsEnumerableAsync(
           predicate: (d => d.DroneRequestId == command.DronePayloadDto.DroneRequestId), includes: medicationIncludes, disableTracking: false);

        if (existingDronePayloads.Any())
        {
            // calculate total weight of existing
            totalPayloadWeight += existingDronePayloads.Sum(x => x.Quantity * x.Medication.Weight);
        }

        // calculate existing weight of drone + new payload data for drone request
        foreach (var medicationPayload in command.DronePayloadDto.Payload)
        {

            var existingDronePayload = await _dronePayloadRepository.FirstOrDefaultAsync(
            predicate: (d => d.DroneRequestId == command.DronePayloadDto.DroneRequestId
                             && d.MedicationId == medicationPayload.MedicationId), includes: medicationIncludes);


            if (existingDronePayload != null)
            {
                existingDronePayload.Quantity += medicationPayload.Quantity;
                totalPayloadWeight += existingDronePayload.Quantity * existingDronePayload.Medication.Weight;

            } else
            {
                var droneMedication =
                  await _medicationRepository.FirstOrDefaultAsync(m => m.Id == medicationPayload.MedicationId);
                totalPayloadWeight += medicationPayload.Quantity * droneMedication.Weight;
            }

        }

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


        foreach (var medicationPayload in command.DronePayloadDto.Payload)
        {
            var existingDronePayload = await _dronePayloadRepository.FirstOrDefaultAsync(
            predicate: (d => d.DroneRequestId == command.DronePayloadDto.DroneRequestId
                             && d.MedicationId == medicationPayload.MedicationId), disableTracking: false);

            if (existingDronePayload != null)
            {
                existingDronePayload.Quantity += medicationPayload.Quantity;
                _dronePayloadRepository.Update(existingDronePayload);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Successfully updated medication quantity {medicationPayload.MedicationId} for drone request {command.DronePayloadDto.DroneRequestId}");

            }
            else
            {
                
                var dronePayload = await _dronePayloadRepository.AddAsync(new Domain.Entities.DronePayload
                {
                    MedicationId = medicationPayload.MedicationId.Value,
                    DroneRequestId = command.DronePayloadDto.DroneRequestId,
                    Quantity = medicationPayload.Quantity,
                    UpdatedAt= DateTime.UtcNow,
                    CreatedAt= DateTime.UtcNow,
                });

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Successfully added medication {medicationPayload.MedicationId} for drone request {command.DronePayloadDto.DroneRequestId}");
            }
        }

        var dronePayloadResponse = _mapper.Map<AddedDronePayloadResponseDto>(command.DronePayloadDto);

        return new BaseCommandResponse<AddedDronePayloadResponseDto>()
        {
            Message = "Drone payload added Successfully",
            StatusCode = HttpStatusCode.Created,
            Data = dronePayloadResponse,
            Success = true
        };


    }
}
