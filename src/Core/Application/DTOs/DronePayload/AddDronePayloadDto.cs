using Domain.Enums;

namespace Application.DTOs.DronePayload;

public class AddDronePayloadDto : IDronePayloadDto
{
    public Guid DroneRequestId { get; set; }
    public Guid? MedicationId { get; set; }
    public int Quantity { get; set; }
    public List<PayloadDto>? Payload { get; set; }
}

public class LoadDroneWithItemDto
{
    public Guid DroneId { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public DroneRequestState Status { get; set; }
    public string DroneRequestCode { get; set; }
    public List<PayloadDto>? Payload { get; set; }
}

public class PayloadDto
{
    public Guid? MedicationId { get; set; }
    public int Quantity { get; set; }
}