
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

    // Get all patients
    [HttpGet]
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

    // Get one patient by ID
    [HttpGet("{id:int}")]
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
    [HttpPost]
    public async Task<ActionResult<PatientResponseDto>> CreatePatient(
        PatientCreateDto dto)
    {
        var patient = new Patient
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            PhoneNumber = dto.PhoneNumber,
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

        // Return the new patient with its generated ID
        return CreatedAtAction(
            nameof(GetPatient),
            new { id = patient.Id },
            response);
    }

    // Update an existing patient
    [HttpPut("{id:int}")]
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

        patient.FirstName = dto.FirstName;
        patient.LastName = dto.LastName;
        patient.DateOfBirth = dto.DateOfBirth;
        patient.Gender = dto.Gender;
        patient.PhoneNumber = dto.PhoneNumber;

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

    // Delete an existing patient
    [HttpDelete("{id:int}")]
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

