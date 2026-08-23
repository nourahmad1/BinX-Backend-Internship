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
        // Create Roles
        // =========================================================

        string[] roles =
        {
            "Admin",
            "Doctor",
            "Patient"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new IdentityRole(role));
            }
        }

        // =========================================================
        // Create Admin User
        // =========================================================

        const string adminEmail = "admin@cardiac.com";
        const string adminPassword = "Admin123";

        var adminUser =
            await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail
            };

            var result = await userManager.CreateAsync(
                adminUser,
                adminPassword);

            if (!result.Succeeded)
            {
                throw new Exception(
                    "Failed to create admin user: " +
                    string.Join(
                        ", ",
                        result.Errors.Select(e => e.Description)));
            }
        }

        // Make sure Admin role is assigned
        if (!await userManager.IsInRoleAsync(
                adminUser,
                "Admin"))
        {
            await userManager.AddToRoleAsync(
                adminUser,
                "Admin");
        }

        // =========================================================
        // Create Doctor User
        // =========================================================

        const string doctorEmail = "doctor@cardiac.com";
        const string doctorPassword = "Doctor123";

        var doctorUser =
            await userManager.FindByEmailAsync(doctorEmail);

        if (doctorUser == null)
        {
            doctorUser = new ApplicationUser
            {
                UserName = doctorEmail,
                Email = doctorEmail
            };

            var result = await userManager.CreateAsync(
                doctorUser,
                doctorPassword);

            if (!result.Succeeded)
            {
                throw new Exception(
                    "Failed to create doctor user: " +
                    string.Join(
                        ", ",
                        result.Errors.Select(e => e.Description)));
            }
        }

        // Make sure Doctor role is assigned
        if (!await userManager.IsInRoleAsync(
                doctorUser,
                "Doctor"))
        {
            await userManager.AddToRoleAsync(
                doctorUser,
                "Doctor");
        }

        // =========================================================
        // Create Patient User
        // =========================================================

        const string patientEmail = "patient@cardiac.com";
        const string patientPassword = "Patient123";

        var patientUser =
            await userManager.FindByEmailAsync(patientEmail);

        if (patientUser == null)
        {
            patientUser = new ApplicationUser
            {
                UserName = patientEmail,
                Email = patientEmail
            };

            var result = await userManager.CreateAsync(
                patientUser,
                patientPassword);

            if (!result.Succeeded)
            {
                throw new Exception(
                    "Failed to create patient user: " +
                    string.Join(
                        ", ",
                        result.Errors.Select(e => e.Description)));
            }
        }

        // Make sure Patient role is assigned
        if (!await userManager.IsInRoleAsync(
                patientUser,
                "Patient"))
        {
            await userManager.AddToRoleAsync(
                patientUser,
                "Patient");
        }

        // =========================================================
        // Existing Sample Data
        // =========================================================

        // Keep your existing patient/medication/appointment
        // seed data here if you already have it.
    }
}