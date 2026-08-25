
using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Entities;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardiacPatientMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly AppDbContext _context;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService,
        AppDbContext context)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _context = context;
    }

    // =========================================================
    // POST: api/Auth/register
    // =========================================================
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(
        RegisterDto dto)
    {
        // Only Doctor and Patient can register publicly.
        // Admin accounts should not be created through this endpoint.
        var allowedRoles = new[] { "Doctor", "Patient" };

        if (!allowedRoles.Contains(dto.Role))
        {
            return BadRequest(new
            {
                message =
                    "Invalid role. Only Doctor or Patient can register."
            });
        }

        // Check if an account with this email already exists.
        var existing =
            await _userManager.FindByEmailAsync(dto.Email);

        if (existing is not null)
        {
            return Conflict(new
            {
                message =
                    "An account with this email already exists."
            });
        }

        // Create the Identity user.
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(
            user,
            dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Registration failed.",
                errors = result.Errors.Select(
                    error => error.Description)
            });
        }

        // Assign the selected role.
        var roleResult =
            await _userManager.AddToRoleAsync(
                user,
                dto.Role);

        if (!roleResult.Succeeded)
        {
            // Remove the user if the role could not be assigned.
            await _userManager.DeleteAsync(user);

            return BadRequest(new
            {
                message = "Could not assign the selected role.",
                errors = roleResult.Errors.Select(
                    error => error.Description)
            });
        }

        // =====================================================
        // If the registered user is a Patient,
        // create and link a Patient profile.
        // =====================================================
        if (dto.Role == "Patient")
        {
            // Split the full name into first name and last name.
            var nameParts = dto.FullName
                .Trim()
                .Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries);

            var firstName =
                nameParts.Length > 0
                    ? nameParts[0]
                    : string.Empty;

            var lastName =
                nameParts.Length > 1
                    ? string.Join(" ", nameParts.Skip(1))
                    : string.Empty;

            var patient = new Patient
            {
                // This is the important link.
                ApplicationUserId = user.Id,

                FirstName = firstName,
                LastName = lastName,

                // These values can be completed later
                // through the patient profile update.
                DateOfBirth = DateTime.MinValue,
                Gender = string.Empty,
                PhoneNumber = string.Empty,

                CreatedAt = DateTime.UtcNow
            };

            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }

        // Get the user's roles.
        var roles =
            await _userManager.GetRolesAsync(user);

        // Generate JWT containing the user's roles.
        return Ok(
            _jwtService.GenerateToken(
                user,
                roles));
    }

    // =========================================================
    // POST: api/Auth/login
    // =========================================================
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        LoginDto dto)
    {
        // Find the user by email.
        var user =
            await _userManager.FindByEmailAsync(dto.Email);

        // Validate credentials.
        if (user is null ||
            !await _userManager.CheckPasswordAsync(
                user,
                dto.Password))
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        // Get the roles assigned to the user.
        var roles =
            await _userManager.GetRolesAsync(user);

        // Generate JWT containing the user's roles.
        return Ok(
            _jwtService.GenerateToken(
                user,
                roles));
    }

    // =========================================================
    // GET: api/Auth/me
    // =========================================================
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<object>> Me()
    {
        // Get the currently authenticated user.
        var user =
            await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Unauthorized();
        }

        // Get the user's roles.
        var roles =
            await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FullName,
            Roles = roles
        });
    }
}
