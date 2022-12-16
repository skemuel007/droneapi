using Application.Contracts.Persistence;
using FluentValidation;

namespace Application.DTOs.DroneRequest.Validators;

public class CreateDroneRequestDtoValidator : AbstractValidator<CreateDroneRequestDto>
{
    private readonly IDronesRepository _dronesRepository;
    private readonly IDroneRequestRepository _droneRequestRepository;
    
    public CreateDroneRequestDtoValidator(IDronesRepository dronesRepository,
        IDroneRequestRepository droneRequestRepository)
    {
        _dronesRepository = dronesRepository ?? throw new ArgumentNullException(nameof(dronesRepository));
        _droneRequestRepository = droneRequestRepository ?? throw new ArgumentNullException(nameof(droneRequestRepository));
        
        RuleFor(d => d.DroneId)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.")
            .MustAsync(async (droneId, token) =>
            {
                var droneExists = await _dronesRepository.AnyAsync(d => d.Id == droneId);
                return droneExists;
            }).WithMessage("Invalid drone id, please enter a valid drone id.");

        RuleFor(d => d.DroneRequestCode)
            .MustAsync(async (droneRequestCode, token) =>
            {
                var droneRequestCodeAlreadyUsed =
                    await _droneRequestRepository.AnyAsync(d => d.DroneRequestCode == droneRequestCode);
                return !droneRequestCodeAlreadyUsed;
            }).WithMessage("Request code already used for another drone request");

        Include(new IDroneRequestDtoValidator());
    }
}