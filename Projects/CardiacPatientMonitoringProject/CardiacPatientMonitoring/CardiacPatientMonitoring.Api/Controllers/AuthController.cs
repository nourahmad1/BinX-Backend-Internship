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
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtService _jwtService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
    }

    // =========================================================
    // POST: api/Auth/register
    // =========================================================

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(
        RegisterDto dto)
    {
        // Check if the email is already registered
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

        // =====================================================
        // Make sure the Doctor role exists
        // =====================================================

        const string roleName = "Doctor";

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var roleResult =
                await _roleManager.CreateAsync(
                    new IdentityRole(roleName));

            if (!roleResult.Succeeded)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = "Could not create Doctor role.",
                        errors = roleResult.Errors.Select(
                            e => e.Description)
                    });
            }
        }

        // =====================================================
        // Create user
        // =====================================================

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            EmailConfirmed = true
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
        // Add user to Doctor role
        // =====================================================

        var roleAssignmentResult =
            await _userManager.AddToRoleAsync(
                user,
                roleName);

        if (!roleAssignmentResult.Succeeded)
        {
            // Remove the user if role assignment failed
            await _userManager.DeleteAsync(user);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "User was created but could not be assigned to Doctor role.",
                    errors =
                        roleAssignmentResult.Errors.Select(
                            e => e.Description)
                });
        }

        // =====================================================
        // Generate JWT
        // =====================================================

        return Ok(
            _jwtService.GenerateToken(user));
    }

    // =========================================================
    // POST: api/Auth/login
    // =========================================================

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        LoginDto dto)
    {
        var user =
            await _userManager.FindByEmailAsync(dto.Email);

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

        return Ok(
            _jwtService.GenerateToken(user));
    }

    // =========================================================
    // GET: api/Auth/me
    // =========================================================

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<object>> Me()
    {
        var user =
            await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FullName,
            Roles = await _userManager.GetRolesAsync(user)
        });
    }
}