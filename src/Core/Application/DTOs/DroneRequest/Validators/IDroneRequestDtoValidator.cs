using FluentValidation;

namespace Application.DTOs.DroneRequest.Validators;

public class IDroneRequestDtoValidator : AbstractValidator<IDroneRequestDto>
{
    public IDroneRequestDtoValidator()
    {
        RuleFor(d => d.Origin)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.");

        RuleFor(d => d.Destination)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.");
        
        RuleFor(d => d.Status).IsInEnum().WithMessage("Please pass a valid status value.");

        RuleFor(d => d.DroneRequestCode)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.");
    }
}