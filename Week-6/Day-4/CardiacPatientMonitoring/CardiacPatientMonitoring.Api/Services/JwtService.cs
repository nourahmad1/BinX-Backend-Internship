using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CardiacPatientMonitoring.Api.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CardiacPatientMonitoring.Api.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AuthTokenResult GenerateToken(
        ApplicationUser user,
        IList<string>? roles = null)
    {
        // Read JWT settings from appsettings.json
        var jwtSettings =
            _configuration.GetSection("Jwt");

        var issuer =
            jwtSettings["Issuer"]
            ?? throw new InvalidOperationException(
                "JWT Issuer is not configured.");

        var audience =
            jwtSettings["Audience"]
            ?? throw new InvalidOperationException(
                "JWT Audience is not configured.");

        var secretKey =
            jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException(
                "JWT SecretKey is not configured.");

        // Use configured expiry time,
        // or 60 minutes if it is missing
        var expiryMinutes =
            int.TryParse(
                jwtSettings["ExpiryMinutes"],
                out var minutes)
                ? minutes
                : 60;

        var expiresAt =
            DateTime.UtcNow.AddMinutes(
                expiryMinutes);

        // =========================================================
        // User claims
        // =========================================================

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.Id),

            new(
                JwtRegisteredClaimNames.Email,
                user.Email ?? string.Empty),

            new(
                ClaimTypes.NameIdentifier,
                user.Id),

            new(
                ClaimTypes.Email,
                user.Email ?? string.Empty),

            new(
                ClaimTypes.Name,
                user.FullName)
        };

        // =========================================================
        // Add user's roles to the JWT
        // Always normalize roles to uppercase
        // =========================================================

        if (roles is not null)
        {
            foreach (var role in roles)
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Role,
                        role.Trim().ToUpperInvariant()));
            }
        }

        // =========================================================
        // Create signing key
        // =========================================================

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    secretKey));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        // =========================================================
        // Create JWT
        // =========================================================

        var token =
            new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

        // =========================================================
        // Return token result
        // =========================================================

        return new AuthTokenResult
        {
            Token =
                new JwtSecurityTokenHandler()
                    .WriteToken(token),

            ExpiresAt = expiresAt
        };
    }
}