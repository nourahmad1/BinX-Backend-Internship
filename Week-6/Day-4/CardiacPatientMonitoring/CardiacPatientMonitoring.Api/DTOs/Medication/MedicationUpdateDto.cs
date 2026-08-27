namespace CardiacPatientMonitoring.Api.DTOs;

public class MedicationUpdateDto
{
    // Updated medication name
    public string Name { get; set; } = string.Empty;

    // Updated dosage
    public string Dosage { get; set; } = string.Empty;

    // Updated frequency
    public string Frequency { get; set; } = string.Empty;

    // Updated start date
    public DateTime StartDate { get; set; }

    // Optional end date
    public DateTime? EndDate { get; set; }

    // Additional notes
    public string? Notes { get; set; }

    // Updated medication stock
    public int StockQuantity { get; set; }

    // Updated price of one unit
    public decimal UnitPrice { get; set; }
}