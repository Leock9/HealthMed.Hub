using System;

namespace HealthMed.Hub.Domain.AvailableTimes.UseCases.UpdateAvailableTime;

public record UpdateAvailableTimeInput(Guid Id, TimeOnly StartTime, TimeOnly EndTime, Guid DoctorId, DayOfWeek DayOfWeek);
