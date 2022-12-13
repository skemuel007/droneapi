using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Entities;

public class Medication : BaseEntity
{
    public string Name { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Weight { get; set; }
}