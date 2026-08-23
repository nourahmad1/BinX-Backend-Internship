
using System.ComponentModel.DataAnnotations;

namespace CardiacPatientMonitoring.Api.DTOs.VitalSign;

public class VitalSignUpdateDto
{
    // Updated heart rate
    [Range(30, 220)]
    public int HeartRate { get; set; }

    // Updated upper blood pressure value
    [Range(50, 250)]
    public int SystolicPressure { get; set; }

    // Updated lower blood pressure value
    [Range(30, 150)]
    public int DiastolicPressure { get; set; }

    // Updated oxygen saturation level
    [Range(50, 100)]
    public decimal OxygenSaturation { get; set; }

    // Time when the reading was recorded
    [Required]
    public DateTime RecordedAt { get; set; }

    // Optional notes
    [MaxLength(200)]
    public string? Notes { get; set; }
}

