namespace HealthMed.Hub.Domain.Doctors.Gateways;

public interface IDoctorGateway
{
    Task<Doctor> CreateAsync(Doctor doctor);
    Task<Doctor> UpdateAsync(Doctor doctor);
    Task<Doctor> GetByIdAsync(Guid id);
    Task<Doctor> GetByCrmAsync(string crm);
    Task<IEnumerable<Doctor>> GetAsync();
    Task DeleteAsync(Guid id);
}
