using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Drone : BaseEntity
{
    [StringLength(100)]
    public string SerialNumber { get; set; }
    [Range(0, 500)]
    public decimal WeightLimit { get; set; }
    [Range(0, 100)]
    public int BatteryCapacity { get; set; }
    public DroneModel Model { get; set; }
    public DroneState State { get; set; }

    public virtual ICollection<DroneRequest> DroneRequests { get; set; }
}