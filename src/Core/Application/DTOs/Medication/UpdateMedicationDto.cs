using Application.DTOs.Common;

namespace Application.DTOs.Medication;

public class UpdateMedicationDto : CreateMedicationDto, IBaseDto
{
    public Guid Id { get; set; }
}