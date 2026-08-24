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

    // =========================================================
    // GET: api/Appointments
    // Admin, Doctor, and Patient can view appointments
    // =========================================================
    [HttpGet]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>>
        GetAppointments(
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
            query = query.Where(
                appointment =>
                    appointment.PatientId == patientId.Value);
        }

        // Filter by status
        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim();

            query = query.Where(
                appointment =>
                    appointment.Status == normalizedStatus);
        }

        // Search by doctor name
        if (!string.IsNullOrWhiteSpace(doctorName))
        {
            var normalizedDoctorName = doctorName.Trim();

            query = query.Where(
                appointment =>
                    appointment.DoctorName.Contains(
                        normalizedDoctorName));
        }

        // Newest appointments first
        var appointments = await query
            .OrderByDescending(
                appointment => appointment.AppointmentDate)
            .Select(appointment => new AppointmentResponseDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                AppointmentDate = appointment.AppointmentDate,
                DoctorName = appointment.DoctorName,
                Reason = appointment.Reason,
                Status = appointment.Status,
                Notes = appointment.Notes
            })
            .ToListAsync();

        return Ok(appointments);
    }

    // =========================================================
    // GET: api/Appointments/patient/{patientId}
    // Admin, Doctor, and Patient can view patient appointments
    // =========================================================
    [HttpGet("patient/{patientId:int}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>>
        GetPatientAppointments(int patientId)
    {
        // Check if patient exists
        var patientExists = await _context.Patients
            .AsNoTracking()
            .AnyAsync(
                patient => patient.Id == patientId);

        if (!patientExists)
        {
            return NotFound(new
            {
                message =
                    $"Patient with ID {patientId} was not found."
            });
        }

        var appointments = await _context.Appointments
            .AsNoTracking()
            .Where(
                appointment =>
                    appointment.PatientId == patientId)
            .OrderByDescending(
                appointment => appointment.AppointmentDate)
            .Select(appointment => new AppointmentResponseDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                AppointmentDate = appointment.AppointmentDate,
                DoctorName = appointment.DoctorName,
                Reason = appointment.Reason,
                Status = appointment.Status,
                Notes = appointment.Notes
            })
            .ToListAsync();

        return Ok(appointments);
    }

    // =========================================================
    // GET: api/Appointments/{id}
    // Admin, Doctor, and Patient can view one appointment
    // =========================================================
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Doctor,Patient")]
    public async Task<ActionResult<AppointmentResponseDto>>
        GetAppointment(int id)
    {
        var appointment = await _context.Appointments
            .AsNoTracking()
            .Where(
                appointment =>
                    appointment.Id == id)
            .Select(appointment => new AppointmentResponseDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                AppointmentDate = appointment.AppointmentDate,
                DoctorName = appointment.DoctorName,
                Reason = appointment.Reason,
                Status = appointment.Status,
                Notes = appointment.Notes
            })
            .FirstOrDefaultAsync();

        if (appointment is null)
        {
            return NotFound(new
            {
                message =
                    $"Appointment with ID {id} was not found."
            });
        }

        return Ok(appointment);
    }

    // =========================================================
    // POST: api/Appointments
    // Only Admin and Doctor can create appointments
    // =========================================================
    [HttpPost]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<AppointmentResponseDto>>
        CreateAppointment(
            [FromBody] AppointmentCreateDto dto)
    {
        // Make sure the patient exists
        var patientExists = await _context.Patients
            .AsNoTracking()
            .AnyAsync(
                patient => patient.Id == dto.PatientId);

        if (!patientExists)
        {
            return NotFound(new
            {
                message =
                    $"Patient with ID {dto.PatientId} was not found."
            });
        }

        var appointment = new Appointment
        {
            PatientId = dto.PatientId,
            AppointmentDate = dto.AppointmentDate,
            DoctorName = dto.DoctorName.Trim(),
            Reason = dto.Reason.Trim(),
            Status = dto.Status.Trim(),

            Notes = string.IsNullOrWhiteSpace(dto.Notes)
                ? null
                : dto.Notes.Trim()
        };

        await _context.Appointments.AddAsync(appointment);

        await _context.SaveChangesAsync();

        var response = ToDto(appointment);

        return CreatedAtAction(
            nameof(GetAppointment),
            new { id = appointment.Id },
            response);
    }

    // =========================================================
    // PUT: api/Appointments/{id}
    // Only Admin and Doctor can update appointments
    // =========================================================
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<AppointmentResponseDto>>
        UpdateAppointment(
            int id,
            [FromBody] AppointmentUpdateDto dto)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(
                appointment =>
                    appointment.Id == id);

        if (appointment is null)
        {
            return NotFound(new
            {
                message =
                    $"Appointment with ID {id} was not found."
            });
        }

        appointment.AppointmentDate =
            dto.AppointmentDate;

        appointment.DoctorName =
            dto.DoctorName.Trim();

        appointment.Reason =
            dto.Reason.Trim();

        appointment.Status =
            dto.Status.Trim();

        appointment.Notes =
            string.IsNullOrWhiteSpace(dto.Notes)
                ? null
                : dto.Notes.Trim();

        await _context.SaveChangesAsync();

        return Ok(ToDto(appointment));
    }

    // =========================================================
    // DELETE: api/Appointments/{id}
    // Only Admin and Doctor can delete appointments
    // =========================================================
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult>
        DeleteAppointment(int id)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(
                appointment =>
                    appointment.Id == id);

        if (appointment is null)
        {
            return NotFound(new
            {
                message =
                    $"Appointment with ID {id} was not found."
            });
        }

        _context.Appointments.Remove(appointment);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // =========================================================
    // Helper method
    // =========================================================
    private static AppointmentResponseDto ToDto(
        Appointment appointment)
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