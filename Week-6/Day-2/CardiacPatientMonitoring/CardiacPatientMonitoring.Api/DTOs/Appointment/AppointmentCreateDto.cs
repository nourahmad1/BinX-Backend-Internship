namespace CardiacPatientMonitoring.Api.DTOs;
//lll
public class AppointmentCreateDto
{
    // Patient this appointment belongs to
    public int PatientId { get; set; }

    // Date and time of the appointment
    public DateTime AppointmentDate { get; set; }

    // Doctor handling the appointment
    public string DoctorName { get; set; } = string.Empty;

    // Reason for the appointment
    public string Reason { get; set; } = string.Empty;

    // New appointments start as scheduled by default
    public string Status { get; set; } = "Scheduled";

    // Optional notes about the appointment
    public string? Notes { get; set; }
}
//cc