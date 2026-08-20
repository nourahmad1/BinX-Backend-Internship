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
public class AppointmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AppointmentsController(AppDbContext context)
    {
        _context = context;
    }

    // Get appointments with optional filters
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetAppointments(
        [FromQuery] int? patientId,
        [FromQuery] string? status,
        [FromQuery] string? doctorName)
    {
        var query = _context.Appointments
            .AsNoTracking()
            .AsQueryable();

        // Filter by patient
        if (patientId.HasValue)
        {
            query = query.Where(a => a.PatientId == patientId.Value);
        }

        // Filter by appointment status
        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim();

            query = query.Where(a => a.Status == normalizedStatus);
        }

        // Filter by doctor name
        if (!string.IsNullOrWhiteSpace(doctorName))
        {
            var normalizedDoctorName = doctorName.Trim();

            query = query.Where(a =>
                a.DoctorName.Contains(normalizedDoctorName));
        }

        // Show the latest appointments first
        var appointments = await query
            .OrderByDescending(a => a.AppointmentDate)
            .Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                AppointmentDate = a.AppointmentDate,
                DoctorName = a.DoctorName,
                Reason = a.Reason,
                Status = a.Status,
                Notes = a.Notes
            })
            .ToListAsync();

        return Ok(appointments);
    }

    // Get appointments for one patient
    [HttpGet("patient/{patientId:int}")]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetPatientAppointments(
        int patientId)
    {
        // Make sure the patient exists before getting the appointments
        var patientExists = await _context.Patients
            .AsNoTracking()
            .AnyAsync(p => p.Id == patientId);

        if (!patientExists)
        {
            return NotFound(new
            {
                message = $"Patient with ID {patientId} was not found."
            });
        }

        var appointments = await _context.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                AppointmentDate = a.AppointmentDate,
                DoctorName = a.DoctorName,
                Reason = a.Reason,
                Status = a.Status,
                Notes = a.Notes
            })
            .ToListAsync();

        return Ok(appointments);
    }

    // Get a single appointment
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppointmentResponseDto>> GetAppointment(int id)
    {
        var appointment = await _context.Appointments
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                AppointmentDate = a.AppointmentDate,
                DoctorName = a.DoctorName,
                Reason = a.Reason,
                Status = a.Status,
                Notes = a.Notes
            })
            .FirstOrDefaultAsync();

        if (appointment is null)
        {
            return NotFound(new
            {
                message = $"Appointment with ID {id} was not found."
            });
        }

        return Ok(appointment);
    }

    // Create a new appointment
    [HttpPost]
    public async Task<ActionResult<AppointmentResponseDto>> CreateAppointment(
        [FromBody] AppointmentCreateDto dto)
    {
        // Check that the patient exists
        var patientExists = await _context.Patients
            .AsNoTracking()
            .AnyAsync(p => p.Id == dto.PatientId);

        if (!patientExists)
        {
            return NotFound(new
            {
                message = $"Patient with ID {dto.PatientId} was not found."
            });
        }

        var appointment = new Appointment
        {
            PatientId = dto.PatientId,
            AppointmentDate = dto.AppointmentDate,
            DoctorName = dto.DoctorName.Trim(),
            Reason = dto.Reason.Trim(),
            Status = dto.Status.Trim(),

            // Store null if no notes were provided
            Notes = string.IsNullOrWhiteSpace(dto.Notes)
                ? null
                : dto.Notes.Trim()
        };

        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();

        var response = ToDto(appointment);

        // Return the created appointment
        return CreatedAtAction(
            nameof(GetAppointment),
            new { id = appointment.Id },
            response);
    }

    // Update an existing appointment
    [HttpPut("{id:int}")]
    public async Task<ActionResult<AppointmentResponseDto>> UpdateAppointment(
        int id,
        [FromBody] AppointmentUpdateDto dto)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment is null)
        {
            return NotFound(new
            {
                message = $"Appointment with ID {id} was not found."
            });
        }

        appointment.AppointmentDate = dto.AppointmentDate;
        appointment.DoctorName = dto.DoctorName.Trim();
        appointment.Reason = dto.Reason.Trim();
        appointment.Status = dto.Status.Trim();
        appointment.Notes = string.IsNullOrWhiteSpace(dto.Notes)
            ? null
            : dto.Notes.Trim();

        await _context.SaveChangesAsync();

        return Ok(ToDto(appointment));
    }

    // Delete an appointment
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment is null)
        {
            return NotFound(new
            {
                message = $"Appointment with ID {id} was not found."
            });
        }

        _context.Appointments.Remove(appointment);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Map the entity to the response DTO
    private static AppointmentResponseDto ToDto(Appointment appointment)
    {
        return new AppointmentResponseDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            AppointmentDate = appointment.AppointmentDate,
            DoctorName = appointment.DoctorName,
            Reason = appointment.Reason,
            Status = appointment.Status,
            Notes = appointment.Notes
        };
    }
}