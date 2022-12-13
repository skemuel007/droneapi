using System.Net;
using Application.Contracts.Persistence;
using Application.Features.Drone.Request.Commands;
using Application.Responses;
using MediatR;

namespace Application.Features.Drone.Handlers.Commands;

public class DeleteDroneCommandHandler : IRequestHandler<DeleteDroneCommand, BaseCommandResponse>
{
    private readonly IDronesRepository _dronesRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDroneCommandHandler(IDronesRepository dronesRepository,
        IUnitOfWork unitOfWork
    )
    {
        _dronesRepository = dronesRepository ?? throw new ArgumentNullException(nameof(dronesRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    }

    public async Task<BaseCommandResponse> Handle(DeleteDroneCommand command, CancellationToken cancellationToken)
    {
        var drone = await _dronesRepository.GetByIdAsync(command.Id);

        if (drone == null)
        {
            return new BaseCommandResponse
            {
                StatusCode = HttpStatusCode.NotFound,
                Success = false,
                Message = "No such drone record"
            };
        }

        _dronesRepository.DeleteAsync(drone);
        await _unitOfWork.CompleteAsync();
        
        return new BaseCommandResponse
        {
            StatusCode = HttpStatusCode.OK,
            Success = true,
            Message = "Drone record deleted successfully."
        }; 
    }
}