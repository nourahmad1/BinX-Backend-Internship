using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CardiacPatientMonitoring.Api.Entities;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.Extensions.Configuration;

namespace CardiacPatientMonitoring.Tests;

public class JwtServiceTests
{
    private static JwtService CreateJwtService()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] =
                    "CardiacPatientMonitoringApi",

                ["Jwt:Audience"] =
                    "CardiacPatientMonitoringClient",

                ["Jwt:SecretKey"] =
                    "ThisIsASecretKeyForTestingJwt123456789",

                ["Jwt:ExpiryMinutes"] = "60"
            })
            .Build();

        return new JwtService(configuration);
    }

    private static ApplicationUser CreateTestUser()
    {
        return new ApplicationUser
        {
            Id = "test-user-id",
            Email = "test@example.com",
            FullName = "Test User"
        };
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidToken()
    {
        // Arrange
        var jwtService = CreateJwtService();
        var user = CreateTestUser();

        var roles = new List<string>
        {
            "Doctor"
        };

        // Act
        var result =
            jwtService.GenerateToken(user, roles);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);

        Assert.True(
            result.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public void GenerateToken_ShouldContainUserClaims()
    {
        // Arrange
        var jwtService = CreateJwtService();
        var user = CreateTestUser();

        var roles = new List<string>
        {
            "Doctor"
        };

        // Act
        var result =
            jwtService.GenerateToken(user, roles);

        var handler = new JwtSecurityTokenHandler();

        var token =
            handler.ReadJwtToken(result.Token);

        // Assert
        Assert.Equal(
            user.Id,
            token.Claims.First(
                c => c.Type ==
                     JwtRegisteredClaimNames.Sub).Value);

        Assert.Equal(
            user.Email,
            token.Claims.First(
                c => c.Type ==
                     JwtRegisteredClaimNames.Email).Value);

        Assert.Equal(
            user.FullName,
            token.Claims.First(
                c => c.Type ==
                     ClaimTypes.Name).Value);
    }

    [Fact]
    public void GenerateToken_ShouldContainUserRole()
    {
        // Arrange
        var jwtService = CreateJwtService();
        var user = CreateTestUser();

        var roles = new List<string>
        {
            "Doctor"
        };

        // Act
        var result =
            jwtService.GenerateToken(user, roles);

        var handler = new JwtSecurityTokenHandler();

        var token =
            handler.ReadJwtToken(result.Token);

        // Assert
        var roleClaim =
            token.Claims.FirstOrDefault(
                c => c.Type == ClaimTypes.Role);

        Assert.NotNull(roleClaim);
        Assert.Equal("Doctor", roleClaim.Value);
    }

    [Fact]
    public void GenerateToken_ShouldContainMultipleRoles()
    {
        // Arrange
        var jwtService = CreateJwtService();
        var user = CreateTestUser();

        var roles = new List<string>
        {
            "Admin",
            "Doctor"
        };

        // Act
        var result =
            jwtService.GenerateToken(user, roles);

        var handler = new JwtSecurityTokenHandler();

        var token =
            handler.ReadJwtToken(result.Token);

        // Assert
        var roleClaims =
            token.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

        Assert.Contains("Admin", roleClaims);
        Assert.Contains("Doctor", roleClaims);
    }

    [Fact]
    public void GenerateToken_ShouldThrowException_WhenIssuerIsMissing()
    {
        // Arrange
        var configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Jwt:Audience"] =
                            "CardiacPatientMonitoringClient",

                        ["Jwt:SecretKey"] =
                            "ThisIsASecretKeyForTestingJwt123456789",

                        ["Jwt:ExpiryMinutes"] = "60"
                    })
                .Build();

        var jwtService =
            new JwtService(configuration);

        var user = CreateTestUser();

        var roles = new List<string>
        {
            "Doctor"
        };

        // Act & Assert
        var exception =
            Assert.Throws<InvalidOperationException>(
                () => jwtService.GenerateToken(
                    user,
                    roles));

        Assert.Equal(
            "JWT Issuer is not configured.",
            exception.Message);
    }

    [Fact]
    public void GenerateToken_ShouldThrowException_WhenSecretKeyIsMissing()
    {
        // Arrange
        var configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Jwt:Issuer"] =
                            "CardiacPatientMonitoringApi",

                        ["Jwt:Audience"] =
                            "CardiacPatientMonitoringClient",

                        ["Jwt:ExpiryMinutes"] = "60"
                    })
                .Build();

        var jwtService =
            new JwtService(configuration);

        var user = CreateTestUser();

        var roles = new List<string>
        {
            "Doctor"
        };

        // Act & Assert
        var exception =
            Assert.Throws<InvalidOperationException>(
                () => jwtService.GenerateToken(
                    user,
                    roles));

        Assert.Equal(
            "JWT SecretKey is not configured.",
            exception.Message);
    }
}