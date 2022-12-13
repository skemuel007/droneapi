using System.Net;
using Application.Contracts.Persistence;
using Application.DTOs.Drone.Validators;
using Application.DTOs.Medication.Validators;
using Application.Features.Drone.Request.Commands;
using Application.Features.Medication.Request.Commands;
using Application.Responses;
using AutoMapper;
using MediatR;

namespace Application.Features.Medication.Handlers.Commands;

public class CreateMedicationCommandHandler : IRequestHandler<CreateMedicationCommand, BaseCommandResponse<Domain.Entities.Medication>>
{
    private readonly IMedicationRepository _medicationRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMedicationCommandHandler(IMedicationRepository medicationRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<BaseCommandResponse<Domain.Entities.Medication>> Handle(CreateMedicationCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateMedicationDtoValidator(_medicationRepository);
        var validationResult = await validator.ValidateAsync(command.CreateMedication);

        if (validationResult.IsValid == false)
        {
            return new BaseCommandResponse<Domain.Entities.Medication>()
            {
                Message = "Create medication validation error",
                Errors = validationResult.Errors.Select(v => v.ErrorMessage).ToList(),
                StatusCode = HttpStatusCode.UnprocessableEntity
            };
        }

        var medication = _mapper.Map<Domain.Entities.Medication>(command.CreateMedication);
        medication = await _medicationRepository.AddAsync(medication);
        await _unitOfWork.CompleteAsync();

        return new BaseCommandResponse<Domain.Entities.Medication>()
        {
            Message = "Medication creation Successful",
            StatusCode = HttpStatusCode.Created,
            Data = medication,
            Success = true
        };
    }
}