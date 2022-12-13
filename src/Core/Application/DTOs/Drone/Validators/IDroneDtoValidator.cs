using Application.Contracts.Persistence;
using FluentValidation;

namespace Application.DTOs.Drone.Validators;

public class IDroneDtoValidator : AbstractValidator<IDroneDto>
{
    public IDroneDtoValidator()
    {

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