namespace CardiacPatientMonitoring.Api.DTOs;

public class PrescriptionResponseDto
{
    // Prescription ID
    public int Id { get; set; }

    // Patient receiving the prescription
    public int PatientId { get; set; }

    // Date when the prescription was created
    public DateTime CreatedAt { get; set; }

    // Total price of all prescription items
    public decimal TotalAmount { get; set; }

    // Items included in the prescription
    public List<PrescriptionItemResponseDto> Items { get; set; }
        = new List<PrescriptionItemResponseDto>();
}

public class PrescriptionItemResponseDto
{
    // Prescription item ID
    public int Id { get; set; }

    // Medication ID
    public int MedicationId { get; set; }

    // Medication name
    public string MedicationName { get; set; } = string.Empty;

    // Quantity requested
    public int Quantity { get; set; }

    // Price of one medication unit
    public decimal UnitPrice { get; set; }

    // Quantity * UnitPrice
    public decimal LineTotal { get; set; }
}