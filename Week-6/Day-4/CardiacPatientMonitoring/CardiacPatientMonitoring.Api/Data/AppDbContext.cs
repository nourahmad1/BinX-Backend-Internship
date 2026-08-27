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

    // =========================================================
    // DbSets
    // =========================================================

    public DbSet<Patient> Patients =>
        Set<Patient>();

    public DbSet<VitalSign> VitalSigns =>
        Set<VitalSign>();

    public DbSet<Medication> Medications =>
        Set<Medication>();

    public DbSet<Appointment> Appointments =>
        Set<Appointment>();

    public DbSet<Prescription> Prescriptions =>
        Set<Prescription>();

    public DbSet<PrescriptionItem> PrescriptionItems =>
        Set<PrescriptionItem>();

    // =========================================================
    // Model Configuration
    // =========================================================

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =====================================================
        // ApplicationUser <-> Patient
        // =====================================================

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

        // =====================================================
        // Patient -> VitalSigns
        // =====================================================

        modelBuilder.Entity<Patient>()
            .HasMany(patient =>
                patient.VitalSigns)
            .WithOne(vitalSign =>
                vitalSign.Patient)
            .HasForeignKey(vitalSign =>
                vitalSign.PatientId)
            .OnDelete(
                DeleteBehavior.Cascade);

        // =====================================================
        // Patient -> Medications
        // =====================================================

        modelBuilder.Entity<Patient>()
            .HasMany(patient =>
                patient.Medications)
            .WithOne(medication =>
                medication.Patient)
            .HasForeignKey(medication =>
                medication.PatientId)
            .OnDelete(
                DeleteBehavior.Cascade);

        // =====================================================
        // Patient -> Appointments
        // =====================================================

        modelBuilder.Entity<Patient>()
            .HasMany(patient =>
                patient.Appointments)
            .WithOne(appointment =>
                appointment.Patient)
            .HasForeignKey(appointment =>
                appointment.PatientId)
            .OnDelete(
                DeleteBehavior.Cascade);

        // =====================================================
        // Patient -> Prescriptions
        // =====================================================

        modelBuilder.Entity<Patient>()
            .HasMany(patient =>
                patient.Prescriptions)
            .WithOne(prescription =>
                prescription.Patient)
            .HasForeignKey(prescription =>
                prescription.PatientId)
            .OnDelete(
                DeleteBehavior.Cascade);

        // =====================================================
        // Prescription -> PrescriptionItems
        // =====================================================

        modelBuilder.Entity<Prescription>()
            .HasMany(prescription =>
                prescription.Items)
            .WithOne(item =>
                item.Prescription)
            .HasForeignKey(item =>
                item.PrescriptionId)
            .OnDelete(
                DeleteBehavior.Cascade);

        // =====================================================
        // Medication -> PrescriptionItems
        // =====================================================

        modelBuilder.Entity<Medication>()
            .HasMany<PrescriptionItem>()
            .WithOne(item =>
                item.Medication)
            .HasForeignKey(item =>
                item.MedicationId)
            .OnDelete(
                DeleteBehavior.Restrict);

        // =====================================================
        // Decimal precision
        // =====================================================

        modelBuilder.Entity<VitalSign>()
            .Property(vitalSign =>
                vitalSign.OxygenSaturation)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Medication>()
            .Property(medication =>
                medication.UnitPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Prescription>()
            .Property(prescription =>
                prescription.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PrescriptionItem>()
            .Property(item =>
                item.UnitPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PrescriptionItem>()
            .Property(item =>
                item.LineTotal)
            .HasPrecision(18, 2);
    }
}