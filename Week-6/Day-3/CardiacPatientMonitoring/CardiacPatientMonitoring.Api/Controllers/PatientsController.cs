
using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardiacPatientMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PatientsController(
        AppDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Get patients with pagination, search, gender filter, and sorting
    // Admin and Doctor can view all patients
    [HttpGet]
    [Authorize(Roles = "ADMIN,DOCTOR")]
    public async Task<ActionResult> GetPatients(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? gender = null,
        [FromQuery] string? sort = null)
    {
        // Make sure page values are valid
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        var query = _context.Patients
            .AsNoTracking()
            .AsQueryable();

        // Search by first name, last name, or phone number
        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();

            query = query.Where(patient =>
                patient.FirstName.Contains(normalizedSearch) ||
                patient.LastName.Contains(normalizedSearch) ||
                patient.PhoneNumber.Contains(normalizedSearch));
        }

        // Filter by gender
        if (!string.IsNullOrWhiteSpace(gender))
        {
            var normalizedGender = gender.Trim();

            query = query.Where(patient =>
                patient.Gender == normalizedGender);
        }

        // Sort patients
        if (!string.IsNullOrWhiteSpace(sort))
        {
            var normalizedSort = sort.Trim().ToLower();

            query = normalizedSort switch
            {
                "firstname" => query
                    .OrderBy(patient => patient.FirstName),

                "lastname" => query
                    .OrderBy(patient => patient.LastName)
                    .ThenBy(patient => patient.FirstName),

                "createdat" => query
                    .OrderBy(patient => patient.CreatedAt),

                _ => query
                    .OrderBy(patient => patient.LastName)
                    .ThenBy(patient => patient.FirstName)
            };
        }
        else
        {
            // Default sorting by last name and then first name
            query = query
                .OrderBy(patient => patient.LastName)
                .ThenBy(patient => patient.FirstName);
        }

        // Count patients after applying the filters
        var totalCount = await query.CountAsync();

        // Calculate the total number of pages
        var totalPages = (int)Math.Ceiling(
            totalCount / (double)pageSize);

        // Get only the patients for the requested page
        var patients = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(patient => new PatientResponseDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                PhoneNumber = patient.PhoneNumber,
                CreatedAt = patient.CreatedAt
            })
            .ToListAsync();

        return Ok(new
        {
            page,
            pageSize,
            totalCount,
            totalPages,
            data = patients
        });
    }

    // Patient can view their own profile
    [HttpGet("me")]
    [Authorize(Roles = "PATIENT")]
    public async Task<ActionResult<PatientResponseDto>> GetMyPatientProfile()
    {
        // Get the currently authenticated Identity user
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Unauthorized();
        }

        // Find the patient profile linked to this user
        var patient = await _context.Patients
            .AsNoTracking()
            .Where(patient =>
                patient.ApplicationUserId == user.Id)
            .Select(patient => new PatientResponseDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                PhoneNumber = patient.PhoneNumber,
                CreatedAt = patient.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (patient is null)
        {
            return NotFound(new
            {
                message = "No patient profile is linked to your account."
            });
        }

        return Ok(patient);
    }

    // Get one patient by ID
    // Admin and Doctor can view any patient
    [HttpGet("{id:int}")]
    [Authorize(Roles = "ADMIN,DOCTOR")]
    public async Task<ActionResult<PatientResponseDto>> GetPatient(int id)
    {
        var patient = await _context.Patients
            .AsNoTracking()
            .Where(patient => patient.Id == id)
            .Select(patient => new PatientResponseDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                PhoneNumber = patient.PhoneNumber,
                CreatedAt = patient.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (patient is null)
        {
            return NotFound(new
            {
                message = $"Patient with ID {id} was not found."
            });
        }

        return Ok(patient);
    }

    // Create a new patient
    // Admin and Doctor can create patients
    [HttpPost]
    [Authorize(Roles = "ADMIN,DOCTOR")]
    public async Task<ActionResult<PatientResponseDto>> CreatePatient(
        PatientCreateDto dto)
    {
        var patient = new Patient
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _context.Patients.AddAsync(patient);
        await _context.SaveChangesAsync();

        var response = new PatientResponseDto
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            PhoneNumber = patient.PhoneNumber,
            CreatedAt = patient.CreatedAt
        };

        return CreatedAtAction(
            nameof(GetPatient),
            new { id = patient.Id },
            response);
    }

    // Update an existing patient
    // Admin and Doctor can update patients
    [HttpPut("{id:int}")]
    [Authorize(Roles = "ADMIN,DOCTOR")]
    public async Task<ActionResult<PatientResponseDto>> UpdatePatient(
        int id,
        PatientUpdateDto dto)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(
                patient => patient.Id == id);

        if (patient is null)
        {
            return NotFound(new
            {
                message = $"Patient with ID {id} was not found."
            });
        }

        patient.FirstName = dto.FirstName.Trim();
        patient.LastName = dto.LastName.Trim();
        patient.DateOfBirth = dto.DateOfBirth;
        patient.Gender = dto.Gender.Trim();
        patient.PhoneNumber = dto.PhoneNumber.Trim();

        await _context.SaveChangesAsync();

        var response = new PatientResponseDto
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            PhoneNumber = patient.PhoneNumber,
            CreatedAt = patient.CreatedAt
        };

        return Ok(response);
    }

    // Delete a patient
    // Only Admin can delete patients
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(
                patient => patient.Id == id);

        if (patient is null)
        {
            return NotFound(new
            {
                message = $"Patient with ID {id} was not found."
            });
        }

        _context.Patients.Remove(patient);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
