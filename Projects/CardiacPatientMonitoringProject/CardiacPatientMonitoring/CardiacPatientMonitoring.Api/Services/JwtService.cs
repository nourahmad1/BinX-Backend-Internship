using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CardiacPatientMonitoring.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace CardiacPatientMonitoring.Api.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;

    public JwtService(
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<AuthTokenResult> GenerateTokenAsync(
        ApplicationUser user)
    {
        // Read JWT settings from appsettings.json
        var jwtSettings = _configuration.GetSection("Jwt");

        var issuer = jwtSettings["Issuer"]
            ?? throw new InvalidOperationException(
                "JWT Issuer is not configured.");

        var audience = jwtSettings["Audience"]
            ?? throw new InvalidOperationException(
                "JWT Audience is not configured.");

        var secretKey = jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException(
                "JWT SecretKey is not configured.");

        // Use configured expiry time,
        // or 60 minutes if it is missing
        var expiryMinutes = int.TryParse(
            jwtSettings["ExpiryMinutes"],
            out var minutes)
            ? minutes
            : 60;

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        // Get all roles assigned to the user
        var roles = await _userManager.GetRolesAsync(user);

        // Add user information to the JWT
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),

            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.FullName)
        };

        // Add each Identity role as a Role claim
        foreach (var role in roles)
        {
            claims.Add(
                new Claim(ClaimTypes.Role, role));
        }

        // Create signing key
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        // Create JWT
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        // Return token and expiration time
        return new AuthTokenResult
        {
            Token = new JwtSecurityTokenHandler()
                .WriteToken(token),

            ExpiresAt = expiresAt
        };
    }
}