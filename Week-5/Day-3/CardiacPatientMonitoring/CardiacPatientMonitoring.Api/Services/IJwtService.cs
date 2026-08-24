using CardiacPatientMonitoring.Api.Entities;

namespace CardiacPatientMonitoring.Api.Services;

public interface IJwtService
{
    // Creates a JWT token for the logged-in user
    // Roles are optional for backward compatibility with existing tests.
    AuthTokenResult GenerateToken(
        ApplicationUser user,
        IList<string>? roles = null);
}

public class AuthTokenResult
{
    // The generated JWT token
    public string Token { get; set; } = string.Empty;

    // Time when the token will expire
    public DateTime ExpiresAt { get; set; }
}