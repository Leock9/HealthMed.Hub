namespace HealthMed.Hub.Domain.AvailableTimes.UseCases.RegisterAvailableTime;

public record RegisterAvailableTimeInput(TimeOnly StartTime, TimeOnly EndTime, Guid DoctorId, DayOfWeek DayOfWeek);

