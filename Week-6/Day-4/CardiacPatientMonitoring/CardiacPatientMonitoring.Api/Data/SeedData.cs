using CardiacPatientMonitoring.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CardiacPatientMonitoring.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // =========================================================
        // 1. Create application roles
        // =========================================================

        string[] roles =
        {
            "ADMIN",
            "DOCTOR",
            "PATIENT"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult =
                    await roleManager.CreateAsync(
                        new IdentityRole(role));

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        roleResult.Errors.Select(
                            error => error.Description));

                    throw new InvalidOperationException(
                        $"Could not create role '{role}': {errors}");
                }
            }
        }

        // =========================================================
        // 2. Create default Admin account
        // =========================================================

        const string adminEmail =
            "admin@cardiacapi.com";

        const string adminPassword =
            "Admin@12345";

        var adminUser =
            await userManager.FindByEmailAsync(adminEmail);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true
            };

            var result =
                await userManager.CreateAsync(
                    adminUser,
                    adminPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    $"Could not create admin user: {errors}");
            }
        }

        // Make sure Admin has ADMIN role
        if (!await userManager.IsInRoleAsync(
                adminUser,
                "ADMIN"))
        {
            var roleResult =
                await userManager.AddToRoleAsync(
                    adminUser,
                    "ADMIN");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    $"Could not assign ADMIN role: {errors}");
            }
        }

        // =========================================================
        // 3. Create default Doctor account
        // =========================================================

        const string doctorEmail =
            "doctor@cardiacapi.com";

        const string doctorPassword =
            "Doctor@12345";

        var doctorUser =
            await userManager.FindByEmailAsync(
                doctorEmail);

        if (doctorUser is null)
        {
            doctorUser = new ApplicationUser
            {
                UserName = doctorEmail,
                Email = doctorEmail,
                FullName = "Dr. Ahmad Saleh",
                EmailConfirmed = true
            };

            var result =
                await userManager.CreateAsync(
                    doctorUser,
                    doctorPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    $"Could not create doctor user: {errors}");
            }
        }

        // Make sure Doctor has DOCTOR role
        if (!await userManager.IsInRoleAsync(
                doctorUser,
                "DOCTOR"))
        {
            var roleResult =
                await userManager.AddToRoleAsync(
                    doctorUser,
                    "DOCTOR");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    $"Could not assign DOCTOR role: {errors}");
            }
        }

        // =========================================================
        // 4. Create default Patient account
        // =========================================================

        const string patientEmail =
            "patient@cardiacapi.com";

        const string patientPassword =
            "Patient@12345";

        var patientUser =
            await userManager.FindByEmailAsync(
                patientEmail);

        if (patientUser is null)
        {
            patientUser = new ApplicationUser
            {
                UserName = patientEmail,
                Email = patientEmail,
                FullName = "Ahmad Hassan",
                EmailConfirmed = true
            };

            var result =
                await userManager.CreateAsync(
                    patientUser,
                    patientPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    $"Could not create patient user: {errors}");
            }
        }

        // Make sure Patient has PATIENT role
        if (!await userManager.IsInRoleAsync(
                patientUser,
                "PATIENT"))
        {
            var roleResult =
                await userManager.AddToRoleAsync(
                    patientUser,
                    "PATIENT");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    $"Could not assign PATIENT role: {errors}");
            }
        }

        // =========================================================
        // 5. Update existing medication stock and prices
        // =========================================================

        var existingMedications =
            await context.Medications.ToListAsync();

        if (existingMedications.Any())
        {
            foreach (var medication in existingMedications)
            {
                switch (medication.Name)
                {
                    case "Aspirin":
                        medication.StockQuantity = 100;
                        medication.UnitPrice = 5.50m;
                        break;

                    case "Atorvastatin":
                        medication.StockQuantity = 50;
                        medication.UnitPrice = 12.00m;
                        break;

                    case "Metoprolol":
                        medication.StockQuantity = 30;
                        medication.UnitPrice = 8.75m;
                        break;
                }
            }

            await context.SaveChangesAsync();
        }

        // =========================================================
        // 6. Don't add sample data again if patients exist
        // =========================================================

        if (await context.Patients.AnyAsync())
        {
            return;
        }

        // =========================================================
        // 7. Add sample patients
        // =========================================================

        var patients = new List<Patient>
        {
            new()
            {
                FirstName = "Ahmad",
                LastName = "Hassan",
                DateOfBirth =
                    new DateTime(1985, 4, 12),
                Gender = "Male",
                PhoneNumber = "0599000001"
            },

            new()
            {
                FirstName = "Sara",
                LastName = "Khalil",
                DateOfBirth =
                    new DateTime(1992, 8, 25),
                Gender = "Female",
                PhoneNumber = "0599000002"
            },

            new()
            {
                FirstName = "Omar",
                LastName = "Nasser",
                DateOfBirth =
                    new DateTime(1978, 11, 3),
                Gender = "Male",
                PhoneNumber = "0599000003"
            }
        };

        await context.Patients.AddRangeAsync(patients);

        await context.SaveChangesAsync();

        // =========================================================
        // 8. Add sample vital signs
        // =========================================================

        var vitalSigns = new List<VitalSign>
        {
            new()
            {
                PatientId = patients[0].Id,
                HeartRate = 78,
                SystolicPressure = 120,
                DiastolicPressure = 80,
                OxygenSaturation = 98.0m,
                RecordedAt =
                    DateTime.UtcNow.AddHours(-2),
                Notes = "Normal reading"
            },

            new()
            {
                PatientId = patients[1].Id,
                HeartRate = 82,
                SystolicPressure = 125,
                DiastolicPressure = 82,
                OxygenSaturation = 97.5m,
                RecordedAt =
                    DateTime.UtcNow.AddHours(-1),
                Notes = "Routine monitoring"
            },

            new()
            {
                PatientId = patients[2].Id,
                HeartRate = 88,
                SystolicPressure = 135,
                DiastolicPressure = 85,
                OxygenSaturation = 96.0m,
                RecordedAt =
                    DateTime.UtcNow.AddMinutes(-30),
                Notes = "Follow-up reading"
            }
        };

        // =========================================================
        // 9. Add sample medications
        // =========================================================

        var medications = new List<Medication>
        {
            new()
            {
                PatientId = patients[0].Id,
                Name = "Aspirin",
                Dosage = "81 mg",
                Frequency = "Once daily",
                StartDate =
                    new DateTime(2026, 1, 10),
                Notes = "Take after food",

                StockQuantity = 100,
                UnitPrice = 5.50m
            },

            new()
            {
                PatientId = patients[1].Id,
                Name = "Atorvastatin",
                Dosage = "20 mg",
                Frequency = "Once daily",
                StartDate =
                    new DateTime(2026, 2, 15),
                Notes = "Evening dose",

                StockQuantity = 50,
                UnitPrice = 12.00m
            },

            new()
            {
                PatientId = patients[2].Id,
                Name = "Metoprolol",
                Dosage = "25 mg",
                Frequency = "Twice daily",
                StartDate =
                    new DateTime(2026, 3, 1),
                Notes =
                    "Regular monitoring recommended",

                StockQuantity = 30,
                UnitPrice = 8.75m
            }
        };

        // =========================================================
        // 10. Add sample appointments
        // =========================================================

        var appointments = new List<Appointment>
        {
            new()
            {
                PatientId = patients[0].Id,
                AppointmentDate =
                    DateTime.UtcNow.AddDays(3),
                DoctorName = "Dr. Ahmad Saleh",
                Reason = "Cardiac follow-up",
                Status = "Scheduled",
                Notes =
                    "Bring previous vital sign records"
            },

            new()
            {
                PatientId = patients[1].Id,
                AppointmentDate =
                    DateTime.UtcNow.AddDays(5),
                DoctorName = "Dr. Lina Omar",
                Reason = "Routine cardiac check",
                Status = "Scheduled"
            },

            new()
            {
                PatientId = patients[2].Id,
                AppointmentDate =
                    DateTime.UtcNow.AddDays(7),
                DoctorName = "Dr. Ahmad Saleh",
                Reason = "Blood pressure review",
                Status = "Scheduled"
            }
        };

        // =========================================================
        // 11. Save all sample data
        // =========================================================

        await context.VitalSigns.AddRangeAsync(
            vitalSigns);

        await context.Medications.AddRangeAsync(
            medications);

        await context.Appointments.AddRangeAsync(
            appointments);

        await context.SaveChangesAsync();
    }
}