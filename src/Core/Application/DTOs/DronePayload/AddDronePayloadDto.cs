namespace Application.DTOs.DronePayload;

public class AddDronePayloadDto : IDronePayloadDto
{
    public Guid DroneRequestId { get; set; }
    public Guid? MedicationId { get; set; }
    public int Quantity { get; set; }
    public List<Guid>? MedicationIds { get; set; }
}