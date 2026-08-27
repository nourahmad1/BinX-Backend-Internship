using CardiacPatientMonitoring.Api.DTOs.VitalSign;
using FluentValidation;

namespace CardiacPatientMonitoring.Api.Validators;

public class VitalSignUpdateDtoValidator
    : AbstractValidator<VitalSignUpdateDto>
{
    public VitalSignUpdateDtoValidator()
    {
        // Keep the heart rate within a reasonable range
        RuleFor(x => x.HeartRate)
            .InclusiveBetween(30, 220)
            .WithMessage("Heart rate must be between 30 and 220.");

        // Check the upper blood pressure value
        RuleFor(x => x.SystolicPressure)
            .InclusiveBetween(60, 250)
            .WithMessage("Systolic pressure must be between 60 and 250.");

        // Check the lower blood pressure value
        RuleFor(x => x.DiastolicPressure)
            .InclusiveBetween(30, 150)
            .WithMessage("Diastolic pressure must be between 30 and 150.");

        // Oxygen saturation should stay within a valid range
        RuleFor(x => x.OxygenSaturation)
            .InclusiveBetween(50, 100)
            .WithMessage("Oxygen saturation must be between 50 and 100.");

        // A recorded date must be provided
        RuleFor(x => x.RecordedAt)
            .NotEmpty()
            .WithMessage("Recorded date is required.");

        // Notes are optional, but should not be too long
        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => x.Notes != null)
            .WithMessage("Notes cannot exceed 500 characters.");
    }
}