using Domain.Enums;

namespace Application.DTOs.DroneRequest;

public interface IDroneRequestDto
{
    public Guid DroneId { get; set; }
    public string Origin { get; set; }
    public string Destination { get; set; }
    public DroneRequestState Status { get; set; } 
    public string DroneRequestCode { get; set; }
}