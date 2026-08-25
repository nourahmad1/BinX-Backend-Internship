
using CardiacPatientMonitoring.Api.Controllers;
using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CardiacPatientMonitoring.Tests;

public class PatientsControllerTests
{
    // Creates a fresh in-memory database for each test.
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    // Creates a mocked UserManager for unit tests.
    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();

        return new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }

    // =========================================================
    // GetPatients
    // =========================================================

    [Fact]
    public async Task GetPatients_ShouldReturnAllPatients()
    {
        // Arrange
        await using var context = CreateContext();

        context.Patients.AddRange(
            new Patient
            {
                FirstName = "Ahmad",
                LastName = "Ali",
                DateOfBirth = new DateTime(1995, 5, 10),
                Gender = "Male",
                PhoneNumber = "0599000000",
                CreatedAt = DateTime.UtcNow
            },
            new Patient
            {
                FirstName = "Sara",
                LastName = "Hassan",
                DateOfBirth = new DateTime(1998, 8, 20),
                Gender = "Female",
                PhoneNumber = "0599111111",
                CreatedAt = DateTime.UtcNow
            });

        await context.SaveChangesAsync();

        var userManagerMock = CreateUserManagerMock();

        var controller = new PatientsController(
            context,
            userManagerMock.Object);

        // Act
        var result = await controller.GetPatients();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var patients = Assert.IsAssignableFrom<
            IEnumerable<PatientResponseDto>>(okResult.Value);

        Assert.Equal(2, patients.Count());
    }

    // =========================================================
    // GetPatient
    // =========================================================

    [Fact]
    public async Task GetPatient_ShouldReturnPatient_WhenPatientExists()
    {
        // Arrange
        await using var context = CreateContext();

        var patient = new Patient
        {
            FirstName = "Ahmad",
            LastName = "Ali",
            DateOfBirth = new DateTime(1995, 5, 10),
            Gender = "Male",
            PhoneNumber = "0599000000",
            CreatedAt = DateTime.UtcNow
        };

        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var userManagerMock = CreateUserManagerMock();

        var controller = new PatientsController(
            context,
            userManagerMock.Object);

        // Act
        var result = await controller.GetPatient(patient.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<PatientResponseDto>(okResult.Value);

        Assert.Equal(patient.Id, response.Id);
        Assert.Equal("Ahmad", response.FirstName);
        Assert.Equal("Ali", response.LastName);
    }

    [Fact]
    public async Task GetPatient_ShouldReturnNotFound_WhenPatientDoesNotExist()
    {
        // Arrange
        await using var context = CreateContext();

        var userManagerMock = CreateUserManagerMock();

        var controller = new PatientsController(
            context,
            userManagerMock.Object);

        // Act
        var result = await controller.GetPatient(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // =========================================================
    // GetMyPatientProfile
    // =========================================================

    [Fact]
    public async Task GetMyPatientProfile_ShouldReturnPatient_WhenProfileIsLinked()
    {
        // Arrange
        await using var context = CreateContext();

        var userManagerMock = CreateUserManagerMock();

        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "patient@example.com",
            UserName = "patient@example.com",
            FullName = "Test Patient"
        };

        var patient = new Patient
        {
            ApplicationUserId = user.Id,
            FirstName = "Test",
            LastName = "Patient",
            DateOfBirth = new DateTime(2000, 1, 1),
            Gender = "Male",
            PhoneNumber = "0599000000",
            CreatedAt = DateTime.UtcNow
        };

        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        userManagerMock
            .Setup(x => x.GetUserAsync(
                It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var controller = new PatientsController(
            context,
            userManagerMock.Object);

        // Act
        var result = await controller.GetMyPatientProfile();

        // Assert
        var okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<PatientResponseDto>(okResult.Value);

        Assert.Equal(patient.Id, response.Id);
        Assert.Equal("Test", response.FirstName);
        Assert.Equal("Patient", response.LastName);

        userManagerMock.Verify(
            x => x.GetUserAsync(
                It.IsAny<System.Security.Claims.ClaimsPrincipal>()),
            Times.Once);
    }

    [Fact]
    public async Task GetMyPatientProfile_ShouldReturnNotFound_WhenProfileIsNotLinked()
    {
        // Arrange
        await using var context = CreateContext();

        var userManagerMock = CreateUserManagerMock();

        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "patient@example.com",
            UserName = "patient@example.com",
            FullName = "Test Patient"
        };

        userManagerMock
            .Setup(x => x.GetUserAsync(
                It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var controller = new PatientsController(
            context,
            userManagerMock.Object);

        // Act
        var result = await controller.GetMyPatientProfile();

        // Assert
        var notFoundResult =
            Assert.IsType<NotFoundObjectResult>(result.Result);

        Assert.NotNull(notFoundResult.Value);
    }

    [Fact]
    public async Task GetMyPatientProfile_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        await using var context = CreateContext();

        var userManagerMock = CreateUserManagerMock();

        userManagerMock
            .Setup(x => x.GetUserAsync(
                It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .ReturnsAsync((ApplicationUser?)null);

        var controller = new PatientsController(
            context,
            userManagerMock.Object);

        // Act
        var result = await controller.GetMyPatientProfile();

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    // =========================================================
    // CreatePatient
    // =========================================================

    [Fact]
    public async Task CreatePatient_ShouldCreatePatient()
    {
        // Arrange
        await using var context = CreateContext();

        var userManagerMock = CreateUserManagerMock();

        var controller = new PatientsController(
            context,
            userManagerMock.Object);

        var dto = new PatientCreateDto
        {
            FirstName = "Omar",
            LastName = "Khaled",
            DateOfBirth = new DateTime(1997, 3, 15),
            Gender = "Male",
            PhoneNumber = "0599222222"
        };

        // Act
        var result = await controller.CreatePatient(dto);

        // Assert
        var createdResult =
            Assert.IsType<CreatedAtActionResult>(result.Result);

        var response =
            Assert.IsType<PatientResponseDto>(createdResult.Value);

        Assert.True(response.Id > 0);
        Assert.Equal("Omar", response.FirstName);
        Assert.Equal("Khaled", response.LastName);

        var savedPatient = await context.Patients
            .FirstOrDefaultAsync(p => p.Id == response.Id);

        Assert.NotNull(savedPatient);
    }

    // =========================================================
    // UpdatePatient
    // =========================================================

    [Fact]
    public async Task UpdatePatient_ShouldUpdatePatient_WhenPatientExists()
    {
        // Arrange
        await using var context = CreateContext();

        var patient = new Patient
        {
            FirstName = "Ahmad",
            LastName = "Ali",
            DateOfBirth = new DateTime(1995, 5, 10),
            Gender = "Male",
            PhoneNumber = "0599000000",
            CreatedAt = DateTime.UtcNow
        };

        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var userManagerMock = CreateUserManagerMock();

        var controller = new PatientsController(
            context,
            userManagerMock.Object);

        var dto = new PatientUpdateDto
        {
            FirstName = "Ahmad Updated",
            LastName = "Ali Updated",
            DateOfBirth = new DateTime(1995, 5, 10),
            Gender = "Male",
            PhoneNumber = "0599333333"
        };

        // Act
        var result = await controller.UpdatePatient(
            patient.Id,
            dto);

        // Assert
        var okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<PatientResponseDto>(okResult.Value);

        Assert.Equal("Ahmad Updated", response.FirstName);
        Assert.Equal("Ali Updated", response.LastName);
        Assert.Equal("0599333333", response.PhoneNumber);
    }

    [Fact]
    public async Task UpdatePatient_ShouldReturnNotFound_WhenPatientDoesNotExist()
    {
        // Arrange
        await using var context = CreateContext();

        var userManagerMock = CreateUserManagerMock();

        var controller = new PatientsController(
            context,
            userManagerMock.Object);

        var dto = new PatientUpdateDto
        {
            FirstName = "Updated",
            LastName = "Patient",
            DateOfBirth = new DateTime(2000, 1, 1),
            Gender = "Male",
            PhoneNumber = "0599444444"
        };

        // Act
        var result = await controller.UpdatePatient(999, dto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // =========================================================
    // DeletePatient
    // =========================================================

    [Fact]
    public async Task DeletePatient_ShouldReturnNoContent_WhenPatientExists()
    {
        // Arrange
        await using var context = CreateContext();

        var patient = new Patient
        {
            FirstName = "Sara",
            LastName = "Hassan",
            DateOfBirth = new DateTime(1998, 8, 20),
            Gender = "Female",
            PhoneNumber = "0599111111",
            CreatedAt = DateTime.UtcNow
        };

        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        var patientId = patient.Id;

        var userManagerMock = CreateUserManagerMock();

        var controller = new PatientsController(
            context,
            userManagerMock.Object);

        // Act
        var result = await controller.DeletePatient(patientId);

        // Assert
        Assert.IsType<NoContentResult>(result);

        var deletedPatient = await context.Patients
            .FirstOrDefaultAsync(p => p.Id == patientId);

        Assert.Null(deletedPatient);
    }

    [Fact]
    public async Task DeletePatient_ShouldReturnNotFound_WhenPatientDoesNotExist()
    {
        // Arrange
        await using var context = CreateContext();

        var userManagerMock = CreateUserManagerMock();

        var controller = new PatientsController(
            context,
            userManagerMock.Object);

        // Act
        var result = await controller.DeletePatient(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
