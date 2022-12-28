using System.Net;
using Application.Contracts.Infrastructure;
using Application.Contracts.Persistence;
using Application.DTOs.Drone.Validators;
using Application.Features.Drone.Request.Commands;
using Application.Responses;
using AutoMapper;
using MediatR;

namespace Application.Features.Drone.Handlers.Commands;

public class CreateDroneCommandHandler : IRequestHandler<CreateDroneCommand, BaseCommandResponse<Domain.Entities.Drone>>
{
    private readonly IDronesRepository _dronesRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public CreateDroneCommandHandler(IDronesRepository dronesRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _dronesRepository = dronesRepository ?? throw new ArgumentNullException(nameof(dronesRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    public async Task<BaseCommandResponse<Domain.Entities.Drone>> Handle(CreateDroneCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateDroneDtoValidator(_dronesRepository);
        var validationResult = await validator.ValidateAsync(command.DroneDto);

        if (validationResult.IsValid == false)
        {
            return new BaseCommandResponse<Domain.Entities.Drone>()
            {
                Message = "Drone creation validation failed",
                Errors = validationResult.Errors.Select(v => v.ErrorMessage).ToList(),
                StatusCode = HttpStatusCode.UnprocessableEntity
            };
        }

        var drone = _mapper.Map<Domain.Entities.Drone>(command.DroneDto);
        drone = await _dronesRepository.AddAsync(drone);
        await _unitOfWork.CompleteAsync();

        await _cacheService.RemoveData("drones"); // invalidate cache

        return new BaseCommandResponse<Domain.Entities.Drone>()
        {
            Message = "Drone creation Successful",
            StatusCode = HttpStatusCode.Created,
            Data = drone,
            Success = true
        };
    }
}