using Application.DTOs.Common;

namespace Application.DTOs.DronePayload;

public class AddedDronePayloadResponseDto : BaseDTO, IDronePayloadDto
{
    public Guid DroneRequestId { get; set; }
    public Guid MedicationId { get; set; }
    public int Quantity { get; set; }
}