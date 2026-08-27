using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardiacPatientMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(
        IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
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
        var appointments =
            await _appointmentService.GetAppointmentsAsync(
                patientId,
                status,
                doctorName);

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
        var result =
            await _appointmentService.GetPatientAppointmentsAsync(
                patientId);

        if (result is null)
        {
            return NotFound(new
            {
                message =
                    $"Patient with ID {patientId} was not found."
            });
        }

        return Ok(result);
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
        var appointment =
            await _appointmentService.GetAppointmentAsync(id);

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
        var result =
            await _appointmentService.CreateAppointmentAsync(dto);

        if (result is null)
        {
            return NotFound(new
            {
                message =
                    $"Patient with ID {dto.PatientId} was not found."
            });
        }

        return CreatedAtAction(
            nameof(GetAppointment),
            new { id = result.Id },
            result);
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
        var result =
            await _appointmentService.UpdateAppointmentAsync(
                id,
                dto);

        if (result is null)
        {
            return NotFound(new
            {
                message =
                    $"Appointment with ID {id} was not found."
            });
        }

        return Ok(result);
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
        var deleted =
            await _appointmentService.DeleteAppointmentAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message =
                    $"Appointment with ID {id} was not found."
            });
        }

        return NoContent();
    }
}