
using System.ComponentModel.DataAnnotations;

namespace CardiacPatientMonitoring.Api.DTOs;

public class AppointmentCreateDto
{
    // Patient this appointment belongs to
    [Required]
    public int PatientId { get; set; }

    // Date and time of the appointment
    [Required]
    public DateTime AppointmentDate { get; set; }

    // Doctor handling the appointment
    [Required]
    [MaxLength(100)]
    public string DoctorName { get; set; } = string.Empty;

    // Reason for the appointment
    [Required]
    [MaxLength(250)]
    public string Reason { get; set; } = string.Empty;

    // New appointments start as Scheduled by default
    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "Scheduled";

    // Optional notes about the appointment
    [MaxLength(500)]
    public string? Notes { get; set; }
}
