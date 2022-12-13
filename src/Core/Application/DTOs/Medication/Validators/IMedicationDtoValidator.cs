using FluentValidation;

namespace Application.DTOs.Medication.Validators;

public class IMedicationDtoValidator : AbstractValidator<IMedicationDto>
{
    public IMedicationDtoValidator()
    {
        RuleFor(m => m.Name)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.")
            .Matches(@"^([a-z]|[A-Z]|[0-9]|[_]|[-])+$")
            .WithMessage("Please use only letters, numbers, hyphen and underscore for {PropertyName} property.");

        RuleFor(m => m.Weight)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.")
            .GreaterThan(0).WithMessage("{PropertyName} cannot be less than zero.");
    }
}