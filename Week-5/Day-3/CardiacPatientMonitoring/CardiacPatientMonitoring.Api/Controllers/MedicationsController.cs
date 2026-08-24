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
public class MedicationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public MedicationsController(AppDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // GET: api/Medications/patient/{patientId}
    // Admin, Doctor, and Patient can view medications
    // =========================================================
    [HttpGet("patient/{patientId:int}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<ActionResult<IEnumerable<MedicationResponseDto>>>
        GetPatientMedications(
            int patientId,
            [FromQuery] string? search = null)
    {
        // Check if the patient exists
        var patientExists = await _context.Patients
            .AsNoTracking()
            .AnyAsync(patient => patient.Id == patientId);

        if (!patientExists)
        {
            return NotFound(new
            {
                message = $"Patient with ID {patientId} was not found."
            });
        }

        var query = _context.Medications
            .AsNoTracking()
            .Where(medication => medication.PatientId == patientId);

        // Search by medication name, dosage, or frequency
        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();

            query = query.Where(medication =>
                medication.Name.Contains(normalizedSearch) ||
                medication.Dosage.Contains(normalizedSearch) ||
                medication.Frequency.Contains(normalizedSearch));
        }

        // Newest medications first
        var medications = await query
            .OrderByDescending(medication => medication.StartDate)
            .Select(medication => new MedicationResponseDto
            {
                Id = medication.Id,
                PatientId = medication.PatientId,
                Name = medication.Name,
                Dosage = medication.Dosage,
                Frequency = medication.Frequency,
                StartDate = medication.StartDate,
                EndDate = medication.EndDate,
                Notes = medication.Notes
            })
            .ToListAsync();

        return Ok(medications);
    }

    // =========================================================
    // GET: api/Medications/{id}
    // Admin, Doctor, and Patient can view one medication
    // =========================================================
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<ActionResult<MedicationResponseDto>>
        GetMedication(int id)
    {
        var medication = await _context.Medications
            .AsNoTracking()
            .Where(medication => medication.Id == id)
            .Select(medication => new MedicationResponseDto
            {
                Id = medication.Id,
                PatientId = medication.PatientId,
                Name = medication.Name,
                Dosage = medication.Dosage,
                Frequency = medication.Frequency,
                StartDate = medication.StartDate,
                EndDate = medication.EndDate,
                Notes = medication.Notes
            })
            .FirstOrDefaultAsync();

        if (medication is null)
        {
            return NotFound(new
            {
                message = $"Medication with ID {id} was not found."
            });
        }

        return Ok(medication);
    }

    // =========================================================
    // POST: api/Medications
    // Only Admin and Doctor can create medications
    // =========================================================
    [HttpPost]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<MedicationResponseDto>>
        CreateMedication(MedicationCreateDto dto)
    {
        // Make sure the patient exists
        var patientExists = await _context.Patients
            .AsNoTracking()
            .AnyAsync(patient => patient.Id == dto.PatientId);

        if (!patientExists)
        {
            return NotFound(new
            {
                message =
                    $"Patient with ID {dto.PatientId} was not found."
            });
        }

        var medication = new Medication
        {
            PatientId = dto.PatientId,
            Name = dto.Name.Trim(),
            Dosage = dto.Dosage.Trim(),
            Frequency = dto.Frequency.Trim(),
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Notes = string.IsNullOrWhiteSpace(dto.Notes)
                ? null
                : dto.Notes.Trim()
        };

        await _context.Medications.AddAsync(medication);
        await _context.SaveChangesAsync();

        var response = ToDto(medication);

        return CreatedAtAction(
            nameof(GetMedication),
            new { id = medication.Id },
            response);
    }

    // =========================================================
    // PUT: api/Medications/{id}
    // Only Admin and Doctor can update medications
    // =========================================================
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<MedicationResponseDto>>
        UpdateMedication(
            int id,
            MedicationUpdateDto dto)
    {
        var medication = await _context.Medications
            .FirstOrDefaultAsync(
                medication => medication.Id == id);

        if (medication is null)
        {
            return NotFound(new
            {
                message =
                    $"Medication with ID {id} was not found."
            });
        }

        medication.Name = dto.Name.Trim();
        medication.Dosage = dto.Dosage.Trim();
        medication.Frequency = dto.Frequency.Trim();
        medication.StartDate = dto.StartDate;
        medication.EndDate = dto.EndDate;

        medication.Notes =
            string.IsNullOrWhiteSpace(dto.Notes)
                ? null
                : dto.Notes.Trim();

        await _context.SaveChangesAsync();

        return Ok(ToDto(medication));
    }

    // =========================================================
    // DELETE: api/Medications/{id}
    // Only Admin and Doctor can delete medications
    // =========================================================
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> DeleteMedication(int id)
    {
        var medication = await _context.Medications
            .FirstOrDefaultAsync(
                medication => medication.Id == id);

        if (medication is null)
        {
            return NotFound(new
            {
                message =
                    $"Medication with ID {id} was not found."
            });
        }

        _context.Medications.Remove(medication);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // =========================================================
    // Helper method
    // =========================================================
    private static MedicationResponseDto ToDto(
        Medication medication)
    {
        return new MedicationResponseDto
        {
            Id = medication.Id,
            PatientId = medication.PatientId,
            Name = medication.Name,
            Dosage = medication.Dosage,
            Frequency = medication.Frequency,
            StartDate = medication.StartDate,
            EndDate = medication.EndDate,
            Notes = medication.Notes
        };
    }
}