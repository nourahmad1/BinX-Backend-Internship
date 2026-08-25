using CardiacPatientMonitoring.Api.DTOs;
using FluentValidation;

namespace CardiacPatientMonitoring.Api.Validators;

public class VitalSignCreateDtoValidator : AbstractValidator<VitalSignCreateDto>
{
    public VitalSignCreateDtoValidator()
    {
        // Make sure the vital sign belongs to a valid patient
        RuleFor(vitalSign => vitalSign.PatientId)
            .GreaterThan(0)
            .WithMessage("Patient ID must be greater than 0.");

        // Check that the heart rate is within a reasonable range
        RuleFor(vitalSign => vitalSign.HeartRate)
            .InclusiveBetween(30, 220)
            .WithMessage("Heart rate must be between 30 and 220 BPM.");

        // Check the upper blood pressure value
        RuleFor(vitalSign => vitalSign.SystolicPressure)
            .InclusiveBetween(70, 250)
            .WithMessage("Systolic pressure must be between 70 and 250 mmHg.");

        // Check the lower blood pressure value
        RuleFor(vitalSign => vitalSign.DiastolicPressure)
            .InclusiveBetween(40, 150)
            .WithMessage("Diastolic pressure must be between 40 and 150 mmHg.");

        // Oxygen saturation should be between 50% and 100%
        RuleFor(vitalSign => vitalSign.OxygenSaturation)
            .InclusiveBetween(50, 100)
            .WithMessage("Oxygen saturation must be between 50% and 100%.");

        // A reading cannot have a future timestamp
        RuleFor(vitalSign => vitalSign.RecordedAt)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Recorded time cannot be in the future.");

        // Notes are optional, but should not be too long
        RuleFor(vitalSign => vitalSign.Notes)
            .MaximumLength(500)
            .When(vitalSign => vitalSign.Notes != null);
    }
}