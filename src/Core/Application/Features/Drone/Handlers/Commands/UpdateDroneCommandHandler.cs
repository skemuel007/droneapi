using System.Net;
using Application.Contracts.Persistence;
using Application.DTOs.Drone.Validators;
using Application.Features.Drone.Request.Commands;
using Application.Responses;
using AutoMapper;
using MediatR;

namespace Application.Features.Drone.Handlers.Commands;

public class UpdateDroneCommandHandler : IRequestHandler<UpdateDroneCommand, BaseCommandResponse>
{
    private readonly IDronesRepository _dronesRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDroneCommandHandler(IDronesRepository dronesRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _dronesRepository = dronesRepository ?? throw new ArgumentNullException(nameof(dronesRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<BaseCommandResponse> Handle(UpdateDroneCommand command, CancellationToken cancellationToken)
    {
        // validate drone request
        var validator = new UpdateDroneDtoValidator(_dronesRepository);
        var validationResult = await validator.ValidateAsync(command.UpdateDroneDto);

        // return validation error
        if (validationResult.IsValid == false)
        {
            return new BaseCommandResponse()
            {
                Message = "Drone update validation failed",
                Errors = validationResult.Errors.Select(v => v.ErrorMessage).ToList(),
                StatusCode = HttpStatusCode.UnprocessableEntity
            };
        }

        // get drone, then use mapper to update changes
        var drone = await _dronesRepository.GetByIdAsync(command.UpdateDroneDto.Id);
        _mapper.Map(command.UpdateDroneDto, drone);
        _dronesRepository.Update(drone); // do drone update, then commit
        await _unitOfWork.CompleteAsync();

        return new BaseCommandResponse()
        {
            Success = true,
            StatusCode = HttpStatusCode.OK,
            Message = "Drone record updated successfully"
        };
    }
}