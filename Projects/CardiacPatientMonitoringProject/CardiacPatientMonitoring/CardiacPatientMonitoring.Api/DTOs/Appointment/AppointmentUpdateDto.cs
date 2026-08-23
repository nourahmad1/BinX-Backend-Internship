
using System.ComponentModel.DataAnnotations;
using CardiacPatientMonitoring.Api.Validation;

namespace CardiacPatientMonitoring.Api.DTOs;

public class AppointmentUpdateDto
{
    // Updated date and time for the appointment
    [Required]
    [FutureDate]
    public DateTime? AppointmentDate { get; set; }

    // Doctor assigned to the appointment
    [Required]
    [MaxLength(100)]
    public string DoctorName { get; set; } = string.Empty;

    // Updated reason for the appointment
    [Required]
    [MaxLength(200)]
    public string Reason { get; set; } = string.Empty;

    // Keep the appointment scheduled by default
    [Required]
    [RegularExpression(
        "^(Scheduled|Completed|Cancelled)$",
        ErrorMessage = "Status must be Scheduled, Completed, or Cancelled.")]
    public string Status { get; set; } = "Scheduled";

    // Optional notes
    [MaxLength(200)]
    public string? Notes { get; set; }
}
