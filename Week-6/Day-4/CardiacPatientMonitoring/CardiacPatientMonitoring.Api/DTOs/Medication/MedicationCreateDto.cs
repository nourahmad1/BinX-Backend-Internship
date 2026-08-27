namespace CardiacPatientMonitoring.Api.DTOs;

public class MedicationCreateDto
{
    // Patient who will take the medication
    public int PatientId { get; set; }

    // Name of the medication
    public string Name { get; set; } = string.Empty;

    // Amount of medication to take
    public string Dosage { get; set; } = string.Empty;

    // How often the medication should be taken
    public string Frequency { get; set; } = string.Empty;

    // Date when the medication starts
    public DateTime StartDate { get; set; }

    // Optional end date
    public DateTime? EndDate { get; set; }

    // Any extra notes about the medication
    public string? Notes { get; set; }

    // Available medication stock
    public int StockQuantity { get; set; }

    // Price of one unit of the medication
    public decimal UnitPrice { get; set; }
}