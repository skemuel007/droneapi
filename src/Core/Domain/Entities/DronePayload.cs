using Domain.Common;

namespace Domain.Entities;

public class DronePayload : BaseEntity
{
    public Guid DroneRequestId { get; set; }
    public Guid MedicationId { get; set; }
    public int Quantity { get; set; }

    public virtual DroneRequest DroneRequest { get; set; }
    public virtual Medication Medication { get; set; }
}