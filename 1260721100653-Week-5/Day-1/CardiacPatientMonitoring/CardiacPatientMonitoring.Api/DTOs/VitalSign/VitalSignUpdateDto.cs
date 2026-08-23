namespace CardiacPatientMonitoring.Api.DTOs.VitalSign;

public class VitalSignUpdateDto
{
    // Updated heart rate
    public int HeartRate { get; set; }

    // Updated upper blood pressure value
    public int SystolicPressure { get; set; }

    // Updated lower blood pressure value
    public int DiastolicPressure { get; set; }

    // Updated oxygen saturation level
    public decimal OxygenSaturation { get; set; }

    // Time when the reading was recorded
    public DateTime RecordedAt { get; set; }

    // Optional notes about the reading
    public string? Notes { get; set; }
}