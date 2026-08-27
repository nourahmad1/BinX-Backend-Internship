
namespace CardiacPatientMonitoring.Api.Entities;

public class Prescription
{
    // Prescription ID
    public int Id { get; set; }

    // Patient who receives the prescription
    public int PatientId { get; set; }

    // Date when the prescription was created
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Total price of all prescription items
    public decimal TotalAmount { get; set; }

    // Navigation property to the patient
    public Patient Patient { get; set; } = null!;

    // Items included in this prescription
    public ICollection<PrescriptionItem> Items { get; set; }
        = new List<PrescriptionItem>();
}
