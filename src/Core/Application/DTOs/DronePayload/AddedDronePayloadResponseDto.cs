using Application.DTOs.Common;
using Domain.Enums;

namespace Application.DTOs.DronePayload;

public class AddedDronePayloadResponseDto : BaseDTO, IDronePayloadDto
{
    public Guid DroneRequestId { get; set; }
    public Guid? MedicationId { get; set; }
    public List<PayloadDto>? Payload { get; set; }
    public int Quantity { get; set; }
}

public class LoadedPayloadResponseDto
{
    public Guid DroneId { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public DroneRequestState Status { get; set; }
    public string DroneRequestCode { get; set; }
    public List<PayloadDto>? Payload { get; set; }
}