using System.IdentityModel.Tokens.Jwt;
using CardiacPatientMonitoring.Api.Entities;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.Extensions.Configuration;

namespace CardiacPatientMonitoring.Tests;

public class JwtServiceTests
{
    [Fact]
    public void GenerateToken_ShouldReturnValidToken()
    {
        // Set up the JWT settings needed for the test
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "CardiacPatientMonitoringApi",
                ["Jwt:Audience"] = "CardiacPatientMonitoringClient",
                ["Jwt:SecretKey"] = "ThisIsASecretKeyForTestingJwt123456789",
                ["Jwt:ExpiryMinutes"] = "60"
            })
            .Build();

        // Create the JWT service using the test configuration
        var jwtService = new JwtService(configuration);

        // Create a sample user for the test
        var user = new ApplicationUser
        {
            Id = "test-user-id",
            Email = "test@example.com",
            FullName = "Test User"
        };

        // Generate a token for the test user
        var result = jwtService.GenerateToken(user);

        // Make sure a token was created successfully
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);

        // The token should expire sometime in the future
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public void GenerateToken_ShouldContainUserClaims()
    {
        // Set up the same JWT configuration used by the service
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "CardiacPatientMonitoringApi",
                ["Jwt:Audience"] = "CardiacPatientMonitoringClient",
                ["Jwt:SecretKey"] = "ThisIsASecretKeyForTestingJwt123456789",
                ["Jwt:ExpiryMinutes"] = "60"
            })
            .Build();

        var jwtService = new JwtService(configuration);

        // Create a user with known values so we can check the claims later
        var user = new ApplicationUser
        {
            Id = "test-user-id",
            Email = "test@example.com",
            FullName = "Test User"
        };

        // Generate the JWT token
        var result = jwtService.GenerateToken(user);

        // Read the information stored inside the token
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Token);

        // Check that the token contains the user's information
        Assert.Equal(user.Id, token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(user.Email, token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal(user.FullName, token.Claims.First(c => c.Type == System.Security.Claims.ClaimTypes.Name).Value);
    }
    [Fact]
public void GenerateToken_ShouldThrowException_WhenIssuerIsMissing()
{
    // Arrange
    // We intentionally leave the Issuer empty to make sure
    // the service reports the configuration problem.
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Audience"] = "CardiacPatientMonitoringClient",
            ["Jwt:SecretKey"] = "ThisIsASecretKeyForTestingJwt123456789",
            ["Jwt:ExpiryMinutes"] = "60"
        })
        .Build();

    var jwtService = new JwtService(configuration);

    var user = new ApplicationUser
    {
        Id = "test-user-id",
        Email = "test@example.com",
        FullName = "Test User"
    };

    // Act & Assert
    var exception = Assert.Throws<InvalidOperationException>(
        () => jwtService.GenerateToken(user));

    Assert.Equal(
        "JWT Issuer is not configured.",
        exception.Message);
}

[Fact]
public void GenerateToken_ShouldThrowException_WhenSecretKeyIsMissing()
{
    // Arrange
    // The secret key is required to sign the JWT.
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "CardiacPatientMonitoringApi",
            ["Jwt:Audience"] = "CardiacPatientMonitoringClient",
            ["Jwt:ExpiryMinutes"] = "60"
        })
        .Build();

    var jwtService = new JwtService(configuration);

    var user = new ApplicationUser
    {
        Id = "test-user-id",
        Email = "test@example.com",
        FullName = "Test User"
    };

    // Act & Assert
    var exception = Assert.Throws<InvalidOperationException>(
        () => jwtService.GenerateToken(user));

    Assert.Equal(
        "JWT SecretKey is not configured.",
        exception.Message);
}
}