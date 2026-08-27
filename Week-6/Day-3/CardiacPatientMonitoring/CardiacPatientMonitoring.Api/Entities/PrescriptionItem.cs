
namespace CardiacPatientMonitoring.Api.Entities;

public class PrescriptionItem
{
    // Prescription item ID
    public int Id { get; set; }

    // Prescription this item belongs to
    public int PrescriptionId { get; set; }

    // Medication being prescribed
    public int MedicationId { get; set; }

    // Quantity requested
    public int Quantity { get; set; }

    // Price of one medication unit at the time of prescription
    public decimal UnitPrice { get; set; }

    // Quantity * UnitPrice
    public decimal LineTotal { get; set; }

    // Navigation property to prescription
    public Prescription Prescription { get; set; } = null!;

    // Navigation property to medication
    public Medication Medication { get; set; } = null!;
}

