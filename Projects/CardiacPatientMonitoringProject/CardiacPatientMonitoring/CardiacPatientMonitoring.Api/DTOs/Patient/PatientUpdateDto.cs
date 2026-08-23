
using System.ComponentModel.DataAnnotations;

namespace CardiacPatientMonitoring.Api.DTOs;

public class PatientUpdateDto
{
    // Updated first name
    [Required]
    [MaxLength(50)]
    [MinLength(2)]
    public string FirstName { get; set; } = string.Empty;

    // Updated last name
    [Required]
    [MaxLength(50)]
    [MinLength(2)]
    public string LastName { get; set; } = string.Empty;

    // Updated date of birth
    [Required]
    public DateTime? DateOfBirth { get; set; }

    // Updated gender
    [Required]
    [MaxLength(20)]
    public string Gender { get; set; } = string.Empty;

    // Updated phone number
    [Required]
    [Phone]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;
}
