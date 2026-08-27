
using CardiacPatientMonitoring.Api.DTOs;

namespace CardiacPatientMonitoring.Api.Services;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsAsync(
        int? patientId,
        string? status,
        string? doctorName);

    Task<IEnumerable<AppointmentResponseDto>?> GetPatientAppointmentsAsync(
        int patientId);

    Task<AppointmentResponseDto?> GetAppointmentAsync(
        int id);

    Task<AppointmentResponseDto?> CreateAppointmentAsync(
        AppointmentCreateDto dto);

    Task<AppointmentResponseDto?> UpdateAppointmentAsync(
        int id,
        AppointmentUpdateDto dto);

    Task<bool> DeleteAppointmentAsync(
        int id);
}
