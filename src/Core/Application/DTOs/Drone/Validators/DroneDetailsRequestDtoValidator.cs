using Application.Contracts.Persistence;
using FluentValidation;

namespace Application.DTOs.Drone.Validators;

public class DroneDetailsRequestDtoValidator : AbstractValidator<DroneDetailsRequestDto>
{
    private readonly IDronesRepository _dronesRepository;

    public DroneDetailsRequestDtoValidator(IDronesRepository dronesRepository)
    {
        _dronesRepository = dronesRepository ?? throw new ArgumentNullException(nameof(dronesRepository));

        When(d => string.IsNullOrEmpty(d.SerialNumber) && (d.Id == null || d.Id == Guid.Empty), () =>
        {
            RuleFor(d => d.SerialNumber)
            .NotEmpty().NotNull().WithMessage("{PropertyName} is required.");
            RuleFor(d => d.Id)
            .NotEmpty().NotNull().WithMessage("{PropertyName} is required.");
        });

        When(d => !string.IsNullOrEmpty(d.SerialNumber), () =>
        {
            
            RuleFor(d => d.SerialNumber)
                .NotEmpty().NotNull().WithMessage("{PropertyName} is required.")
                .Must(x => x.Length is > 0 and <= 100)
                .WithMessage("{PropertyName} number characters must be between 1 and 100 characters.")
                .MustAsync(async (serialNumber, token) =>
                {
                    var droneExists = await _dronesRepository.AnyAsync(d => d.SerialNumber == serialNumber);
                    return droneExists;
                }).WithMessage("Drone with {PropertyName} does not exist.");
        });

        When(d => (d.Id.HasValue || d.Id != Guid.Empty), () =>
        {

            RuleFor(d => d.Id)
                .NotEmpty().NotNull().WithMessage("{PropertyName} is required.")
                .MustAsync(async (Id, token) =>
                {
                    var droneExists = await _dronesRepository.AnyAsync(d => d.Id == Id);
                    return droneExists;
                }).WithMessage("Drone with {PropertyName} does not exist");
        });
    }
}
