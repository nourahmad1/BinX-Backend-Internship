using CardiacPatientMonitoring.Api.DTOs;
using FluentValidation;

namespace CardiacPatientMonitoring.Api.Validators;

public class PatientCreateDtoValidator : AbstractValidator<PatientCreateDto>
{
    public PatientCreateDtoValidator()
    {
        // First name is required and should not be too long
        RuleFor(patient => patient.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        // Last name is required and should not be too long
        RuleFor(patient => patient.LastName)
            .NotEmpty()
            .MaximumLength(50);

        // Date of birth must be a valid date in the past
        RuleFor(patient => patient.DateOfBirth)
            .NotEmpty()
            .LessThan(DateTime.UtcNow)
            .WithMessage("Date of birth must be in the past.");

        // Only Male or Female are accepted for now
        RuleFor(patient => patient.Gender)
            .NotEmpty()
            .Must(gender =>
                gender.Equals("Male", StringComparison.OrdinalIgnoreCase) ||
                gender.Equals("Female", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Gender must be Male or Female.");

        // Allow common characters used in phone numbers
        RuleFor(patient => patient.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20)
            .Matches(@"^[0-9+\-\s()]+$")
            .WithMessage("Phone number contains invalid characters.");
    }
}