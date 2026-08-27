
using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CardiacPatientMonitoring.Api.Services;

public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _context;

    public AppointmentService(AppDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // Get all appointments
    // =========================================================
    public async Task<IEnumerable<AppointmentResponseDto>>
        GetAppointmentsAsync(
            int? patientId,
            string? status,
            string? doctorName)
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

        return await query
            .OrderByDescending(
                appointment =>
                    appointment.AppointmentDate)
            .Select(appointment =>
                new AppointmentResponseDto
                {
                    Id = appointment.Id,
                    PatientId = appointment.PatientId,
                    AppointmentDate =
                        appointment.AppointmentDate,
                    DoctorName =
                        appointment.DoctorName,
                    Reason =
                        appointment.Reason,
                    Status =
                        appointment.Status,
                    Notes =
                        appointment.Notes
                })
            .ToListAsync();
    }

    // =========================================================
    // Get appointments for a specific patient
    // =========================================================
    public async Task<IEnumerable<AppointmentResponseDto>?>
        GetPatientAppointmentsAsync(int patientId)
    {
        var patientExists = await _context.Patients
            .AsNoTracking()
            .AnyAsync(
                patient =>
                    patient.Id == patientId);

        if (!patientExists)
        {
            return null;
        }

        return await _context.Appointments
            .AsNoTracking()
            .Where(
                appointment =>
                    appointment.PatientId == patientId)
            .OrderByDescending(
                appointment =>
                    appointment.AppointmentDate)
            .Select(appointment =>
                new AppointmentResponseDto
                {
                    Id = appointment.Id,
                    PatientId = appointment.PatientId,
                    AppointmentDate =
                        appointment.AppointmentDate,
                    DoctorName =
                        appointment.DoctorName,
                    Reason =
                        appointment.Reason,
                    Status =
                        appointment.Status,
                    Notes =
                        appointment.Notes
                })
            .ToListAsync();
    }

    // =========================================================
    // Get one appointment
    // =========================================================
    public async Task<AppointmentResponseDto?>
        GetAppointmentAsync(int id)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Where(
                appointment =>
                    appointment.Id == id)
            .Select(appointment =>
                new AppointmentResponseDto
                {
                    Id = appointment.Id,
                    PatientId = appointment.PatientId,
                    AppointmentDate =
                        appointment.AppointmentDate,
                    DoctorName =
                        appointment.DoctorName,
                    Reason =
                        appointment.Reason,
                    Status =
                        appointment.Status,
                    Notes =
                        appointment.Notes
                })
            .FirstOrDefaultAsync();
    }

    // =========================================================
    // Create appointment
    // Business logic + transaction
    // =========================================================
    public async Task<AppointmentResponseDto?>
        CreateAppointmentAsync(
            AppointmentCreateDto dto)
    {
        // =====================================================
        // Business Rule 1:
        // Patient must exist
        // =====================================================

        var patientExists = await _context.Patients
            .AsNoTracking()
            .AnyAsync(
                patient =>
                    patient.Id == dto.PatientId);

        if (!patientExists)
        {
            return null;
        }

        // =====================================================
        // Business Rule 2:
        // Appointment date cannot be in the past
        // =====================================================

        if (dto.AppointmentDate < DateTime.UtcNow)
        {
            throw new ArgumentException(
                "Appointment date cannot be in the past.");
        }

        // =====================================================
        // Business Rule 3:
        // Doctor name is required
        // =====================================================

        if (string.IsNullOrWhiteSpace(dto.DoctorName))
        {
            throw new ArgumentException(
                "Doctor name is required.");
        }

        // =====================================================
        // Business Rule 4:
        // Reason is required
        // =====================================================

        if (string.IsNullOrWhiteSpace(dto.Reason))
        {
            throw new ArgumentException(
                "Appointment reason is required.");
        }

        // =====================================================
        // Business Rule 5:
        // Prevent duplicate appointment for same patient,
        // doctor, and date
        // =====================================================

        var duplicateAppointment =
            await _context.Appointments
                .AsNoTracking()
                .AnyAsync(
                    appointment =>
                        appointment.PatientId ==
                            dto.PatientId
                        &&
                        appointment.DoctorName ==
                            dto.DoctorName.Trim()
                        &&
                        appointment.AppointmentDate ==
                            dto.AppointmentDate);

        if (duplicateAppointment)
        {
            throw new InvalidOperationException(
                "This appointment already exists.");
        }

        // =====================================================
        // Start database transaction
        // =====================================================

        await using var transaction =
            await _context.Database
                .BeginTransactionAsync();

        try
        {
            // =================================================
            // Create appointment
            // =================================================

            var appointment = new Appointment
            {
                PatientId = dto.PatientId,

                AppointmentDate =
                    dto.AppointmentDate,

                DoctorName =
                    dto.DoctorName.Trim(),

                Reason =
                    dto.Reason.Trim(),

                Status =
                    string.IsNullOrWhiteSpace(dto.Status)
                        ? "Scheduled"
                        : dto.Status.Trim(),

                Notes =
                    string.IsNullOrWhiteSpace(dto.Notes)
                        ? null
                        : dto.Notes.Trim()
            };

            // =================================================
            // Add appointment
            // =================================================

            await _context.Appointments
                .AddAsync(appointment);

            // =================================================
            // Save changes
            // =================================================

            await _context.SaveChangesAsync();

            // =================================================
            // Commit transaction
            // =================================================

            await transaction.CommitAsync();

            return ToDto(appointment);
        }
        catch
        {
            // =================================================
            // Rollback if anything fails
            // =================================================

            await transaction.RollbackAsync();

            throw;
        }
    }

    // =========================================================
    // Update appointment
    // =========================================================
    public async Task<AppointmentResponseDto?>
        UpdateAppointmentAsync(
            int id,
            AppointmentUpdateDto dto)
    {
        var appointment =
            await _context.Appointments
                .FirstOrDefaultAsync(
                    appointment =>
                        appointment.Id == id);

        if (appointment is null)
        {
            return null;
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

        return ToDto(appointment);
    }

    // =========================================================
    // Delete appointment
    // =========================================================
    public async Task<bool>
        DeleteAppointmentAsync(int id)
    {
        var appointment =
            await _context.Appointments
                .FirstOrDefaultAsync(
                    appointment =>
                        appointment.Id == id);

        if (appointment is null)
        {
            return false;
        }

        _context.Appointments.Remove(appointment);

        await _context.SaveChangesAsync();

        return true;
    }

    // =========================================================
    // Convert entity to DTO
    // =========================================================
    private static AppointmentResponseDto ToDto(
        Appointment appointment)
    {
        return new AppointmentResponseDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            AppointmentDate =
                appointment.AppointmentDate,
            DoctorName =
                appointment.DoctorName,
            Reason =
                appointment.Reason,
            Status =
                appointment.Status,
            Notes =
                appointment.Notes
        };
    }
}
