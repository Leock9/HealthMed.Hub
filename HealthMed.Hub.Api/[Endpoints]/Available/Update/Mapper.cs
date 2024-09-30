using FastEndpoints;
using HealthMed.Hub.Domain.AvailableTimes.UseCases.UpdateAvailableTime;

namespace Available.Update;

public class Mapper : Mapper<Request, Response, object>
{
    public UpdateAvailableTimeInput ToInput(Request r) => new
    (
        r.Id,
        TimeOnly.ParseExact(r.StartTime, "HH:mm"),
        TimeOnly.ParseExact(r.EndTime, "HH:mm"),
        r.DoctorId,
        r.DayOfWeek
);
}