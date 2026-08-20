namespace CardiacPatientMonitoring.Api.DTOs;

public class AppointmentResponseDto
{
    // Appointment ID
    public int Id { get; set; }

    // ID of the patient
    public int PatientId { get; set; }

    // Date and time of the appointment
    public DateTime AppointmentDate { get; set; }

    // Doctor assigned to the appointment
    public string DoctorName { get; set; } = string.Empty;

    // Reason for the appointment
    public string Reason { get; set; } = string.Empty;

    // Current appointment status
    public string Status { get; set; } = string.Empty;

    // Optional notes
    public string? Notes { get; set; }
}