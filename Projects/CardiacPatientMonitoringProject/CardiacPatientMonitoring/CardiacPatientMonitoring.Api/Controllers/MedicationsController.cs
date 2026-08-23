
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

    // Get medications for a specific patient
    [HttpGet("patient/{patientId:int}")]
    public async Task<ActionResult<IEnumerable<MedicationResponseDto>>>
        GetPatientMedications(
            int patientId,
            [FromQuery] string? search = null)
    {
        var patientExists = await _context.Patients
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

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(medication =>
                medication.Name.Contains(search) ||
                medication.Dosage.Contains(search) ||
                medication.Frequency.Contains(search));
        }

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

    // Get one medication by ID
    [HttpGet("{id:int}")]
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

    // Create a new medication
    [HttpPost]
    public async Task<ActionResult<MedicationResponseDto>>
        CreateMedication(MedicationCreateDto dto)
    {
        var patientExists = await _context.Patients
            .AnyAsync(patient => patient.Id == dto.PatientId);

        if (!patientExists)
        {
            return NotFound(new
            {
                message = $"Patient with ID {dto.PatientId} was not found."
            });
        }

        var medication = new Medication
        {
            PatientId = dto.PatientId,
            Name = dto.Name,
            Dosage = dto.Dosage,
            Frequency = dto.Frequency,
            StartDate = dto.StartDate!.Value,
            EndDate = dto.EndDate,
            Notes = dto.Notes
        };

        await _context.Medications.AddAsync(medication);
        await _context.SaveChangesAsync();

        var response = new MedicationResponseDto
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

        return CreatedAtAction(
            nameof(GetMedication),
            new { id = medication.Id },
            response);
    }

    // Update an existing medication
    [HttpPut("{id:int}")]
    public async Task<ActionResult<MedicationResponseDto>>
        UpdateMedication(
            int id,
            MedicationUpdateDto dto)
    {
        var medication = await _context.Medications
            .FirstOrDefaultAsync(medication => medication.Id == id);

        if (medication is null)
        {
            return NotFound(new
            {
                message = $"Medication with ID {id} was not found."
            });
        }

        medication.Name = dto.Name;
        medication.Dosage = dto.Dosage;
        medication.Frequency = dto.Frequency;
        medication.StartDate = dto.StartDate!.Value;
        medication.EndDate = dto.EndDate;
        medication.Notes = dto.Notes;

        await _context.SaveChangesAsync();

        var response = new MedicationResponseDto
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

        return Ok(response);
    }

    // Delete a medication
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMedication(int id)
    {
        var medication = await _context.Medications
            .FirstOrDefaultAsync(medication => medication.Id == id);

        if (medication is null)
        {
            return NotFound(new
            {
                message = $"Medication with ID {id} was not found."
            });
        }

        _context.Medications.Remove(medication);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
