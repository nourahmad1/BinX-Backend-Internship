using CardiacPatientMonitoring.Api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CardiacPatientMonitoring.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Database tables used by the application
    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<VitalSign> VitalSigns => Set<VitalSign>();

    public DbSet<Medication> Medications => Set<Medication>();

    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // A patient can have multiple vital sign records
        modelBuilder.Entity<Patient>()
            .HasMany(patient => patient.VitalSigns)
            .WithOne(vitalSign => vitalSign.Patient)
            .HasForeignKey(vitalSign => vitalSign.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // A patient can have multiple medications
        modelBuilder.Entity<Patient>()
            .HasMany(patient => patient.Medications)
            .WithOne(medication => medication.Patient)
            .HasForeignKey(medication => medication.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // A patient can have multiple appointments
        modelBuilder.Entity<Patient>()
            .HasMany(patient => patient.Appointments)
            .WithOne(appointment => appointment.Patient)
            .HasForeignKey(appointment => appointment.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Store oxygen saturation with two decimal places
        modelBuilder.Entity<VitalSign>()
            .Property(vitalSign => vitalSign.OxygenSaturation)
            .HasPrecision(5, 2);
    }
}