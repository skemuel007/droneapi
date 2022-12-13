using Domain.Enums;

namespace Application.DTOs.Drone;

public interface IDroneDto
{
    public string SerialNumber { get; set; }
    public decimal WeightLimit { get; set; }
    public int BatteryCapacity { get; set; }
    public DroneModel Model { get; set; }
    public DroneState State { get; set; }
}