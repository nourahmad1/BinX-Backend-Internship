
using System.ComponentModel.DataAnnotations;
using CardiacPatientMonitoring.Api.Validation;

namespace CardiacPatientMonitoring.Api.DTOs;

public class AppointmentCreateDto
{
    // Patient this appointment belongs to
    [Required]
    public int PatientId { get; set; }

    // Date and time of the appointment
    [Required]
    [FutureDate]
    public DateTime? AppointmentDate { get; set; }

    // Doctor handling the appointment
    [Required]
    [MaxLength(100)]
    public string DoctorName { get; set; } = string.Empty;

    // Reason for the appointment
    [Required]
    [MaxLength(200)]
    public string Reason { get; set; } = string.Empty;

    // New appointments start as scheduled by default
    [Required]
    [RegularExpression(
        "^(Scheduled|Completed|Cancelled)$",
        ErrorMessage = "Status must be Scheduled, Completed, or Cancelled.")]
    public string Status { get; set; } = "Scheduled";

    // Optional notes
    [MaxLength(200)]
    public string? Notes { get; set; }
}