namespace HealthMed.Hub.Domain.Appointments.Gateways;

public interface IAppointmentsGateway
{
    Task<Appointment> CreateAsync(Appointment appointment);
    Task<Appointment> UpdateAsync(Appointment appointments);
    Task<Appointment> GetByIdAsync(Guid id);
    Task<IEnumerable<Appointment>> GetAsync();
    Task DeleteAsync(Guid id);
}
