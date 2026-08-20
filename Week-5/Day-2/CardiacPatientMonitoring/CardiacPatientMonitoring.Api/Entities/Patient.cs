namespace CardiacPatientMonitoring.Api.Entities;

public class Patient
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

    // Patient's contact number
    public string PhoneNumber { get; set; } = string.Empty;

    // Date when the patient was added
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Vital sign records for this patient
    public ICollection<VitalSign> VitalSigns { get; set; }
        = new List<VitalSign>();

    // Medications assigned to this patient
    public ICollection<Medication> Medications { get; set; }
        = new List<Medication>();

    // Appointments for this patient
    public ICollection<Appointment> Appointments { get; set; }
        = new List<Appointment>();
}