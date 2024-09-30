using FastEndpoints;
using HealthMed.Hub.Domain.AvailableTimes.UseCases.GetAvailableTimes;

namespace Available.List;

public class Mapper : Mapper<Request, Response, object>
{
    public GetAvailableTimesInput ToInput(Request r) => new(r.DoctorId);
}