namespace CardiacPatientMonitoring.Api.DTOs;

public class PrescriptionCreateDto
{
    // Patient receiving the prescription
    public int PatientId { get; set; }

    // Medications included in the prescription
    public List<PrescriptionItemCreateDto> Items { get; set; }
        = new List<PrescriptionItemCreateDto>();
}

public class PrescriptionItemCreateDto
{
    // Medication requested
    public int MedicationId { get; set; }

    // Quantity requested
    public int Quantity { get; set; }
}