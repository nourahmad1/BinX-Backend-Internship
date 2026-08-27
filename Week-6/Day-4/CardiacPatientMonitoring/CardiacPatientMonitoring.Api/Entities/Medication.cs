namespace CardiacPatientMonitoring.Api.Entities;

public class Medication
{
    // Medication ID
    public int Id { get; set; }

    // ID of the patient taking the medication
    public int PatientId { get; set; }

    // Medication name
    public string Name { get; set; } = string.Empty;

    // Prescribed dosage
    public string Dosage { get; set; } = string.Empty;

    // How often the medication is taken
    public string Frequency { get; set; } = string.Empty;

    // Date when the medication starts
    public DateTime StartDate { get; set; }

    // Optional date when the medication ends
    public DateTime? EndDate { get; set; }

    // Additional notes about the medication
    public string? Notes { get; set; }

    // Available medication stock
    public int StockQuantity { get; set; }

    // Price of one unit of the medication
    public decimal UnitPrice { get; set; }

    // Navigation property back to the patient
    public Patient Patient { get; set; } = null!;
}