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
    // ADMIN, DOCTOR, and PATIENT can view medications
    // =========================================================
    [HttpGet("patient/{patientId:int}")]
    [Authorize(Roles = "ADMIN,DOCTOR,PATIENT")]
    public async Task<ActionResult<IEnumerable<MedicationResponseDto>>>
        GetPatientMedications(
            int patientId,
            [FromQuery] string? search = null)
    {
        var patientExists = await _context.Patients
            .AsNoTracking()
            .AnyAsync(patient => patient.Id == patientId);

        if (!patientExists)
        {
            return NotFound(new
            {
                message =
                    $"Patient with ID {patientId} was not found."
            });
        }

        var query = _context.Medications
            .AsNoTracking()
            .Where(
                medication =>
                    medication.PatientId == patientId);

        // Search by medication name, dosage, or frequency
        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();

            query = query.Where(
                medication =>
                    medication.Name.Contains(normalizedSearch) ||
                    medication.Dosage.Contains(normalizedSearch) ||
                    medication.Frequency.Contains(normalizedSearch));
        }

        // Newest medications first
        var medications = await query
            .OrderByDescending(
                medication => medication.StartDate)
            .Select(medication => new MedicationResponseDto
            {
                Id = medication.Id,
                PatientId = medication.PatientId,
                Name = medication.Name,
                Dosage = medication.Dosage,
                Frequency = medication.Frequency,
                StartDate = medication.StartDate,
                EndDate = medication.EndDate,
                Notes = medication.Notes,

                // Stock and price
                StockQuantity = medication.StockQuantity,
                UnitPrice = medication.UnitPrice
            })
            .ToListAsync();

        return Ok(medications);
    }

    // =========================================================
    // GET: api/Medications/{id}
    // ADMIN, DOCTOR, and PATIENT can view one medication
    // =========================================================
    [HttpGet("{id:int}")]
    [Authorize(Roles = "ADMIN,DOCTOR,PATIENT")]
    public async Task<ActionResult<MedicationResponseDto>>
        GetMedication(int id)
    {
        var medication = await _context.Medications
            .AsNoTracking()
            .Where(
                medication =>
                    medication.Id == id)
            .Select(medication => new MedicationResponseDto
            {
                Id = medication.Id,
                PatientId = medication.PatientId,
                Name = medication.Name,
                Dosage = medication.Dosage,
                Frequency = medication.Frequency,
                StartDate = medication.StartDate,
                EndDate = medication.EndDate,
                Notes = medication.Notes,

                // Stock and price
                StockQuantity = medication.StockQuantity,
                UnitPrice = medication.UnitPrice
            })
            .FirstOrDefaultAsync();

        if (medication is null)
        {
            return NotFound(new
            {
                message =
                    $"Medication with ID {id} was not found."
            });
        }

        return Ok(medication);
    }

    // =========================================================
    // POST: api/Medications
    // Only ADMIN and DOCTOR can create medications
    // =========================================================
    [HttpPost]
    [Authorize(Roles = "ADMIN,DOCTOR")]
    public async Task<ActionResult<MedicationResponseDto>>
        CreateMedication(
            MedicationCreateDto dto)
    {
        // Make sure the patient exists
        var patientExists = await _context.Patients
            .AsNoTracking()
            .AnyAsync(
                patient =>
                    patient.Id == dto.PatientId);

        if (!patientExists)
        {
            return NotFound(new
            {
                message =
                    $"Patient with ID {dto.PatientId} was not found."
            });
        }

        // Validate stock
        if (dto.StockQuantity < 0)
        {
            return BadRequest(new
            {
                message =
                    "Stock quantity cannot be negative."
            });
        }

        // Validate price
        if (dto.UnitPrice < 0)
        {
            return BadRequest(new
            {
                message =
                    "Unit price cannot be negative."
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

            Notes =
                string.IsNullOrWhiteSpace(dto.Notes)
                    ? null
                    : dto.Notes.Trim(),

            // New stock and price fields
            StockQuantity = dto.StockQuantity,
            UnitPrice = dto.UnitPrice
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
    // Only ADMIN and DOCTOR can update medications
    // =========================================================
    [HttpPut("{id:int}")]
    [Authorize(Roles = "ADMIN,DOCTOR")]
    public async Task<ActionResult<MedicationResponseDto>>
        UpdateMedication(
            int id,
            MedicationUpdateDto dto)
    {
        var medication = await _context.Medications
            .FirstOrDefaultAsync(
                medication =>
                    medication.Id == id);

        if (medication is null)
        {
            return NotFound(new
            {
                message =
                    $"Medication with ID {id} was not found."
            });
        }

        // Validate stock
        if (dto.StockQuantity < 0)
        {
            return BadRequest(new
            {
                message =
                    "Stock quantity cannot be negative."
            });
        }

        // Validate price
        if (dto.UnitPrice < 0)
        {
            return BadRequest(new
            {
                message =
                    "Unit price cannot be negative."
            });
        }

        medication.Name =
            dto.Name.Trim();

        medication.Dosage =
            dto.Dosage.Trim();

        medication.Frequency =
            dto.Frequency.Trim();

        medication.StartDate =
            dto.StartDate;

        medication.EndDate =
            dto.EndDate;

        medication.Notes =
            string.IsNullOrWhiteSpace(dto.Notes)
                ? null
                : dto.Notes.Trim();

        // Update stock and price
        medication.StockQuantity =
            dto.StockQuantity;

        medication.UnitPrice =
            dto.UnitPrice;

        await _context.SaveChangesAsync();

        return Ok(ToDto(medication));
    }

    // =========================================================
    // DELETE: api/Medications/{id}
    // Only ADMIN and DOCTOR can delete medications
    // =========================================================
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "ADMIN,DOCTOR")]
    public async Task<IActionResult>
        DeleteMedication(int id)
    {
        var medication = await _context.Medications
            .FirstOrDefaultAsync(
                medication =>
                    medication.Id == id);

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
            Notes = medication.Notes,

            // Stock and price
            StockQuantity = medication.StockQuantity,
            UnitPrice = medication.UnitPrice
        };
    }
}