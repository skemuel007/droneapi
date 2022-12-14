using System.Net;
using Application.Contracts.Persistence;
using Application.Features.Medication.Request.Commands;
using Application.Responses;
using MediatR;

namespace Application.Features.Medication.Handlers.Commands;

public class DeleteMedicationCommandHandler : IRequestHandler<DeleteMedicationCommand, BaseCommandResponse>
{
    private readonly IMedicationRepository _medicationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMedicationCommandHandler(IMedicationRepository medicationRepository,
        IUnitOfWork unitOfWork
    )
    {
        _medicationRepository = medicationRepository ?? throw new ArgumentNullException(nameof(medicationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    }

    public async Task<BaseCommandResponse> Handle(DeleteMedicationCommand command, CancellationToken cancellationToken)
    {
        var medication = await _medicationRepository.GetByIdAsync(command.Id);

        if (medication == null)
        {
            return new BaseCommandResponse
            {
                StatusCode = HttpStatusCode.NotFound,
                Success = false,
                Message = "No such medication record"
            };
        }

        _medicationRepository.DeleteAsync(medication);
        await _unitOfWork.CompleteAsync();
        
        return new BaseCommandResponse
        {
            StatusCode = HttpStatusCode.OK,
            Success = true,
            Message = "Medication record deleted successfully."
        }; 
    }
}