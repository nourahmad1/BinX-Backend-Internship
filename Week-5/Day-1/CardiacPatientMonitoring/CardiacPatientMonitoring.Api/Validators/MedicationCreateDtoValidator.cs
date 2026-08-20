using CardiacPatientMonitoring.Api.DTOs;
using FluentValidation;

namespace CardiacPatientMonitoring.Api.Validators;

public class MedicationCreateDtoValidator : AbstractValidator<MedicationCreateDto>
{
    public MedicationCreateDtoValidator()
    {
        // Make sure the medication belongs to a valid patient
        RuleFor(medication => medication.PatientId)
            .GreaterThan(0)
            .WithMessage("Patient ID must be greater than 0.");

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

        // Frequency tells us how often the medication is taken
        RuleFor(medication => medication.Frequency)
            .NotEmpty()
            .WithMessage("Frequency is required.")
            .MaximumLength(100)
            .WithMessage("Frequency cannot exceed 100 characters.");

        // A medication should not start in the future
        RuleFor(medication => medication.StartDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Start date cannot be in the future.");

        // If an end date is provided, it should come after the start date
        RuleFor(medication => medication.EndDate)
            .GreaterThanOrEqualTo(medication => medication.StartDate)
            .When(medication => medication.EndDate.HasValue)
            .WithMessage("End date cannot be earlier than the start date.");

        // Notes are optional, but should not be too long
        RuleFor(medication => medication.Notes)
            .MaximumLength(500)
            .When(medication => medication.Notes != null);
    }
}