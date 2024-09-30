using FastEndpoints;
using HealthMed.Hub.Domain.Doctors.UseCases.GetDoctors;

namespace Doctors.List;

public class Response
{
    public GetDoctorsOutput Message { get; set; } = default!;
}
