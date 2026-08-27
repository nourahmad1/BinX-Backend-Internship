using CardiacPatientMonitoring.Api.DTOs;
using FluentValidation;

namespace CardiacPatientMonitoring.Api.Validators;

public class MedicationUpdateDtoValidator : AbstractValidator<MedicationUpdateDto>
{
    public MedicationUpdateDtoValidator()
    {
        // Medication name is required
        RuleFor(medication => medication.Name)
            .NotEmpty()
            .WithMessage("Medication name is required.")
            .MaximumLength(200)
            .WithMessage("Medication name cannot exceed 200 characters.");

        // Dosage must be provided
        RuleFor(medication => medication.Dosage)
            .NotEmpty()
            .WithMessage("Dosage is required.")
            .MaximumLength(100)
            .WithMessage("Dosage cannot exceed 100 characters.");

        // Make sure the medication frequency is provided
        RuleFor(medication => medication.Frequency)
            .NotEmpty()
            .WithMessage("Frequency is required.")
            .MaximumLength(100)
            .WithMessage("Frequency cannot exceed 100 characters.");

        // The medication should not start in the future
        RuleFor(medication => medication.StartDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Start date cannot be in the future.");

        // End date should not be before the start date
        RuleFor(medication => medication.EndDate)
            .GreaterThanOrEqualTo(medication => medication.StartDate)
            .When(medication => medication.EndDate.HasValue)
            .WithMessage("End date cannot be earlier than the start date.");

        // Notes are optional, but have a maximum length
        RuleFor(medication => medication.Notes)
            .MaximumLength(500)
            .When(medication => medication.Notes != null);
    }
}