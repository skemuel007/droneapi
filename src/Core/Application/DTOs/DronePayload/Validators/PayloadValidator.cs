using Application.Contracts.Persistence;
using FluentValidation;

namespace Application.DTOs.DronePayload.Validators;

public class PayloadValidator : AbstractValidator<PayloadDto>
{
    private readonly IMedicationRepository _medicationRepository;

    public PayloadValidator(IMedicationRepository medicationRepository)
    {
        _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));

        RuleFor(dp => dp.Quantity)
            .NotEmpty().NotNull().WithMessage("{PropertyName} is required.")
            .GreaterThan(0).WithMessage("{PropertyName} should be greater than 0.");


        RuleFor(dp => dp.MedicationId)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.")
            .MustAsync(async (medicationId, token) =>
            {
                var medicationExists = await _medicationRepository.AnyAsync(d => d.Id == medicationId);
                return medicationExists;
            }).WithMessage("Invalid medication id, please enter a valid medication id.");
    }
}
