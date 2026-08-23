using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardiacPatientMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PatientsController(AppDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // GET: api/Patients
    // Admin, Doctor, Nurse
    // =========================================================

    [HttpGet]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<ActionResult<IEnumerable<PatientResponseDto>>> GetPatients()
    {
        var patients = await _context.Patients
            .AsNoTracking()
            .OrderBy(patient => patient.LastName)
            .ThenBy(patient => patient.FirstName)
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

        return Ok(patients);
    }

    // =========================================================
    // GET: api/Patients/{id}
    // Admin, Doctor, Nurse
    // =========================================================

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
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

    // =========================================================
    // POST: api/Patients
    // Admin, Doctor
    // =========================================================

    [HttpPost]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<PatientResponseDto>> CreatePatient(
        PatientCreateDto dto)
    {
        var patient = new Patient
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            DateOfBirth = dto.DateOfBirth!.Value,
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

    // =========================================================
    // PUT: api/Patients/{id}
    // Admin, Doctor
    // =========================================================

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<PatientResponseDto>> UpdatePatient(
        int id,
        PatientUpdateDto dto)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(patient => patient.Id == id);

        if (patient is null)
        {
            return NotFound(new
            {
                message = $"Patient with ID {id} was not found."
            });
        }

        patient.FirstName = dto.FirstName.Trim();
        patient.LastName = dto.LastName.Trim();
        patient.DateOfBirth = dto.DateOfBirth!.Value;
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

    // =========================================================
    // DELETE: api/Patients/{id}
    // Admin, Doctor
    // =========================================================

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(patient => patient.Id == id);

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