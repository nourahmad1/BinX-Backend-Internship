
using System.ComponentModel.DataAnnotations;

namespace CardiacPatientMonitoring.Api.DTOs;

public class PatientCreateDto
{
    // Patient's first name
    [Required]
    [MaxLength(50)]
    [MinLength(2)]
    public string FirstName { get; set; } = string.Empty;

    // Patient's last name
    [Required]
    [MaxLength(50)]
    [MinLength(2)]
    public string LastName { get; set; } = string.Empty;

    // Patient's date of birth
    [Required]
    public DateTime? DateOfBirth { get; set; }

    // Patient's gender
    [Required]
    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    // Contact phone number
    [Required]
    [Phone]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;
}
