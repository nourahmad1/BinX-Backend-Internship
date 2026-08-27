namespace CardiacPatientMonitoring.Api.Entities;

public class Appointment
{
    // Appointment ID 
    public int Id { get; set; }

    // ID of the patient for this appointment
    public int PatientId { get; set; }

    // Date and time of the appointment
    public DateTime AppointmentDate { get; set; }

    // Doctor assigned to the appointment
    public string DoctorName { get; set; } = string.Empty;

    // Reason for the appointment
    public string Reason { get; set; } = string.Empty;

    // Current status of the appointment
    public string Status { get; set; } = "Scheduled";

    // Optional notes
    public string? Notes { get; set; }

    // Navigation property back to the patient
    public Patient Patient { get; set; } = null!;
}//1111111111111