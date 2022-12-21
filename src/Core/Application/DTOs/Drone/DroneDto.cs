using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.Drone;

public class DroneDto : BaseDTO, IDroneDto
{
    public string SerialNumber { get; set; }
    public decimal WeightLimit { get; set; }
    public int BatteryCapacity { get; set; }
    public DroneModel Model { get; set; }
    public string DroneModel { get; set; }
    public DroneState State { get; set; }
    public string DroneState { get; set; }
    public DateTime CreatedAt { get; set; }
}
