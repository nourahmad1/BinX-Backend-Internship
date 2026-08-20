using CardiacPatientMonitoring.Api.Entities;

namespace CardiacPatientMonitoring.Api.Services;

public class PatientService
{
    private readonly IPatientRepository _repository;

    public PatientService(IPatientRepository repository)
    {
        _repository = repository;
    }

    // Gets a patient and returns the full name.
    // If the repository fails, the service returns null instead of
    // letting the database error break the whole operation.
    public async Task<string?> GetPatientFullNameAsync(int id)
    {
        try
        {
            var patient = await _repository.GetByIdAsync(id);

            if (patient is null)
            {
                return null;
            }

            return $"{patient.FirstName} {patient.LastName}";
        }
        catch (Exception)
        {
            // Keep this simple for the training example.
            // A real application would usually log the exception.
            return null;
        }
    }
}