using System.ComponentModel.DataAnnotations;

namespace CardiacPatientMonitoring.Api.DTOs;

public class LoginDto
{
    // User email used for login
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    // User password
    [Required]
    public string Password { get; set; } = string.Empty;
}