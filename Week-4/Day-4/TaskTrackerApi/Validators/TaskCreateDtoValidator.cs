using FluentValidation;
using TaskTrackerApi.DTOs;

namespace TaskTrackerApi.Validators;

public class TaskCreateDtoValidator : AbstractValidator<TaskCreateDto>
{
    public TaskCreateDtoValidator()
    {
        // Validate task title
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Title is required and cannot exceed 200 characters.");

        RuleFor(x => x.Title)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("Title cannot contain only spaces.");

        // Validate user ID
        RuleFor(x => x.UserId)
            .NotEmpty()
            .Must(BeValidGuid)
            .WithMessage("UserId must be a valid user ID.");
    }

    private static bool BeValidGuid(string userId)
    {
        return Guid.TryParse(userId, out _);
    }
}