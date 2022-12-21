using Application.Contracts.Persistence;
using FluentValidation;

namespace Application.DTOs.Drone.Validators;
public class CreateDroneDtoValidator : AbstractValidator<CreateDroneDto>
{
    private readonly IDronesRepository _dronesRepository;

    public CreateDroneDtoValidator(IDronesRepository dronesRepository)
    {
        _dronesRepository = dronesRepository ?? throw new ArgumentNullException(nameof(dronesRepository));
        RuleFor(d => d.SerialNumber)
            .NotEmpty().NotNull().WithMessage("{PropertyName} is required.")
            .Must(x => x.Length is > 0 and <= 100)
            .WithMessage("{PropertyName} number characters must be between 1 and 100 characters.")
            .MustAsync(async (serialNumber, token) =>
            {
                var droneExists = await _dronesRepository.AnyAsync(d => d.SerialNumber == serialNumber);
                return !droneExists;
            }).WithMessage("{PropertyName} exists.");
        Include(new IDroneDtoValidator());
    }
}