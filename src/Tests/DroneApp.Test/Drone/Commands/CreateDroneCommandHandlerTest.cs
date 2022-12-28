using Application.Contracts.Infrastructure;
using Application.Contracts.Persistence;
using Application.DTOs.Drone;
using Application.Features.Drone.Handlers.Commands;
using Application.Features.Drone.Request.Commands;
using Application.Mappings;
using Application.Responses;
using AutoMapper;
using Domain.Enums;
using DroneApp.Test.Mocks.Drone;
using MediatR;
using Moq;
using NuGet.Frameworks;
using Shouldly;

namespace DroneApp.Test.Drone.Commands;

public class CreateDroneCommandHandlerTest
{
    private readonly Mock<IDronesRepository> _mockDroneRepository;
    private readonly IMapper _mapper;
    private readonly Mock<IUnitOfWork> _mockIUnitOfWork;
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ICacheService> _mockICacheService;
    private readonly CreateDroneDto _createDroneDto;

    public CreateDroneCommandHandlerTest()
    {
        _mockDroneRepository = MockDroneRepository.GetDroneRepository();
        
        var mapperConfig = new MapperConfiguration(c =>
        {
            c.AddProfile<MappingProfile>();
        });

        _mapper = mapperConfig.CreateMapper();
        _mockIUnitOfWork = new Mock<IUnitOfWork>();
        _mockMediator = new Mock<IMediator>();
        _mockICacheService = new Mock<ICacheService>();

        // _ = new Mock<IDronesRepository>();

        _createDroneDto = new CreateDroneDto()
        {
            SerialNumber = "2ZR5Y-S29XX-OEXDL-VIULN-5XVR7",
            BatteryCapacity = 100,
            WeightLimit = 500,
            Model = DroneModel.MIDDLEWEIGHT,
            State = DroneState.IDLE
        };
    }
    
    [Fact]
    public async Task RegisterDrone()
    {
        // Arrange
        var handler = new CreateDroneCommandHandler(_mockDroneRepository.Object, _mapper, _mockIUnitOfWork.Object,
            _mockICacheService.Object);

        // Act
        var result = await handler.Handle(new CreateDroneCommand()
        {
            DroneDto = _createDroneDto
        }, CancellationToken.None);

        var drones = await _mockDroneRepository.Object.GetAllAsync();
        
        // Assert
        result.ShouldBeOfType<BaseCommandResponse<Domain.Entities.Drone>>();
        drones.Count.ShouldBeGreaterThan(2);
    }

    [Fact]
    public async Task RegisterDrone_WithInvalidParameter()
    {
        // Arrange
        CreateDroneDto createDroneDto = new CreateDroneDto()
        {
            SerialNumber = "",
            BatteryCapacity = 100,
            WeightLimit = 500,
            Model = DroneModel.MIDDLEWEIGHT,
            State = DroneState.IDLE
        };
        var handler = new CreateDroneCommandHandler(_mockDroneRepository.Object, _mapper, _mockIUnitOfWork.Object,
            _mockICacheService.Object);

        // Act
        var result = await handler.Handle(command: new CreateDroneCommand()
        {
            DroneDto = createDroneDto
        }, CancellationToken.None);
        
        
        // Assert
        Assert.Equal(result.Data, null);
        Assert.IsType<bool>(result.Success);
        Assert.False(result.Success);
        result.Errors.Count.ShouldBeGreaterThan(0);
        result.Message.ShouldContain("Drone creation validation failed");
    }
    
    [Fact]
    public async Task RegisterDrone_WithInvalidBatteryPercentage()
    {
        // Arrange
        CreateDroneDto createDroneDto = new CreateDroneDto()
        {
            SerialNumber = "93DD4428382388ADFDKD",
            BatteryCapacity = 0,
            WeightLimit = 500,
            Model = DroneModel.MIDDLEWEIGHT,
            State = DroneState.IDLE
        };
        var handler = new CreateDroneCommandHandler(_mockDroneRepository.Object, _mapper, _mockIUnitOfWork.Object,
            _mockICacheService.Object);

        // Act
        var result = await handler.Handle(command: new CreateDroneCommand()
        {
            DroneDto = createDroneDto
        }, CancellationToken.None);
        
        
        // Assert
        Assert.Equal(result.Data, null);
        Assert.IsType<bool>(result.Success);
        Assert.False(result.Success);
        result.Errors.Count.ShouldBeGreaterThan(0);
        result.Message.ShouldContain("Drone creation validation failed");
    }
}