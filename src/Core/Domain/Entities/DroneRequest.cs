using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class DroneRequest : BaseEntity
{
    public Guid DroneId { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public DroneRequestState Status { get; set; }
    public string DroneRequestCode { get; set; }
    public virtual ICollection<DronePayload> DronePayloads { get; set; }

    public virtual Drone Drone { get; set; }
}