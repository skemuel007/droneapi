using Application.Contracts.Persistence;
using FluentValidation;

namespace Application.DTOs.Medication.Validators;

public class MedicationDetailsParamDtoValidator : AbstractValidator<MedicationDetailsParamsDto>
{
    private readonly IMedicationRepository _medicationRepository;
    public MedicationDetailsParamDtoValidator(IMedicationRepository medicationRepository)
    {
        _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));

        When(m => !string.IsNullOrEmpty(m.Code), () =>
        {
            RuleFor(m => m.Code)
                .MustAsync(async (code, token) =>
                {
                    var medicationExists = await _medicationRepository.AnyAsync(d => d.Code == code);
                    return medicationExists;
                }).WithMessage("Medication record does not exists");
        });
        
        When(m => m.Id != null || m.Id != Guid.Empty, () =>
        {
            RuleFor(m => m.Id)
                .MustAsync(async (id, context) =>
                {
                    var medicationExists = await _medicationRepository.AnyAsync(d => d.Id == id);
                    return medicationExists;
                }).WithMessage("Medication record does not exist.");
        });

    }
}