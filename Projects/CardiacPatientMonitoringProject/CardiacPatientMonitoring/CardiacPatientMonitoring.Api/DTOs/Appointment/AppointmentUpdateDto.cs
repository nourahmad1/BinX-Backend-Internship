namespace CardiacPatientMonitoring.Api.DTOs;

public class AppointmentUpdateDto
{
    // Updated date and time for the appointment
    public DateTime AppointmentDate { get; set; }

    // Doctor assigned to the appointment
    public string DoctorName { get; set; } = string.Empty;

    // Updated reason for the appointment
    public string Reason { get; set; } = string.Empty;

    // Keep the appointment scheduled by default
    public string Status { get; set; } = "Scheduled";

    // Optional notes
    public string? Notes { get; set; }
}