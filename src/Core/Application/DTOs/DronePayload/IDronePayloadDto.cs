namespace Application.DTOs.DronePayload;

public interface IDronePayloadDto
{
    public Guid DroneRequestId { get; set; }
    public Guid MedicationId { get; set; }
    public int Quantity { get; set; }
}