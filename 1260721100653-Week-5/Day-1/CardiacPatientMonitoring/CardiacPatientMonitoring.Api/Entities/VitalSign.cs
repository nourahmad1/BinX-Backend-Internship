namespace CardiacPatientMonitoring.Api.Entities;

public class VitalSign
{
    // Vital sign record ID
    public int Id { get; set; }

    // ID of the patient this reading belongs to
    public int PatientId { get; set; }

    // Heart rate in beats per minute
    public int HeartRate { get; set; }

    // Upper blood pressure value
    public int SystolicPressure { get; set; }

    // Lower blood pressure value
    public int DiastolicPressure { get; set; }

    // Oxygen level in the blood
    public decimal OxygenSaturation { get; set; }

    // Time when the reading was recorded
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    // Optional notes about the reading
    public string? Notes { get; set; }

    // Navigation property back to the patient
    public Patient Patient { get; set; } = null!;
}