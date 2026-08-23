
using System.ComponentModel.DataAnnotations;

namespace CardiacPatientMonitoring.Api.DTOs;

public class MedicationCreateDto
{
    // Patient who will take the medication
    [Required]
    public int PatientId { get; set; }

    // Name of the medication
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    // Amount of medication to take
    [Required]
    [MaxLength(50)]
    public string Dosage { get; set; } = string.Empty;

    // How often the medication should be taken
    [Required]
    [MaxLength(100)]
    public string Frequency { get; set; } = string.Empty;

    // Date when the medication starts
    [Required]
    public DateTime? StartDate { get; set; }

    // Optional end date
    public DateTime? EndDate { get; set; }

    // Any extra notes
    [MaxLength(200)]
    public string? Notes { get; set; }
}

