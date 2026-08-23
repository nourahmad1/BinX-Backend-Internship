namespace CardiacPatientMonitoring.Api.DTOs;

public class AuthResponseDto
{
    // JWT token returned after login
    public string Token { get; set; } = string.Empty;

    // Time when the token expires
    public DateTime ExpiresAt { get; set; }

    // Email of the logged-in user
    public string Email { get; set; } = string.Empty;

    // Full name of the logged-in user
    public string FullName { get; set; } = string.Empty;
}