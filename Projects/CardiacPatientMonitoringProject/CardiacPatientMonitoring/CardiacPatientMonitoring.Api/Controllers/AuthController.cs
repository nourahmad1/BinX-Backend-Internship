using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Entities;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CardiacPatientMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    // =========================================================
    // Register
    // =========================================================

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterDto dto)
    {
        // Check if email already exists
        var existingUser =
            await _userManager.FindByEmailAsync(dto.Email);

        if (existingUser != null)
        {
            return BadRequest(new
            {
                message = "Email is already registered."
            });
        }

        // Create user
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var result =
            await _userManager.CreateAsync(
                user,
                dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Registration failed.",
                errors = result.Errors.Select(
                    e => e.Description)
            });
        }

        // =====================================================
        // Assign default Patient role
        // =====================================================

        await _userManager.AddToRoleAsync(
            user,
            "Patient");

        return Ok(new
        {
            message = "Registration successful."
        });
    }

    // =========================================================
    // Login
    // =========================================================

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginDto dto)
    {
        // Find user
        var user =
            await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        // Check password
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

        // =====================================================
        // Generate JWT
        // =====================================================

        var tokenResult =
            await _jwtService.GenerateTokenAsync(user);

        // =====================================================
        // Get User Roles
        // =====================================================

        var roles =
            await _userManager.GetRolesAsync(user);

        // =====================================================
        // Return Login Response
        // =====================================================

        return Ok(new
        {
            message = "Login successful.",
            token = tokenResult.Token,
            expiresAt = tokenResult.ExpiresAt,
            user = new
            {
                id = user.Id,
                email = user.Email,
                roles = roles
            }
        });
    }
}