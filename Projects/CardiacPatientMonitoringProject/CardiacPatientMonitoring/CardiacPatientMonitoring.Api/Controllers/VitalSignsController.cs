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

    // =========================================================
    // GET: api/VitalSigns
    // Admin, Doctor, Nurse
    // =========================================================

    [HttpGet]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<ActionResult<IEnumerable<VitalSignResponseDto>>> GetVitalSigns()
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

    // =========================================================
    // GET: api/VitalSigns/patient/{patientId}
    // Admin, Doctor, Nurse
    // =========================================================

    [HttpGet("patient/{patientId:int}")]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<ActionResult<IEnumerable<VitalSignResponseDto>>> GetPatientVitalSigns(
        int patientId)
    {
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

    // =========================================================
    // GET: api/VitalSigns/{id}
    // Admin, Doctor, Nurse
    // =========================================================

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<ActionResult<VitalSignResponseDto>> GetVitalSign(int id)
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

    // =========================================================
    // POST: api/VitalSigns
    // Admin, Doctor
    // =========================================================

    [HttpPost]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<VitalSignResponseDto>> CreateVitalSign(
        VitalSignCreateDto dto)
    {
        var patientExists = await _context.Patients
            .AnyAsync(patient => patient.Id == dto.PatientId);

        if (!patientExists)
        {
            return BadRequest(new
            {
                message = $"Patient with ID {dto.PatientId} does not exist."
            });
        }

        var vitalSign = new VitalSign
        {
            PatientId = dto.PatientId,
            HeartRate = dto.HeartRate,
            SystolicPressure = dto.SystolicPressure,
            DiastolicPressure = dto.DiastolicPressure,
            OxygenSaturation = dto.OxygenSaturation,
            RecordedAt = dto.RecordedAt!.Value,
            Notes = dto.Notes
        };

        await _context.VitalSigns.AddAsync(vitalSign);
        await _context.SaveChangesAsync();

        var response = new VitalSignResponseDto
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

        return CreatedAtAction(
            nameof(GetVitalSign),
            new { id = vitalSign.Id },
            response);
    }

    // =========================================================
    // PUT: api/VitalSigns/{id}
    // Admin, Doctor
    // =========================================================

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<VitalSignResponseDto>> UpdateVitalSign(
        int id,
        VitalSignUpdateDto dto)
    {
        var vitalSign = await _context.VitalSigns
            .FirstOrDefaultAsync(vitalSign => vitalSign.Id == id);

        if (vitalSign is null)
        {
            return NotFound(new
            {
                message = $"Vital sign with ID {id} was not found."
            });
        }

        vitalSign.HeartRate = dto.HeartRate;
        vitalSign.SystolicPressure = dto.SystolicPressure;
        vitalSign.DiastolicPressure = dto.DiastolicPressure;
        vitalSign.OxygenSaturation = dto.OxygenSaturation;
        vitalSign.RecordedAt = dto.RecordedAt;
        vitalSign.Notes = dto.Notes;

        await _context.SaveChangesAsync();

        var response = new VitalSignResponseDto
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

        return Ok(response);
    }

    // =========================================================
    // DELETE: api/VitalSigns/{id}
    // Admin, Doctor
    // =========================================================

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> DeleteVitalSign(int id)
    {
        var vitalSign = await _context.VitalSigns
            .FirstOrDefaultAsync(vitalSign => vitalSign.Id == id);

        if (vitalSign is null)
        {
            return NotFound(new
            {
                message = $"Vital sign with ID {id} was not found."
            });
        }

        _context.VitalSigns.Remove(vitalSign);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}