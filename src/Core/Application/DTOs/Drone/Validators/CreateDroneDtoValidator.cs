using Application.Contracts.Persistence;
using FluentValidation;

namespace Application.DTOs.Drone.Validators;

public class CreateDroneDtoValidator : AbstractValidator<CreateDroneDto>
{
    private readonly IDronesRepository _dronesRepository;

    public CreateDroneDtoValidator(IDronesRepository dronesRepository)
    {
        _dronesRepository = dronesRepository ?? throw new ArgumentNullException(nameof(dronesRepository));
        Include(new IDroneDtoValidator(_dronesRepository));
    }
}