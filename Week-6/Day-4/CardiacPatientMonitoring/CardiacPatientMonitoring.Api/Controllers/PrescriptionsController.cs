using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CardiacPatientMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,DOCTOR")]
public class PrescriptionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PrescriptionsController(AppDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // POST: api/Prescriptions
    // Create a new prescription
    // =========================================================

    [HttpPost]
    public async Task<ActionResult<PrescriptionResponseDto>> CreatePrescription(
        PrescriptionCreateDto dto)
    {
        // =====================================================
        // 1. Validate patient
        // =====================================================

        var patientExists = await _context.Patients
            .AsNoTracking()
            .AnyAsync(patient => patient.Id == dto.PatientId);

        if (!patientExists)
        {
            return NotFound(new
            {
                message =
                    $"Patient with ID {dto.PatientId} was not found."
            });
        }

        // =====================================================
        // 2. Validate items
        // =====================================================

        if (dto.Items is null || dto.Items.Count == 0)
        {
            return BadRequest(new
            {
                message =
                    "Prescription must contain at least one item."
            });
        }

        if (dto.Items.Any(item => item.Quantity <= 0))
        {
            return BadRequest(new
            {
                message =
                    "Quantity must be greater than zero."
            });
        }

        // =====================================================
        // 3. Start database transaction
        // =====================================================

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            // =================================================
            // 4. Load medications
            // =================================================

            var medicationIds =
                dto.Items
                    .Select(item => item.MedicationId)
                    .Distinct()
                    .ToList();

            var medications =
                await _context.Medications
                    .Where(medication =>
                        medicationIds.Contains(medication.Id))
                    .ToListAsync();

            // =================================================
            // 5. Check that all medications exist
            // =================================================

            var missingMedicationIds =
                medicationIds
                    .Except(
                        medications.Select(
                            medication => medication.Id))
                    .ToList();

            if (missingMedicationIds.Count > 0)
            {
                return NotFound(new
                {
                    message =
                        "One or more medications were not found.",
                    medicationIds = missingMedicationIds
                });
            }

            // =================================================
            // 6. Check stock availability
            // =================================================

            foreach (var item in dto.Items)
            {
                var medication =
                    medications.First(
                        medication =>
                            medication.Id ==
                            item.MedicationId);

                if (medication.StockQuantity < item.Quantity)
                {
                    return BadRequest(new
                    {
                        message =
                            $"Insufficient stock for medication '{medication.Name}'.",
                        availableStock =
                            medication.StockQuantity,
                        requestedQuantity =
                            item.Quantity
                    });
                }
            }

            // =================================================
            // 7. Create prescription
            // =================================================

            var prescription = new Prescription
            {
                PatientId = dto.PatientId,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = 0
            };

            await _context.Prescriptions.AddAsync(
                prescription);

            // =================================================
            // 8. Create prescription items
            // =================================================

            decimal totalAmount = 0;

            foreach (var item in dto.Items)
            {
                var medication =
                    medications.First(
                        medication =>
                            medication.Id ==
                            item.MedicationId);

                var lineTotal =
                    medication.UnitPrice *
                    item.Quantity;

                var prescriptionItem =
                    new PrescriptionItem
                    {
                        Prescription = prescription,
                        MedicationId = medication.Id,
                        Quantity = item.Quantity,
                        UnitPrice = medication.UnitPrice,
                        LineTotal = lineTotal
                    };

                await _context.PrescriptionItems.AddAsync(
                    prescriptionItem);

                // =================================================
                // 9. Decrease medication stock
                // =================================================

                medication.StockQuantity -= item.Quantity;

                // =================================================
                // 10. Calculate total
                // =================================================

                totalAmount += lineTotal;
            }

            prescription.TotalAmount = totalAmount;

            // =================================================
            // 11. Save everything
            // =================================================

            await _context.SaveChangesAsync();

            // =================================================
            // 12. Commit transaction
            // =================================================

            await transaction.CommitAsync();

            // =================================================
            // 13. Build response
            // =================================================

            var response = new PrescriptionResponseDto
            {
                Id = prescription.Id,
                PatientId = prescription.PatientId,
                CreatedAt = prescription.CreatedAt,
                TotalAmount = prescription.TotalAmount,
                Items = prescription.Items
                    .Select(item => new PrescriptionItemResponseDto
                    {
                        Id = item.Id,
                        MedicationId = item.MedicationId,
                        MedicationName =
                            medications.First(
                                medication =>
                                    medication.Id ==
                                    item.MedicationId)
                            .Name,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        LineTotal = item.LineTotal
                    })
                    .ToList()
            };

            return CreatedAtAction(
                nameof(GetPrescription),
                new { id = prescription.Id },
                response);
        }
        catch
        {
            // =================================================
            // Rollback if anything fails
            // =================================================

            await transaction.RollbackAsync();

            throw;
        }
    }

    // =========================================================
    // GET: api/Prescriptions/{id}
    // =========================================================

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PrescriptionResponseDto>>
        GetPrescription(int id)
    {
        var prescription =
            await _context.Prescriptions
                .AsNoTracking()
                .Include(prescription =>
                    prescription.Items)
                .ThenInclude(item =>
                    item.Medication)
                .FirstOrDefaultAsync(
                    prescription =>
                        prescription.Id == id);

        if (prescription is null)
        {
            return NotFound(new
            {
                message =
                    $"Prescription with ID {id} was not found."
            });
        }

        var response =
            new PrescriptionResponseDto
            {
                Id = prescription.Id,
                PatientId = prescription.PatientId,
                CreatedAt = prescription.CreatedAt,
                TotalAmount = prescription.TotalAmount,

                Items = prescription.Items
                    .Select(item =>
                        new PrescriptionItemResponseDto
                        {
                            Id = item.Id,
                            MedicationId =
                                item.MedicationId,

                            MedicationName =
                                item.Medication.Name,

                            Quantity =
                                item.Quantity,

                            UnitPrice =
                                item.UnitPrice,

                            LineTotal =
                                item.LineTotal
                        })
                    .ToList()
            };

        return Ok(response);
    }
}