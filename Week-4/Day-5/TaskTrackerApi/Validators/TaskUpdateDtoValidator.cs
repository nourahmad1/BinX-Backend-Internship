using FluentValidation;
using TaskTrackerApi.DTOs;

namespace TaskTrackerApi.Validators;

public class TaskUpdateDtoValidator : AbstractValidator<TaskUpdateDto>
{
    public TaskUpdateDtoValidator()
    {
        // Validate task title
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters.")
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("Title cannot contain only spaces.");
    }
}