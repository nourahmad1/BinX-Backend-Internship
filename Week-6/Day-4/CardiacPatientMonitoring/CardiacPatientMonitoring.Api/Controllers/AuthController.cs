
using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Entities;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
    // Public registration
    // Only Doctor and Patient can register
    // =========================================================
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult> Register(
        RegisterDto dto)
    {
        var normalizedRole = dto.Role.Trim().ToUpper();

        // Admin cannot be registered publicly
        if (normalizedRole != "DOCTOR" &&
            normalizedRole != "PATIENT")
        {
            return BadRequest(new
            {
                message =
                    "Only DOCTOR or PATIENT can register."
            });
        }

        // Check if email already exists
        var existingUser =
            await _userManager.FindByEmailAsync(
                dto.Email);

        if (existingUser is not null)
        {
            return Conflict(new
            {
                message =
                    "A user with this email already exists."
            });
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            EmailConfirmed = true
        };

        var createResult =
            await _userManager.CreateAsync(
                user,
                dto.Password);

        if (!createResult.Succeeded)
        {
            return BadRequest(new
            {
                message = "User registration failed.",
                errors = createResult.Errors.Select(
                    error => error.Description)
            });
        }

        // Assign selected role
        var roleResult =
            await _userManager.AddToRoleAsync(
                user,
                normalizedRole);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return BadRequest(new
            {
                message = "Could not assign user role.",
                errors = roleResult.Errors.Select(
                    error => error.Description)
            });
        }

        // Create Patient profile and link it to Identity user
        if (normalizedRole == "PATIENT")
        {
            var patient = new Patient
            {
                ApplicationUserId = user.Id,
                FirstName = dto.FullName,
                LastName = "",
                DateOfBirth = DateTime.UtcNow,
                Gender = "",
                PhoneNumber = "",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }

        var roles =
            await _userManager.GetRolesAsync(user);

        var token =
            _jwtService.GenerateToken(
                user,
                roles);

        return Ok(new
        {
            message = "Registration successful.",
            token,
            user = new
            {
                id = user.Id,
                email = user.Email,
                fullName = user.FullName,
                roles
            }
        });
    }

    // =========================================================
    // POST: api/Auth/login
    // =========================================================
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult> Login(
        LoginDto dto)
    {
        var user =
            await _userManager.FindByEmailAsync(
                dto.Email);

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var passwordValid =
            await _userManager.CheckPasswordAsync(
                user,
                dto.Password);

        if (!passwordValid)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var roles =
            await _userManager.GetRolesAsync(user);

        var token =
            _jwtService.GenerateToken(
                user,
                roles);

        return Ok(new
        {
            message = "Login successful.",
            token,
            user = new
            {
                id = user.Id,
                email = user.Email,
                fullName = user.FullName,
                roles
            }
        });
    }

    // =========================================================
    // GET: api/Auth/me
    // Authenticated users can view their own account
    // =========================================================
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult> Me()
    {
        var user =
            await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "User account was not found."
            });
        }

        var roles =
            await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            id = user.Id,
            email = user.Email,
            fullName = user.FullName,
            roles
        });
    }
}

