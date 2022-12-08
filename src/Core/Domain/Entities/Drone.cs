using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Drone : BaseEntity
{
    public string SerialNumber { get; set; }
    public decimal WeightLimit { get; set; }
    public int BatteryCapacity { get; set; }
    public DroneModel Model { get; set; }
    public DroneState State { get; set; }
}