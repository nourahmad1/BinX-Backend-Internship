using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CardiacPatientMonitoring.Tests;

public class PatientsApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PatientsApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPatient_ShouldReturnNotFound_WhenPatientDoesNotExist()
    {
        // Arrange
        var patientId = 99999;

        // Act
        var response = await _client.GetAsync($"/api/Patients/{patientId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}