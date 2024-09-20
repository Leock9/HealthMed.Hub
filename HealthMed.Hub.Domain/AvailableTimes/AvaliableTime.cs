using System;
using HealthMed.Hub.Domain.Base;

namespace HealthMed.Hub.Domain.AvailableTimes;

public record AvaliableTime(TimeOnly StartTime, TimeOnly EndTime, Guid DoctorId, DayOfWeek DayOfWeek)
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public TimeOnly StartTime { get; init; } = StartTime < EndTime ?
                                               StartTime : throw new DomainException("StartTime must be earlier than EndTime");

    public TimeOnly EndTime { get; init; } = EndTime > StartTime ?
                                             EndTime : throw new DomainException("EndTime must be later than StartTime");

    public Guid DoctorId { get; init; } = DoctorId != Guid.Empty ?
                                          DoctorId : throw new DomainException("DoctorId cannot be an empty GUID");

    public DayOfWeek DayOfWeek { get; init; } = DayOfWeek;
}
