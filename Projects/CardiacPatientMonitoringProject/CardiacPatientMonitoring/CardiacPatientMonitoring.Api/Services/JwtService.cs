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
        // =========================================================
        // JWT Configuration
        // =========================================================

        var jwtSettings =
            _configuration.GetSection("Jwt");

        var secretKey =
            jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException(
                "JWT SecretKey is not configured.");

        var issuer =
            jwtSettings["Issuer"]
            ?? throw new InvalidOperationException(
                "JWT Issuer is not configured.");

        var audience =
            jwtSettings["Audience"]
            ?? throw new InvalidOperationException(
                "JWT Audience is not configured.");

        var expiryMinutes =
            int.Parse(
                jwtSettings["ExpiryMinutes"] ?? "60");

        // =========================================================
        // Get User Roles
        // =========================================================

        var roles =
            await _userManager.GetRolesAsync(user);

        // =========================================================
        // Create Claims
        // =========================================================

        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id),

            new Claim(
                ClaimTypes.Name,
                user.UserName ?? string.Empty),

            new Claim(
                ClaimTypes.Email,
                user.Email ?? string.Empty)
        };

        // Add user's roles to JWT
        foreach (var role in roles)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role));
        }

        // =========================================================
        // Create Signing Key
        // =========================================================

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        // =========================================================
        // Calculate Expiration
        // =========================================================

        var expiresAt =
            DateTime.UtcNow.AddMinutes(expiryMinutes);

        // =========================================================
        // Create JWT
        // =========================================================

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        // =========================================================
        // Convert JWT to String
        // =========================================================

        var tokenString =
            new JwtSecurityTokenHandler()
                .WriteToken(token);

        // =========================================================
        // Return Token Result
        // =========================================================

        return new AuthTokenResult
        {
            Token = tokenString,
            ExpiresAt = expiresAt
        };
    }
}