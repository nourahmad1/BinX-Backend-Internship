using CardiacPatientMonitoring.Api.Controllers;
using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardiacPatientMonitoring.Tests;

public class PatientsControllerTests
{
    // Creates a fresh in-memory database for each test.
    // This keeps the tests independent from each other.
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    // GetPatients should return all patients from the database.
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

        var controller = new PatientsController(context);

        // Act
        var result = await controller.GetPatients();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var patients = Assert.IsAssignableFrom<
            IEnumerable<PatientResponseDto>>(okResult.Value);

        Assert.Equal(2, patients.Count());
    }

    // GetPatient should return the requested patient when the ID exists.
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

        var controller = new PatientsController(context);

        // Act
        var result = await controller.GetPatient(patient.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var response = Assert.IsType<PatientResponseDto>(okResult.Value);

        Assert.Equal(patient.Id, response.Id);
        Assert.Equal("Ahmad", response.FirstName);
        Assert.Equal("Ali", response.LastName);
    }

    // GetPatient should return 404 when the requested patient does not exist.
    [Fact]
    public async Task GetPatient_ShouldReturnNotFound_WhenPatientDoesNotExist()
    {
        // Arrange
        await using var context = CreateContext();

        var controller = new PatientsController(context);

        // Act
        var result = await controller.GetPatient(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // CreatePatient should save the patient and return 201 Created.
    [Fact]
    public async Task CreatePatient_ShouldCreatePatient()
    {
        // Arrange
        await using var context = CreateContext();

        var controller = new PatientsController(context);

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

        // Make sure the patient was actually saved in the database.
        var savedPatient = await context.Patients
            .FirstOrDefaultAsync(p => p.Id == response.Id);

        Assert.NotNull(savedPatient);
    }

    // UpdatePatient should change the existing patient's information.
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

        var controller = new PatientsController(context);

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
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var response =
            Assert.IsType<PatientResponseDto>(okResult.Value);

        Assert.Equal("Ahmad Updated", response.FirstName);
        Assert.Equal("Ali Updated", response.LastName);
        Assert.Equal("0599333333", response.PhoneNumber);
    }

    // UpdatePatient should return 404 when the patient does not exist.
    [Fact]
    public async Task UpdatePatient_ShouldReturnNotFound_WhenPatientDoesNotExist()
    {
        // Arrange
        await using var context = CreateContext();

        var controller = new PatientsController(context);

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

    // DeletePatient should remove the patient and return 204 No Content.
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

        var controller = new PatientsController(context);

        // Act
        var result = await controller.DeletePatient(patientId);

        // Assert
        Assert.IsType<NoContentResult>(result);

        // Make sure the patient was actually removed.
        var deletedPatient = await context.Patients
            .FirstOrDefaultAsync(p => p.Id == patientId);

        Assert.Null(deletedPatient);
    }

    // DeletePatient should return 404 when the patient does not exist.
    [Fact]
    public async Task DeletePatient_ShouldReturnNotFound_WhenPatientDoesNotExist()
    {
        // Arrange
        await using var context = CreateContext();

        var controller = new PatientsController(context);

        // Act
        var result = await controller.DeletePatient(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}