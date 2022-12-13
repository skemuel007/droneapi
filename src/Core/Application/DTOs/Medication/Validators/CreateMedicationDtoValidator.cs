using Application.Contracts.Persistence;
using FluentValidation;

namespace Application.DTOs.Medication.Validators;

public class CreateMedicationDtoValidator : AbstractValidator<CreateMedicationDto>
{
    private readonly IMedicationRepository _medicationRepository;
    public CreateMedicationDtoValidator(IMedicationRepository medicationRepository)
    {
        _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));
        RuleFor(d => d.Code)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.")
            .Matches(@"^([A-Z]|[0-9]|[_])+$").WithMessage("Please use only uppercase letters, numbers and underscore for {PropertyName} property.")
            .MustAsync(async (code, token) =>
            {
                var medicationExists = await _medicationRepository.AnyAsync(d => d.Code == code);
                return !medicationExists;
            }).WithMessage("{PropertyName} exists.");
        Include(new IMedicationDtoValidator());
    }
}