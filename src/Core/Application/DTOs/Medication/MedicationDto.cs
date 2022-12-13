using Application.DTOs.Common;

namespace Application.DTOs.Medication;

public class MedicationDto : BaseDTO, IMedicationDto
{
    public string Name { get; set; }
    public decimal Weight { get; set; }
    public string Code { get; set; }
    public string Image { get; set; }
    public DateTime CreatedAt { get; set; }
}