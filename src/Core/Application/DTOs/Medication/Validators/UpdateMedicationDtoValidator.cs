using Application.Contracts.Persistence;
using FluentValidation;

namespace Application.DTOs.Medication.Validators;

public class UpdateMedicationDtoValidator : AbstractValidator<UpdateMedicationDto>
{
    private readonly IMedicationRepository _medicationRepository;

    public UpdateMedicationDtoValidator(IMedicationRepository medicationRepository)
    {
        _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));
        RuleFor(d => d.Code)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.")
            .Matches(@"^([A-Z]|[0-9]|[_])+$")
            .WithMessage("Please use only uppercase letters, numbers and underscore for {PropertyName} property.");

        RuleFor(d => d.Id)
            .NotEmpty().NotEmpty().WithMessage("{PropertyName} is required.")
            .MustAsync(async (id, token) =>
            {
                var medicationExists = await _medicationRepository.AnyAsync(d => d.Id == id);
                return medicationExists;
            }).WithMessage("Medication record does not exists.");
        Include(new IMedicationDtoValidator());
        // Include(new CreateMedicationDtoValidator(_medicationRepository));
    }
}