using CardiacPatientMonitoring.Api.Entities;

namespace CardiacPatientMonitoring.Api.Services;

public interface IPatientRepository
{
    // Gets a patient by ID.
    // The repository is responsible for getting the data,
    // while the service will handle the business logic.
    Task<Patient?> GetByIdAsync(int id);
}