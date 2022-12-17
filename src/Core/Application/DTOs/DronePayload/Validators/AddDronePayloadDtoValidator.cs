using Application.Contracts.Persistence;
using Application.DTOs.DroneRequest.Validators;
using FluentValidation;

namespace Application.DTOs.DronePayload.Validators;

public class AddDronePayloadDtoValidator : AbstractValidator<AddDronePayloadDto>
{ 
    private readonly IMedicationRepository _medicationRepository; 
    private readonly IDroneRequestRepository _droneRequestRepository;
    private readonly IDronePayloadRepository _dronePayloadRepository; 
    
    public AddDronePayloadDtoValidator(
        IDronePayloadRepository dronePayloadRepository, 
        IDroneRequestRepository droneRequestRepository, 
        IMedicationRepository medicationRepository) 
    { 
        _dronePayloadRepository = dronePayloadRepository ?? throw new ArgumentNullException(nameof(dronePayloadRepository)); 
        _droneRequestRepository = droneRequestRepository ?? throw new ArgumentNullException(nameof(droneRequestRepository));
        _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));

        RuleFor(dp => dp.Quantity)
            .NotEmpty().NotEmpty().WithMessage("{PropertyName} is required.")
            .GreaterThan(0).WithMessage("{PropertyName} should be greater than 0.");
        
        RuleFor(dp => dp.MedicationId)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.")
            .MustAsync(async (medicationId, token) =>
            {
                var medicationExists = await _medicationRepository.AnyAsync(d => d.Id == medicationId );
                return medicationExists;
            }).WithMessage("Invalid medication id, please enter a valid medication id.");
        
        RuleFor(dp => dp.DroneRequestId)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.")
            .MustAsync(async (droneRequestId, token) =>
            {
                var droneRequestExists = await _droneRequestRepository.AnyAsync(d => d.Id == droneRequestId );
                return droneRequestExists;
            }).WithMessage("Invalid droneRequest id, please enter a valid droneRequest id.");
    }
}