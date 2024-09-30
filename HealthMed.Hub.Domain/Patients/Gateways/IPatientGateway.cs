namespace HealthMed.Hub.Domain.Patients.Gateways;

public interface IPatientGateway
{
    Task<Patient> CreateAsync(Patient patient);
    Task<Patient> UpdateAsync(Patient patient);
    Task<Patient> GetByIdAsync(Guid id);
    Task<IEnumerable<Patient>> GetAsync();
    Task<Patient> GetByDocumentAsync(string document);
    Task DeleteAsync(Guid id);
}
