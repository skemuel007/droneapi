namespace Application.DTOs.DronePayload;

public interface IDroneMultiPayloadDto
{
    public Guid DroneRequestId { get; set; }
    public List<Guid> MedicationIds { get; set; }
    public int Quantity { get; set; }
}