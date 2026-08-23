
using System.ComponentModel.DataAnnotations;

namespace CardiacPatientMonitoring.Api.Validation;

public class FutureDateAttribute : ValidationAttribute
{
    public FutureDateAttribute()
    {
        ErrorMessage = "Date must be in the future.";
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        if (value is DateTime date)
        {
            return date > DateTime.UtcNow;
        }

        return false;
    }
}
