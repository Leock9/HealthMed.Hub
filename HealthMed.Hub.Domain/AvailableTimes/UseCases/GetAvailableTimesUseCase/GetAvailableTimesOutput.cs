using System;
using System.Collections.Generic;

namespace HealthMed.Hub.Domain.AvailableTimes.UseCases.GetAvailableTimes;

public record AvailableTimeDto(Guid Id, TimeOnly StartTime, TimeOnly EndTime, DayOfWeek DayOfWeek, bool IsOccupied);

public record GetAvailableTimesOutput(IEnumerable<AvailableTimeDto> AvailableTimes);
