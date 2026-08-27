namespace CardiacPatientMonitoring.Api.DTOs;

public class MedicationResponseDto
{
    // Medication ID
    public int Id { get; set; }

    // Patient who is taking the medication
    public int PatientId { get; set; }

    // Name of the medication
    public string Name { get; set; } = string.Empty;

    // Prescribed dosage
    public string Dosage { get; set; } = string.Empty;

    // How often it should be taken
    public string Frequency { get; set; } = string.Empty;

    // Date when the medication started
    public DateTime StartDate { get; set; }

    // Optional date when the medication ends
    public DateTime? EndDate { get; set; }

    // Additional notes
    public string? Notes { get; set; }
}