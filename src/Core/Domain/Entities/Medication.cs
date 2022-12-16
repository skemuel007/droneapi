using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Entities;

public class Medication : BaseEntity
{
    [RegularExpression(@"^([a-z]|[A-Z]|[0-9]|[_]|[-])+$")]
    public string Name { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Weight { get; set; }
    [RegularExpression(@"^([A-Z]|[0-9]|[_])+$")]
    public string Code { get; set; }
    public string Image { get; set; }
    public virtual ICollection<DronePayload> DronePayloads { get; set; }
}