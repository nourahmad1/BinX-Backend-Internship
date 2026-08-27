using Microsoft.AspNetCore.Identity;

namespace CardiacPatientMonitoring.Api.Entities;

public class ApplicationUser : IdentityUser
{
    // Full name of the user 
    public string FullName { get; set; } = string.Empty;

    // Navigation property to the patient profile
    public Patient? Patient { get; set; }
}