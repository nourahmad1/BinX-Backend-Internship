using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using CardiacPatientMonitoring.Api.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace CardiacPatientMonitoring.Tests;

public class PatientsApiIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PatientsApiIntegrationTests(
        CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // =========================================================
    // GET: api/Patients/{id}
    // Happy path
    // =========================================================

    [Fact]
    public async Task GetPatient_ReturnsPatient_WhenPatientExists()
    {
        // Arrange
        const int patientId = 1;

        var token = CreateTestJwt(
            userId: "test-admin-id",
            email: "admin@test.com",
            fullName: "Test Admin",
            role: "Admin");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        // Act
        var response =
            await _client.GetAsync(
                $"/api/Patients/{patientId}");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var patient =
            await response.Content
                .ReadFromJsonAsync<PatientResponseDto>();

        Assert.NotNull(patient);

        Assert.Equal(
            1,
            patient.Id);

        Assert.Equal(
            "Ahmad",
            patient.FirstName);

        Assert.Equal(
            "Hassan",
            patient.LastName);

        Assert.Equal(
            new DateTime(1985, 4, 12),
            patient.DateOfBirth);

        Assert.Equal(
            "Male",
            patient.Gender);

        Assert.Equal(
            "0599000001",
            patient.PhoneNumber);

        Assert.NotEqual(
            default,
            patient.CreatedAt);
    }

    // =========================================================
    // GET: api/Patients/{id}
    // Not found
    // =========================================================

    [Fact]
    public async Task GetPatient_ReturnsNotFound_WhenPatientDoesNotExist()
    {
        // Arrange
        const int patientId = 99999;

        var token = CreateTestJwt(
            userId: "test-admin-id",
            email: "admin@test.com",
            fullName: "Test Admin",
            role: "Admin");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        // Act
        var response =
            await _client.GetAsync(
                $"/api/Patients/{patientId}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    // =========================================================
    // Protected endpoint
    // Valid JWT + Admin role
    // =========================================================

    [Fact]
    public async Task GetPatient_ReturnsOk_WhenValidAdminJwtIsProvided()
    {
        // Arrange
        const int patientId = 1;

        var token = CreateTestJwt(
            userId: "test-admin-id",
            email: "admin@test.com",
            fullName: "Test Admin",
            role: "Admin");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        // Act
        var response =
            await _client.GetAsync(
                $"/api/Patients/{patientId}");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    // =========================================================
    // Create a real JWT for integration testing
    // =========================================================

    private static string CreateTestJwt(
        string userId,
        string email,
        string fullName,
        string role)
    {
        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                userId),

            new Claim(
                JwtRegisteredClaimNames.Email,
                email),

            new Claim(
                ClaimTypes.NameIdentifier,
                userId),

            new Claim(
                ClaimTypes.Email,
                email),

            new Claim(
                ClaimTypes.Name,
                fullName),

            new Claim(
                ClaimTypes.Role,
                role)
        };

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    CustomWebApplicationFactory
                        .TestSecretKey));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer:
                    CustomWebApplicationFactory
                        .TestIssuer,

                audience:
                    CustomWebApplicationFactory
                        .TestAudience,

                claims:
                    claims,

                expires:
                    DateTime.UtcNow.AddMinutes(30),

                signingCredentials:
                    credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}