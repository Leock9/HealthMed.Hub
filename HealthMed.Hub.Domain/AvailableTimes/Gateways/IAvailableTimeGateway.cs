namespace HealthMed.Hub.Domain.AvailableTimes.Gateways;

public interface IAvailableTimeGateway
{
    Task<AvaliableTime> CreateAsync(AvaliableTime avaliableTime);
    Task<AvaliableTime> UpdateAsync(AvaliableTime avaliableTime);
    Task<AvaliableTime> GetByIdAsync(Guid id);
    Task<IEnumerable<AvaliableTime>> GetAsync();
    Task DeleteAsync(Guid id);
    Task<IEnumerable<AvaliableTime>> GetByDoctorIdAsync(Guid doctorId);
}
