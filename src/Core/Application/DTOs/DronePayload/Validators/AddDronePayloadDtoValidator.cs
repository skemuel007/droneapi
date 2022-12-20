using Application.Contracts.Persistence;
using Application.DTOs.DroneRequest.Validators;
using FluentValidation;

namespace Application.DTOs.DronePayload.Validators;

public class AddDronePayloadDtoValidator : AbstractValidator<AddDronePayloadDto>
{ 
    private readonly IMedicationRepository _medicationRepository; 
    private readonly IDroneRequestRepository _droneRequestRepository;
    
    public AddDronePayloadDtoValidator(
        IDroneRequestRepository droneRequestRepository, 
        IMedicationRepository medicationRepository) 
    { 
        _droneRequestRepository = droneRequestRepository ?? throw new ArgumentNullException(nameof(droneRequestRepository));
        _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));

        RuleFor(dp => dp.Quantity)
            .NotEmpty().NotNull().WithMessage("{PropertyName} is required.")
            .GreaterThan(0).WithMessage("{PropertyName} should be greater than 0.");

        When(dp => !dp.MedicationId.HasValue && dp.MedicationIds?.Count <= 0, () =>
        {
            RuleFor(dp => dp.MedicationId)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.");

            RuleForEach(dp => dp.MedicationIds)
           .NotNull().WithMessage("{PropertyName} property is required.");

        });

        When(dp => dp.MedicationId.HasValue, () =>
        {
            RuleFor(dp => dp.MedicationId)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.")
            .MustAsync(async (medicationId, token) =>
            {
                var medicationExists = await _medicationRepository.AnyAsync(d => d.Id == medicationId);
                return medicationExists;
            }).WithMessage("Invalid medication id, please enter a valid medication id.");
        });

        When(dp => dp.MedicationIds?.Count > 0, () =>
        {
            RuleForEach(dp => dp.MedicationIds)
            .NotNull().WithMessage("{PropertyName} property is required.")
            .MustAsync(async (medicationId, token) =>
            {
                var medicationExists = await _medicationRepository.AnyAsync(d => d.Id == medicationId);
                return medicationExists;
            }).WithMessage("Invalid medication id, please enter a valid medication id at {CollectionINdex}.");
        });
        
        RuleFor(dp => dp.DroneRequestId)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.")
            .MustAsync(async (droneRequestId, token) =>
            {
                var droneRequestExists = await _droneRequestRepository.AnyAsync(d => d.Id == droneRequestId );
                return droneRequestExists;
            }).WithMessage("Invalid droneRequest id, please enter a valid droneRequest id.");
    }
}
