using FastEndpoints;
using HealthMed.Hub.Domain.AvailableTimes.UseCases.RegisterAvailableTime;

namespace Available
{
    public class Mapper : Mapper<Request, Response, object>
    {
        public RegisterAvailableTimeInput ToInput(Request r) => new
        (
            TimeOnly.ParseExact(r.StartTime, "HH:mm"), 
            TimeOnly.ParseExact(r.EndTime, "HH:mm"), 
            r.DoctorId, 
            r.DayOfWeek
        );
    }
}