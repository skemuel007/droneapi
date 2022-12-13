using Application.Contracts.Persistence;
using FluentValidation;

namespace Application.DTOs.Drone.Validators;

public class IDroneDtoValidator : AbstractValidator<IDroneDto>
{
    private readonly IDronesRepository _dronesRepository;

    public IDroneDtoValidator(IDronesRepository dronesRepository)
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

        RuleFor(d => d.Model).IsInEnum();
        RuleFor(d => d.State).IsInEnum();
        
        RuleFor(d => d.WeightLimit)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0")
            .LessThanOrEqualTo(500).WithMessage("{PropertyName} must be less than or equal to 100");

        RuleFor(d => d.BatteryCapacity)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("{PropertyName} must be less than or equal to 100");


    }
}