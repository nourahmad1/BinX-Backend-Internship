using CardiacPatientMonitoring.Api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CardiacPatientMonitoring.Api.Data;

public class AppDbContext
    : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients =>
        Set<Patient>();

    public DbSet<VitalSign> VitalSigns =>
        Set<VitalSign>();

    public DbSet<Medication> Medications =>
        Set<Medication>();

    public DbSet<Appointment> Appointments =>
        Set<Appointment>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ApplicationUser <-> Patient
        modelBuilder.Entity<Patient>()
            .HasOne(patient =>
                patient.ApplicationUser)
            .WithOne(user =>
                user.Patient)
            .HasForeignKey<Patient>(
                patient =>
                    patient.ApplicationUserId)
            .OnDelete(
                DeleteBehavior.SetNull);

        // Patient -> VitalSigns
        modelBuilder.Entity<Patient>()
            .HasMany(patient =>
                patient.VitalSigns)
            .WithOne(vitalSign =>
                vitalSign.Patient)
            .HasForeignKey(vitalSign =>
                vitalSign.PatientId)
            .OnDelete(
                DeleteBehavior.Cascade);

        // Patient -> Medications
        modelBuilder.Entity<Patient>()
            .HasMany(patient =>
                patient.Medications)
            .WithOne(medication =>
                medication.Patient)
            .HasForeignKey(medication =>
                medication.PatientId)
            .OnDelete(
                DeleteBehavior.Cascade);

        // Patient -> Appointments
        modelBuilder.Entity<Patient>()
            .HasMany(patient =>
                patient.Appointments)
            .WithOne(appointment =>
                appointment.Patient)
            .HasForeignKey(appointment =>
                appointment.PatientId)
            .OnDelete(
                DeleteBehavior.Cascade);

        // Oxygen saturation precision
        modelBuilder.Entity<VitalSign>()
            .Property(vitalSign =>
                vitalSign.OxygenSaturation)
            .HasPrecision(5, 2);
    }
}