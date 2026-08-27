namespace CardiacPatientMonitoring.Api.DTOs;

public class VitalSignCreateDto
{
    // Patient this vital sign record belongs to
    public int PatientId { get; set; }

    // Heart rate in beats per minute
    public int HeartRate { get; set; }

    // Upper blood pressure value
    public int SystolicPressure { get; set; }

    // Lower blood pressure value
    public int DiastolicPressure { get; set; }

    // Oxygen level in the blood
    public decimal OxygenSaturation { get; set; }

    // Time when the reading was taken
    public DateTime RecordedAt { get; set; }

    // Optional notes about the reading
    public string? Notes { get; set; }
}