using System.Linq.Expressions;
using Application.Contracts.Persistence;
using Application.DTOs.Common;
using Application.DTOs.Drone;
using Application.Features.Drone.Handlers.Queries;
using Application.Features.Drone.Request.Queries;
using Application.Mappings;
using Application.Responses;
using AutoMapper;
using Domain.Enums;
using DroneApp.Test.Mocks.Drone;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace DroneApp.Test.Drone.Queries;

public class GetDroneQueryHandlerTest
{
    private readonly Mock<IDronesRepository> _mockDroneRepository;
    private readonly IMapper _mapper;
    
    public GetDroneQueryHandlerTest()
    {
        _mockDroneRepository = MockDroneRepository.GetDroneRepository();
        // _mockDroneRepository = new Mock<IDronesRepository>(); 
        var mapperConfig = new MapperConfiguration(c =>
        {
            c.AddProfile<MappingProfile>();
        });

        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task GetAvailableDronesForLoading()
    {
        var drones = new List<Domain.Entities.Drone>()
        {
            new Domain.Entities.Drone
            {
                Id = new Guid("1daa4db5-0ce9-4cb3-9388-fd4317cb3904"),
                SerialNumber = "2ZR5Y-S29XX-OEXDL-VIULN-5XVR4",
                BatteryCapacity = 100,
                WeightLimit = 500,
                Model = DroneModel.HEAVYWEIGHT,
                State = DroneState.IDLE,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            new Domain.Entities.Drone()
            {
                Id = new Guid("8af14faf-d8d9-4ab6-9281-dacb4ae340ef"),
                SerialNumber = "2IXEI-FNP0E-GMTPA-LXBPL-DZ665",
                BatteryCapacity = 100,
                WeightLimit = 400,
                Model = DroneModel.MIDDLEWEIGHT,
                State = DroneState.IDLE,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }
        };

        var paginatedDroneList = Paginated<Domain.Entities.Drone>.ToPaginatedList(
            data: drones,
            count: drones.Count,
            pageIndex: 1,
            pageSize: 10,
            sortOrder: null,
            sortColumn: null,
            filterColumn: null,
            filterQuery: null);
        
        // _mockPaginatedQueryRequest.Setup(x => x.Predicate.Compile()).Verifiable();
        _mockDroneRepository.Setup(d => d.GetPaginated(It.IsAny<PaginateQueryRequestNew<Domain.Entities.Drone>>()))
            .ReturnsAsync(paginatedDroneList);
        
        /*_mockDroneRepository.Setup(d => d.GetPaginated(new PaginateQueryRequestNew<Domain.Entities.Drone>
        {
            Predicate = It.IsAny<Expression<Func<Domain.Entities.Drone, bool>>>(),
            Includes = null,
            Page = 1,
            OrderBy = null,
            DisableTracking = true,
            PageSize = 10
        })).ReturnsAsync(paginatedDroneList);*/
        
       // Arrange
       var handler =
           new GetAvailableDronesForLoadingRequestHandler(_mockDroneRepository.Object);

       // Act
       var result = await handler.Handle(new GetAvailableDronesForLoadingRequest(), CancellationToken.None);

       // Assert
       result.ShouldBeOfType<Paginated<Domain.Entities.Drone>>();
       result.Data.Count.ShouldBeGreaterThan(1);
    }

    [Theory]
    [InlineData("388DKAKLALD")]
    [InlineData("FII3LS3KIEL")]
    [InlineData("KDKLLSEKDLD")]
    public async Task CheckDroneBatteryLevelForWithNonExistingSerialNumber(string serialNumber)
    {
        var command = new DroneDetailsRequestDto()
        {
            SerialNumber = serialNumber
        };

        _mockDroneRepository
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Domain.Entities.Drone, bool>>>()
            , null, null, true))
            .Returns(Task.FromResult<Domain.Entities.Drone>(null));
        // Arrange
        var handler = new GetDroneDetailsRequestHandler(_mockDroneRepository.Object, _mapper);
        
        // Act 
        var result = await handler.Handle(new GetDroneDetailsRequest()
        {
            DroneDetailsRequestDto = command
        }, CancellationToken.None);
        
        // Assert
        Assert.False(result.Success);
    }
    
    [Fact]
    public async Task CheckDroneBatteryLevelForDroneBySerialNumber()
    {
        var command = new DroneDetailsRequestDto()
        {
            SerialNumber = "2ZR5Y-S29XX-OEXDL-VIULN-5XVR4"
        };

        _mockDroneRepository
            .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<Domain.Entities.Drone, bool>>>()
                , It.IsAny<Func<IQueryable<Domain.Entities.Drone>, IOrderedQueryable<Domain.Entities.Drone>>>(),
                It.IsAny<List<Expression<Func<Domain.Entities.Drone, object>>>>(), true))
            .ReturnsAsync( new Domain.Entities.Drone() {
                Id = new Guid("1daa4db5-0ce9-4cb3-9388-fd4317cb3904"),
                SerialNumber = "2ZR5Y-S29XX-OEXDL-VIULN-5XVR4",
                BatteryCapacity = 100,
                WeightLimit = 500,
                Model = DroneModel.HEAVYWEIGHT,
                State = DroneState.IDLE,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
        // Arrange
        var handler = new GetDroneDetailsRequestHandler(_mockDroneRepository.Object, _mapper);
        
        // Act 
        var result = await handler.Handle(new GetDroneDetailsRequest()
        {
            DroneDetailsRequestDto = command
        }, CancellationToken.None);
        
        // Assert
        Assert.False(result.Success);
    }
}