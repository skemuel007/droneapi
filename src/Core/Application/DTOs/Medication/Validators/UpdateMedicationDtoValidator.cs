using Application.Contracts.Persistence;
using FluentValidation;

namespace Application.DTOs.Medication.Validators;

public class UpdateMedicationDtoValidator : AbstractValidator<UpdateMedicationDto>
{
    private readonly IMedicationRepository _medicationRepository;

    public UpdateMedicationDtoValidator(IMedicationRepository medicationRepository)
    {
        _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));
        Include(new CreateMedicationDtoValidator(_medicationRepository));
    }
}