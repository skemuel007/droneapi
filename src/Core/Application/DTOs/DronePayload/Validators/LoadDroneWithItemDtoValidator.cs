using Application.Contracts.Persistence;
using Domain.Enums;
using FluentValidation;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.DTOs.DronePayload.Validators;

public class LoadDroneWithItemDtoValidator : AbstractValidator<LoadDroneWithItemDto>
{
    private readonly IMedicationRepository _medicationRepository;
    private readonly IDronesRepository _dronesRepository;
    private readonly IDroneRequestRepository _droneRequestRepository;
    public LoadDroneWithItemDtoValidator(
        IDronesRepository dronesRepository,
        IMedicationRepository medicationRepository,
        IDroneRequestRepository droneRequestRepository)
    {
        _dronesRepository = dronesRepository ?? throw new ArgumentNullException(nameof(dronesRepository));
        _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));
        _droneRequestRepository = droneRequestRepository ?? throw new ArgumentNullException(nameof(droneRequestRepository));

        RuleFor(d => d.Origin)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.");

        RuleFor(d => d.Destination)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.");

        RuleFor(d => d.Status).IsInEnum().WithMessage("Please pass a valid status value.");

        RuleFor(d => d.DroneRequestCode)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.");

        RuleFor(dp => dp.DroneId)
            .NotEmpty().NotNull().WithMessage("{PropertyName} property is required.")
            .MustAsync(async (droneId, token) =>
            {
                var droneExists = await _dronesRepository.AnyAsync(d => d.Id == droneId);
                return droneExists;
            }).WithMessage("Invalid drone id, please enter a valid drone id.")
            .MustAsync(async (droneId, token) =>
            {
                var droneEngaged = await _droneRequestRepository.AnyAsync(d => d.Id == droneId && d.Status == DroneRequestState.ENGAGED);
                return !droneEngaged;
            }).WithMessage($"Drone is already engaged");

        RuleForEach(itemToValidate => new PayloadValidator(_medicationRepository));
    }
}
