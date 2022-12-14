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

public class UpdateMedicationCommandHandler : IRequestHandler<UpdateMedicationCommand, BaseCommandResponse>
{
    private readonly IMedicationRepository _medicationRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMedicationCommandHandler(IMedicationRepository medicationRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<BaseCommandResponse> Handle(UpdateMedicationCommand command, CancellationToken cancellationToken)
    {
        // validate drone request
        var validator = new UpdateMedicationDtoValidator(_medicationRepository);
        var validationResult = await validator.ValidateAsync(command.UpdateMedicationDto);

        // return validation error
        if (validationResult.IsValid == false)
        {
            return new BaseCommandResponse()
            {
                Message = "Medication update validation failed.",
                Errors = validationResult.Errors.Select(v => v.ErrorMessage).ToList(),
                StatusCode = HttpStatusCode.UnprocessableEntity
            };
        }

        // get medication, then use mapper to update changes
        var medication = await _medicationRepository.GetByIdAsync(command.UpdateMedicationDto.Id);
        _mapper.Map(command.UpdateMedicationDto, medication);
        _medicationRepository.Update(medication); // do drone update, then commit
        await _unitOfWork.CompleteAsync();

        return new BaseCommandResponse()
        {
            Success = true,
            StatusCode = HttpStatusCode.OK,
            Message = "Medication record updated successfully"
        };
    }
}