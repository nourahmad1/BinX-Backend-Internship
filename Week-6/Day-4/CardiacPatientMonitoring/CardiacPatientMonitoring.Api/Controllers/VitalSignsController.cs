using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.DTOs.VitalSign;
using CardiacPatientMonitoring.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardiacPatientMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VitalSignsController : ControllerBase
{
    private readonly AppDbContext _context;

    public VitalSignsController(AppDbContext context)
    {
        _context = context;
    }

    // Get all vital sign records
    // Admin, Doctor, and Patient can view them
    [HttpGet]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<ActionResult<IEnumerable<VitalSignResponseDto>>>
        GetVitalSigns()
    {
        var vitalSigns = await _context.VitalSigns
            .AsNoTracking()
            .OrderByDescending(vitalSign => vitalSign.RecordedAt)
            .Select(vitalSign => new VitalSignResponseDto
            {
                Id = vitalSign.Id,
                PatientId = vitalSign.PatientId,
                HeartRate = vitalSign.HeartRate,
                SystolicPressure = vitalSign.SystolicPressure,
                DiastolicPressure = vitalSign.DiastolicPressure,
                OxygenSaturation = vitalSign.OxygenSaturation,
                RecordedAt = vitalSign.RecordedAt,
                Notes = vitalSign.Notes
            })
            .ToListAsync();

        return Ok(vitalSigns);
    }

    // Get all vital signs for a specific patient
    // Admin, Doctor, and Patient can view them
    [HttpGet("patient/{patientId:int}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<ActionResult<IEnumerable<VitalSignResponseDto>>>
        GetPatientVitalSigns(int patientId)
    {
        // Make sure the patient exists
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

        var vitalSigns = await _context.VitalSigns
            .AsNoTracking()
            .Where(vitalSign => vitalSign.PatientId == patientId)
            .OrderByDescending(vitalSign => vitalSign.RecordedAt)
            .Select(vitalSign => new VitalSignResponseDto
            {
                Id = vitalSign.Id,
                PatientId = vitalSign.PatientId,
                HeartRate = vitalSign.HeartRate,
                SystolicPressure = vitalSign.SystolicPressure,
                DiastolicPressure = vitalSign.DiastolicPressure,
                OxygenSaturation = vitalSign.OxygenSaturation,
                RecordedAt = vitalSign.RecordedAt,
                Notes = vitalSign.Notes
            })
            .ToListAsync();

        return Ok(vitalSigns);
    }

    // Get one vital sign record by its ID
    // Admin, Doctor, and Patient can view it
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<ActionResult<VitalSignResponseDto>>
        GetVitalSign(int id)
    {
        var vitalSign = await _context.VitalSigns
            .AsNoTracking()
            .Where(vitalSign => vitalSign.Id == id)
            .Select(vitalSign => new VitalSignResponseDto
            {
                Id = vitalSign.Id,
                PatientId = vitalSign.PatientId,
                HeartRate = vitalSign.HeartRate,
                SystolicPressure = vitalSign.SystolicPressure,
                DiastolicPressure = vitalSign.DiastolicPressure,
                OxygenSaturation = vitalSign.OxygenSaturation,
                RecordedAt = vitalSign.RecordedAt,
                Notes = vitalSign.Notes
            })
            .FirstOrDefaultAsync();

        if (vitalSign is null)
        {
            return NotFound(new
            {
                message = $"Vital sign with ID {id} was not found."
            });
        }

        return Ok(vitalSign);
    }

    // Create a new vital sign record
    // Only Admin and Doctor can add readings
    [HttpPost]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<VitalSignResponseDto>>
        CreateVitalSign(VitalSignCreateDto dto)
    {
        // Check that the patient exists before creating the reading
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

        var vitalSign = new VitalSign
        {
            PatientId = dto.PatientId,
            HeartRate = dto.HeartRate,
            SystolicPressure = dto.SystolicPressure,
            DiastolicPressure = dto.DiastolicPressure,
            OxygenSaturation = dto.OxygenSaturation,
            RecordedAt = dto.RecordedAt,
            Notes = string.IsNullOrWhiteSpace(dto.Notes)
                ? null
                : dto.Notes.Trim()
        };

        await _context.VitalSigns.AddAsync(vitalSign);
        await _context.SaveChangesAsync();

        var response = ToDto(vitalSign);

        return CreatedAtAction(
            nameof(GetVitalSign),
            new { id = vitalSign.Id },
            response);
    }

    // Update an existing vital sign record
    // Only Admin and Doctor can update readings
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<VitalSignResponseDto>>
        UpdateVitalSign(
            int id,
            VitalSignUpdateDto dto)
    {
        var vitalSign = await _context.VitalSigns
            .FirstOrDefaultAsync(
                vitalSign => vitalSign.Id == id);

        if (vitalSign is null)
        {
            return NotFound(new
            {
                message =
                    $"Vital sign with ID {id} was not found."
            });
        }

        vitalSign.HeartRate = dto.HeartRate;
        vitalSign.SystolicPressure = dto.SystolicPressure;
        vitalSign.DiastolicPressure = dto.DiastolicPressure;
        vitalSign.OxygenSaturation = dto.OxygenSaturation;
        vitalSign.RecordedAt = dto.RecordedAt;

        vitalSign.Notes =
            string.IsNullOrWhiteSpace(dto.Notes)
                ? null
                : dto.Notes.Trim();

        await _context.SaveChangesAsync();

        return Ok(ToDto(vitalSign));
    }

    // Delete a vital sign record
    // Only Admin and Doctor can delete readings
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult>
        DeleteVitalSign(int id)
    {
        var vitalSign = await _context.VitalSigns
            .FirstOrDefaultAsync(
                vitalSign => vitalSign.Id == id);

        if (vitalSign is null)
        {
            return NotFound(new
            {
                message =
                    $"Vital sign with ID {id} was not found."
            });
        }

        _context.VitalSigns.Remove(vitalSign);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Convert the entity to the response DTO
    private static VitalSignResponseDto ToDto(
        VitalSign vitalSign)
    {
        return new VitalSignResponseDto
        {
            Id = vitalSign.Id,
            PatientId = vitalSign.PatientId,
            HeartRate = vitalSign.HeartRate,
            SystolicPressure = vitalSign.SystolicPressure,
            DiastolicPressure = vitalSign.DiastolicPressure,
            OxygenSaturation = vitalSign.OxygenSaturation,
            RecordedAt = vitalSign.RecordedAt,
            Notes = vitalSign.Notes
        };
    }
}