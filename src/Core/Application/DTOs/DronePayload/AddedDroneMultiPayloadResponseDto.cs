using Application.DTOs.Common;

namespace Application.DTOs.DronePayload;

public class AddedDroneMultiPayloadResponseDto : BaseDTO, IDroneMultiPayloadDto
{
    public Guid DroneRequestId { get; set; }
    public List<Guid> MedicationIds { get; set; }
    public int Quantity { get; set; }
}