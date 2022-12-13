using Application.DTOs.Common;

namespace Application.DTOs.Medication;

public class MedicationDetailsParamsDto : BaseDTO
{
    public string? Code { get; set; }
}