using CardiacPatientMonitoring.Api.DTOs;
using FluentValidation;

namespace CardiacPatientMonitoring.Api.Validators;

public class AppointmentCreateDtoValidator : AbstractValidator<AppointmentCreateDto>
{
    public AppointmentCreateDtoValidator()
    {
        // Patient ID must be a valid positive number
        RuleFor(appointment => appointment.PatientId)
            .GreaterThan(0)
            .WithMessage("Patient ID must be greater than 0.");

        // The appointment should not be scheduled in the past
        RuleFor(appointment => appointment.AppointmentDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Appointment date cannot be in the past.");

        // Doctor name is required and has a maximum length
        RuleFor(appointment => appointment.DoctorName)
            .NotEmpty()
            .WithMessage("Doctor name is required.")
            .MaximumLength(200)
            .WithMessage("Doctor name cannot exceed 200 characters.");

        // A reason should be provided for the appointment
        RuleFor(appointment => appointment.Reason)
            .NotEmpty()
            .WithMessage("Appointment reason is required.")
            .MaximumLength(500)
            .WithMessage("Appointment reason cannot exceed 500 characters.");

        // Status is required
        RuleFor(appointment => appointment.Status)
            .NotEmpty()
            .WithMessage("Appointment status is required.")
            .MaximumLength(50)
            .WithMessage("Appointment status cannot exceed 50 characters.");

        // Notes are optional, but should not be too long
        RuleFor(appointment => appointment.Notes)
            .MaximumLength(500)
            .When(appointment => appointment.Notes != null)
            .WithMessage("Notes cannot exceed 500 characters.");
    }
}