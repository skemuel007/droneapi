namespace Application.DTOs.Medication;

public class CreateMedicationDto : IMedicationDto
{
    public string Name { get; set; }
    public decimal Weight { get; set; }
    public string Code { get; set; }
    public string Image { get; set; }
}