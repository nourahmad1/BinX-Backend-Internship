
using CardiacPatientMonitoring.Api.Controllers;
using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Entities;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CardiacPatientMonitoring.Tests;

public class AuthControllerTests
{
    // Fake JWT service.
    private readonly Mock<IJwtService> _jwtServiceMock = new();

    // Create a fresh in-memory database for each test.
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    // Create a mocked UserManager.
    private static Mock<UserManager<ApplicationUser>>
        CreateUserManagerMock()
    {
        var store =
            new Mock<IUserStore<ApplicationUser>>();

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
    // REGISTER
    // =========================================================

    [Fact]
    public async Task Register_ShouldReturnOk_WhenUserIsCreated()
    {
        // Arrange
        await using var context = CreateContext();

        var userManager = CreateUserManagerMock();

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

        userManager
            .Setup(x => x.GetRolesAsync(
                It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string>
            {
                "Doctor"
            });

        _jwtServiceMock
            .Setup(x => x.GenerateToken(
                It.IsAny<ApplicationUser>(),
                It.IsAny<IList<string>>()))
            .Returns(new AuthTokenResult
            {
                Token = "fake-jwt-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            });

        var controller = new AuthController(
            userManager.Object,
            _jwtServiceMock.Object,
            context);

        var dto = new RegisterDto
        {
            Email = "test@example.com",
            FullName = "Test User",
            Password = "Password123!",
            Role = "Doctor"
        };

        // Act
        var result = await controller.Register(dto);

        // Assert
        var okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        Assert.IsType<AuthTokenResult>(okResult.Value);

        userManager.Verify(
            x => x.AddToRoleAsync(
                It.IsAny<ApplicationUser>(),
                "Doctor"),
            Times.Once);

        userManager.Verify(
            x => x.GetRolesAsync(
                It.IsAny<ApplicationUser>()),
            Times.Once);

        _jwtServiceMock.Verify(
            x => x.GenerateToken(
                It.IsAny<ApplicationUser>(),
                It.Is<IList<string>>(
                    roles => roles.Contains("Doctor"))),
            Times.Once);
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenEmailAlreadyExists()
    {
        // Arrange
        await using var context = CreateContext();

        var userManager = CreateUserManagerMock();

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
            _jwtServiceMock.Object,
            context);

        var dto = new RegisterDto
        {
            Email = "test@example.com",
            FullName = "Test User",
            Password = "Password123!",
            Role = "Doctor"
        };

        // Act
        var result = await controller.Register(dto);

        // Assert
        Assert.IsType<ConflictObjectResult>(
            result.Result);

        userManager.Verify(
            x => x.CreateAsync(
                It.IsAny<ApplicationUser>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenRoleIsInvalid()
    {
        // Arrange
        await using var context = CreateContext();

        var userManager = CreateUserManagerMock();

        var controller = new AuthController(
            userManager.Object,
            _jwtServiceMock.Object,
            context);

        var dto = new RegisterDto
        {
            Email = "test@example.com",
            FullName = "Test User",
            Password = "Password123!",
            Role = "Admin"
        };

        // Act
        var result = await controller.Register(dto);

        // Assert
        var badRequest =
            Assert.IsType<BadRequestObjectResult>(
                result.Result);

        Assert.NotNull(badRequest.Value);

        userManager.Verify(
            x => x.CreateAsync(
                It.IsAny<ApplicationUser>(),
                It.IsAny<string>()),
            Times.Never);
    }

    // =========================================================
    // LOGIN
    // =========================================================

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        await using var context = CreateContext();

        var userManager = CreateUserManagerMock();

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
            .Setup(x => x.CheckPasswordAsync(
                user,
                "Password123!"))
            .ReturnsAsync(true);

        userManager
            .Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>
            {
                "Doctor"
            });

        _jwtServiceMock
            .Setup(x => x.GenerateToken(
                user,
                It.IsAny<IList<string>>()))
            .Returns(new AuthTokenResult
            {
                Token = "fake-jwt-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            });

        var controller = new AuthController(
            userManager.Object,
            _jwtServiceMock.Object,
            context);

        var dto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act
        var result = await controller.Login(dto);

        // Assert
        var okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        var tokenResult =
            Assert.IsType<AuthTokenResult>(
                okResult.Value);

        Assert.Equal(
            "fake-jwt-token",
            tokenResult.Token);

        userManager.Verify(
            x => x.GetRolesAsync(user),
            Times.Once);

        _jwtServiceMock.Verify(
            x => x.GenerateToken(
                user,
                It.Is<IList<string>>(
                    roles => roles.Contains("Doctor"))),
            Times.Once);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        await using var context = CreateContext();

        var userManager = CreateUserManagerMock();

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
            .Setup(x => x.CheckPasswordAsync(
                user,
                "WrongPassword"))
            .ReturnsAsync(false);

        var controller = new AuthController(
            userManager.Object,
            _jwtServiceMock.Object,
            context);

        var dto = new LoginDto
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        // Act
        var result = await controller.Login(dto);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(
            result.Result);

        userManager.Verify(
            x => x.GetRolesAsync(
                It.IsAny<ApplicationUser>()),
            Times.Never);

        _jwtServiceMock.Verify(
            x => x.GenerateToken(
                It.IsAny<ApplicationUser>(),
                It.IsAny<IList<string>>()),
            Times.Never);
    }

    // =========================================================
    // ME
    // =========================================================

    [Fact]
    public async Task Me_ShouldReturnUserInformation_WhenUserIsAuthenticated()
    {
        // Arrange
        await using var context = CreateContext();

        var userManager = CreateUserManagerMock();

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
            .ReturnsAsync(new List<string>
            {
                "Doctor"
            });

        var controller = new AuthController(
            userManager.Object,
            _jwtServiceMock.Object,
            context);

        var claims = new[]
        {
            new System.Security.Claims.Claim(
                System.Security.Claims.ClaimTypes.NameIdentifier,
                user.Id)
        };

        var identity =
            new System.Security.Claims.ClaimsIdentity(
                claims,
                "TestAuthentication");

        controller.ControllerContext =
            new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User =
                        new System.Security.Claims.ClaimsPrincipal(
                            identity)
                }
            };

        // Act
        var result = await controller.Me();

        // Assert
        var okResult =
            Assert.IsType<OkObjectResult>(
                result.Result);

        Assert.NotNull(okResult.Value);

        userManager.Verify(
            x => x.GetRolesAsync(user),
            Times.Once);
    }

    [Fact]
    public async Task Me_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        await using var context = CreateContext();

        var userManager = CreateUserManagerMock();

        userManager
            .Setup(x => x.GetUserAsync(
                It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .ReturnsAsync((ApplicationUser?)null);

        var controller = new AuthController(
            userManager.Object,
            _jwtServiceMock.Object,
            context);

        // Act
        var result = await controller.Me();

        // Assert
        Assert.IsType<UnauthorizedResult>(
            result.Result);

        userManager.Verify(
            x => x.GetRolesAsync(
                It.IsAny<ApplicationUser>()),
            Times.Never);
    }
}
