using CardiacPatientMonitoring.Api.Controllers;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Entities;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CardiacPatientMonitoring.Tests;

public class AuthControllerTests
{
    // Use a fake JWT service so these tests focus on the controller logic.
    private readonly Mock<IJwtService> _jwtServiceMock = new();

    // A new user should be registered successfully.
    [Fact]
    public async Task Register_ShouldReturnOk_WhenUserIsCreated()
    {
        // Arrange
        var store = new Mock<IUserStore<ApplicationUser>>();

        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        var user = new ApplicationUser
        {
            Id = "user-1",
            Email = "test@example.com",
            UserName = "test@example.com",
            FullName = "Test User"
        };

        userManager
            .Setup(x => x.FindByEmailAsync("test@example.com"))
            .ReturnsAsync((ApplicationUser?)null);

        userManager
            .Setup(x => x.CreateAsync(
                It.IsAny<ApplicationUser>(),
                "Password123!"))
            .ReturnsAsync(IdentityResult.Success);

        userManager
            .Setup(x => x.AddToRoleAsync(
                It.IsAny<ApplicationUser>(),
                "Doctor"))
            .ReturnsAsync(IdentityResult.Success);

        _jwtServiceMock
            .Setup(x => x.GenerateToken(It.IsAny<ApplicationUser>()))
            .Returns(new AuthTokenResult
            {
                Token = "fake-jwt-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            });

        var controller = new AuthController(
            userManager.Object,
            _jwtServiceMock.Object);

        var dto = new RegisterDto
        {
            Email = "test@example.com",
            FullName = "Test User",
            Password = "Password123!"
        };

        // Act
        var result = await controller.Register(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        Assert.IsType<AuthTokenResult>(okResult.Value);
    }

    // Registration should fail when the email is already registered.
    [Fact]
    public async Task Register_ShouldReturnConflict_WhenEmailAlreadyExists()
    {
        // Arrange
        var store = new Mock<IUserStore<ApplicationUser>>();

        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        var existingUser = new ApplicationUser
        {
            Id = "existing-user",
            Email = "test@example.com",
            UserName = "test@example.com"
        };

        userManager
            .Setup(x => x.FindByEmailAsync("test@example.com"))
            .ReturnsAsync(existingUser);

        var controller = new AuthController(
            userManager.Object,
            _jwtServiceMock.Object);

        var dto = new RegisterDto
        {
            Email = "test@example.com",
            FullName = "Test User",
            Password = "Password123!"
        };

        // Act
        var result = await controller.Register(dto);

        // Assert
        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    // Login should return a JWT token when the credentials are correct.
    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        var store = new Mock<IUserStore<ApplicationUser>>();

        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        var user = new ApplicationUser
        {
            Id = "user-1",
            Email = "test@example.com",
            UserName = "test@example.com",
            FullName = "Test User"
        };

        userManager
            .Setup(x => x.FindByEmailAsync("test@example.com"))
            .ReturnsAsync(user);

        userManager
            .Setup(x => x.CheckPasswordAsync(user, "Password123!"))
            .ReturnsAsync(true);

        _jwtServiceMock
            .Setup(x => x.GenerateToken(user))
            .Returns(new AuthTokenResult
            {
                Token = "fake-jwt-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            });

        var controller = new AuthController(
            userManager.Object,
            _jwtServiceMock.Object);

        var dto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act
        var result = await controller.Login(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        var tokenResult = Assert.IsType<AuthTokenResult>(okResult.Value);

        Assert.Equal("fake-jwt-token", tokenResult.Token);
    }

    // Login should return 401 when the password is incorrect.
    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var store = new Mock<IUserStore<ApplicationUser>>();

        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        var user = new ApplicationUser
        {
            Id = "user-1",
            Email = "test@example.com",
            UserName = "test@example.com"
        };

        userManager
            .Setup(x => x.FindByEmailAsync("test@example.com"))
            .ReturnsAsync(user);

        userManager
            .Setup(x => x.CheckPasswordAsync(user, "WrongPassword"))
            .ReturnsAsync(false);

        var controller = new AuthController(
            userManager.Object,
            _jwtServiceMock.Object);

        var dto = new LoginDto
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        // Act
        var result = await controller.Login(dto);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result.Result);

        // No token should be generated when login fails.
        _jwtServiceMock.Verify(
            x => x.GenerateToken(It.IsAny<ApplicationUser>()),
            Times.Never);
    }

    // Me should return the current user's information when the user exists.
    [Fact]
    public async Task Me_ShouldReturnUserInformation_WhenUserIsAuthenticated()
    {
        // Arrange
        var store = new Mock<IUserStore<ApplicationUser>>();

        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        var user = new ApplicationUser
        {
            Id = "user-1",
            Email = "test@example.com",
            UserName = "test@example.com",
            FullName = "Test User"
        };

        userManager
            .Setup(x => x.GetUserAsync(
                It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .ReturnsAsync(user);

        userManager
            .Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "Doctor" });

        var controller = new AuthController(
            userManager.Object,
            _jwtServiceMock.Object);

        // Simulate an authenticated request.
        var claims = new[]
        {
            new System.Security.Claims.Claim(
                System.Security.Claims.ClaimTypes.NameIdentifier,
                user.Id)
        };

        var identity = new System.Security.Claims.ClaimsIdentity(
            claims,
            "TestAuthentication");

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new System.Security.Claims.ClaimsPrincipal(identity)
            }
        };

        // Act
        var result = await controller.Me();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        Assert.NotNull(okResult.Value);

        // Make sure the controller also loads the user's roles.
        userManager.Verify(
            x => x.GetRolesAsync(user),
            Times.Once);
    }

    // Me should return 401 when the user cannot be found.
    [Fact]
    public async Task Me_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        var store = new Mock<IUserStore<ApplicationUser>>();

        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        // Simulate an authenticated request where the user no longer exists.
        userManager
            .Setup(x => x.GetUserAsync(
                It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .ReturnsAsync((ApplicationUser?)null);

        var controller = new AuthController(
            userManager.Object,
            _jwtServiceMock.Object);

        // Act
        var result = await controller.Me();

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }
}