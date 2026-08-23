using CardiacPatientMonitoring.Api.Entities;

namespace CardiacPatientMonitoring.Api.Services;

public interface IJwtService
{
    // Creates a JWT token for the logged-in user
    Task<AuthTokenResult> GenerateTokenAsync(ApplicationUser user);
}

public class AuthTokenResult
{
    // The generated JWT token
    public string Token { get; set; } = string.Empty;

    // Time when the token will expire
    public DateTime ExpiresAt { get; set; }
}