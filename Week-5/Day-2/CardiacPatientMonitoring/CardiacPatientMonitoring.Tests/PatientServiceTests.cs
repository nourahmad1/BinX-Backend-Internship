using CardiacPatientMonitoring.Api.Entities;
using CardiacPatientMonitoring.Api.Services;
using Moq;

namespace CardiacPatientMonitoring.Tests;

public class PatientServiceTests
{
    [Fact]
    public async Task GetPatientFullNameAsync_ShouldReturnFullName_WhenPatientExists()
    {
        // Arrange
        // Create a fake repository instead of using the real database.
        var mockRepository = new Mock<IPatientRepository>();

        var patient = new Patient
        {
            Id = 1,
            FirstName = "Ahmad",
            LastName = "Ali"
        };

        // Tell the mock what to return when the service asks for patient 1.
        mockRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(patient);

        var service = new PatientService(mockRepository.Object);

        // Act
        // Call the real service method we want to test.
        var result = await service.GetPatientFullNameAsync(1);

        // Assert
        // The service should combine the first and last name correctly.
        Assert.Equal("Ahmad Ali", result);
    }
    [Fact]
public async Task GetPatientFullNameAsync_ShouldReturnNull_WhenRepositoryThrowsException()
{
    // Arrange
    // Create a fake repository.
    var mockRepository = new Mock<IPatientRepository>();

    // Tell the mock to simulate a repository failure.
    mockRepository
        .Setup(repository => repository.GetByIdAsync(1))
        .ThrowsAsync(new InvalidOperationException("Database error"));

    var service = new PatientService(mockRepository.Object);

    // Act
    // The service should handle the exception instead of throwing it.
    var result = await service.GetPatientFullNameAsync(1);

    // Assert
    // When the repository fails, the service should return null.
    Assert.Null(result);
}
[Fact]
public async Task GetPatientFullNameAsync_ShouldCallRepositoryOnce()
{
    // Arrange
    var mockRepository = new Mock<IPatientRepository>();

    var patient = new Patient
    {
        Id = 1,
        FirstName = "Ahmad",
        LastName = "Ali"
    };

    mockRepository
        .Setup(repository => repository.GetByIdAsync(1))
        .ReturnsAsync(patient);

    var service = new PatientService(mockRepository.Object);

    // Act
    await service.GetPatientFullNameAsync(1);

    // Assert
    // Make sure the repository was called exactly once.
    mockRepository.Verify(
        repository => repository.GetByIdAsync(1),
        Times.Once);
}
}