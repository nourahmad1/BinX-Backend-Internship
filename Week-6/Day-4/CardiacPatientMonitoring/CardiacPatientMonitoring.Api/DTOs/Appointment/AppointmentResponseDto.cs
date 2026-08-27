namespace CardiacPatientMonitoring.Api.DTOs;

public class AppointmentResponseDto
{
    // Appointment ID
    public int Id { get; set; }

    // Patient this appointment belongs to
    public int PatientId { get; set; }

    // Date and time of the appointment
    public DateTime AppointmentDate { get; set; }

    // Doctor handling the appointment
    public string DoctorName { get; set; } = string.Empty;

    // Reason for the appointment
    public string Reason { get; set; } = string.Empty;

    // Appointment status
    public string Status { get; set; } = "Scheduled";

    // Optional notes about the appointment
    public string? Notes { get; set; }
}