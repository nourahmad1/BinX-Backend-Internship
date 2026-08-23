using CardiacPatientMonitoring.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CardiacPatientMonitoring.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(
        AppDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        const string email = "admin@cardiacapi.com";
        const string password = "Admin@12345";

        // Create the default admin account if it doesn't already exist
        var existingUser =
            await userManager.FindByEmailAsync(email);

        if (existingUser is null)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = "System Administrator",
                EmailConfirmed = true
            };

            var result =
                await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    $"Could not create seed user: {errors}");
            }
        }

        // Don't add the sample data again if patients already exist
        if (await context.Patients.AnyAsync())
        {
            return;
        }

        // Add some sample patients for testing the API
        var patients = new List<Patient>
        {
            new()
            {
                FirstName = "Ahmad",
                LastName = "Hassan",
                DateOfBirth = new DateTime(1985, 4, 12),
                Gender = "Male",
                PhoneNumber = "0599000001"
            },

            new()
            {
                FirstName = "Sara",
                LastName = "Khalil",
                DateOfBirth = new DateTime(1992, 8, 25),
                Gender = "Female",
                PhoneNumber = "0599000002"
            },

            new()
            {
                FirstName = "Omar",
                LastName = "Nasser",
                DateOfBirth = new DateTime(1978, 11, 3),
                Gender = "Male",
                PhoneNumber = "0599000003"
            }
        };

        await context.Patients.AddRangeAsync(patients);
        await context.SaveChangesAsync();

        // Add sample vital signs for the patients
        var vitalSigns = new List<VitalSign>
        {
            new()
            {
                PatientId = patients[0].Id,
                HeartRate = 78,
                SystolicPressure = 120,
                DiastolicPressure = 80,
                OxygenSaturation = 98.0m,
                RecordedAt = DateTime.UtcNow.AddHours(-2),
                Notes = "Normal reading"
            },

            new()
            {
                PatientId = patients[1].Id,
                HeartRate = 82,
                SystolicPressure = 125,
                DiastolicPressure = 82,
                OxygenSaturation = 97.5m,
                RecordedAt = DateTime.UtcNow.AddHours(-1),
                Notes = "Routine monitoring"
            },

            new()
            {
                PatientId = patients[2].Id,
                HeartRate = 88,
                SystolicPressure = 135,
                DiastolicPressure = 85,
                OxygenSaturation = 96.0m,
                RecordedAt = DateTime.UtcNow.AddMinutes(-30),
                Notes = "Follow-up reading"
            }
        };

        // Add sample medications
        var medications = new List<Medication>
        {
            new()
            {
                PatientId = patients[0].Id,
                Name = "Aspirin",
                Dosage = "81 mg",
                Frequency = "Once daily",
                StartDate = new DateTime(2026, 1, 10),
                Notes = "Take after food"
            },

            new()
            {
                PatientId = patients[1].Id,
                Name = "Atorvastatin",
                Dosage = "20 mg",
                Frequency = "Once daily",
                StartDate = new DateTime(2026, 2, 15),
                Notes = "Evening dose"
            },

            new()
            {
                PatientId = patients[2].Id,
                Name = "Metoprolol",
                Dosage = "25 mg",
                Frequency = "Twice daily",
                StartDate = new DateTime(2026, 3, 1),
                Notes = "Regular monitoring recommended"
            }
        };

        // Add sample appointments
        var appointments = new List<Appointment>
        {
            new()
            {
                PatientId = patients[0].Id,
                AppointmentDate = DateTime.UtcNow.AddDays(3),
                DoctorName = "Dr. Ahmad Saleh",
                Reason = "Cardiac follow-up",
                Status = "Scheduled",
                Notes = "Bring previous vital sign records"
            },

            new()
            {
                PatientId = patients[1].Id,
                AppointmentDate = DateTime.UtcNow.AddDays(5),
                DoctorName = "Dr. Lina Omar",
                Reason = "Routine cardiac check",
                Status = "Scheduled"
            },

            new()
            {
                PatientId = patients[2].Id,
                AppointmentDate = DateTime.UtcNow.AddDays(7),
                DoctorName = "Dr. Ahmad Saleh",
                Reason = "Blood pressure review",
                Status = "Scheduled"
            }
        };

        // Save all the sample data
        await context.VitalSigns.AddRangeAsync(vitalSigns);
        await context.Medications.AddRangeAsync(medications);
        await context.Appointments.AddRangeAsync(appointments);

        await context.SaveChangesAsync();
    }
}