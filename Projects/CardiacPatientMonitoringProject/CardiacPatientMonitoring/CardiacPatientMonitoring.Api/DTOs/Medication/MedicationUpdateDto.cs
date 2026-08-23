
using System.ComponentModel.DataAnnotations;

namespace CardiacPatientMonitoring.Api.DTOs;

public class MedicationUpdateDto
{
    // Updated medication name
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    // Updated dosage
    [Required]
    [MaxLength(50)]
    public string Dosage { get; set; } = string.Empty;

    // Updated frequency
    [Required]
    [MaxLength(100)]
    public string Frequency { get; set; } = string.Empty;

    // Updated start date
    [Required]
    public DateTime? StartDate { get; set; }

    // Optional end date
    public DateTime? EndDate { get; set; }

    // Additional notes
    [MaxLength(200)]
    public string? Notes { get; set; }
}
