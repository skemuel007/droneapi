namespace Application.DTOs.DronePayload;

public class AddDroneMultiPayloadDto : IDroneMultiPayloadDto
{
    public Guid DroneRequestId { get; set; }
    public List<Guid> MedicationIds { get; set; }
    public int Quantity { get; set; }
}