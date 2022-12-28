using Application.Contracts.Persistence;
using Domain.Enums;
using Moq;

namespace DroneApp.Test.Mocks.Drone;

public static class MockDroneRepository
{
    public static Mock<IDronesRepository> GetDroneRepository()
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

        var mockDroneRepository = new Mock<IDronesRepository>();

        mockDroneRepository.Setup(d => d.GetAllAsync()).ReturnsAsync(drones);

        mockDroneRepository.Setup(d => d.AddAsync(It.IsAny<Domain.Entities.Drone>()))
            .ReturnsAsync((Domain.Entities.Drone drone) =>
            {
                drones.Add(drone);
                return drone;
            });

        return mockDroneRepository;
    }
}