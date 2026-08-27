using System.ComponentModel.DataAnnotations;

namespace CardiacPatientMonitoring.Api.DTOs;

public class RegisterDto
{
    // Full name of the new user
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    // Email used for the account
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    // Password must be at least 6 characters
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    // Role assigned to the new account.
    // Public registration allows Doctor or Patient.
    // Admin accounts should be created separately.
    [Required]
    public string Role { get; set; } = "Patient";
}