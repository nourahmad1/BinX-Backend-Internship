namespace CardiacPatientMonitoring.Api.DTOs;

public class PatientResponseDto
{
    // Patient ID
    public int Id { get; set; }

    // Patient's first name
    public string FirstName { get; set; } = string.Empty;

    // Patient's last name
    public string LastName { get; set; } = string.Empty;

    // Patient's date of birth
    public DateTime DateOfBirth { get; set; }

    // Patient's gender
    public string Gender { get; set; } = string.Empty;

    // Patient's phone number
    public string PhoneNumber { get; set; } = string.Empty;

    // Date when the patient was added
    public DateTime CreatedAt { get; set; }
}