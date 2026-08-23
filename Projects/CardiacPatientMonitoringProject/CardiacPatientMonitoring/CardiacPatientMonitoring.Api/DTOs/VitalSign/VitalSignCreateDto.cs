
using System.ComponentModel.DataAnnotations;

namespace CardiacPatientMonitoring.Api.DTOs;

public class VitalSignCreateDto
{
    // Patient this vital sign record belongs to
    [Required]
    public int PatientId { get; set; }

    // Heart rate in beats per minute
    [Range(30, 220)]
    public int HeartRate { get; set; }

    // Upper blood pressure value
    [Range(50, 250)]
    public int SystolicPressure { get; set; }

    // Lower blood pressure value
    [Range(30, 150)]
    public int DiastolicPressure { get; set; }

    // Oxygen level in the blood
    [Range(50, 100)]
    public decimal OxygenSaturation { get; set; }

    // Time when the reading was taken
    [Required]
    public DateTime? RecordedAt { get; set; }

    // Optional notes about the reading
    [MaxLength(200)]
    public string? Notes { get; set; }
}
