using FastEndpoints;
using HealthMed.Hub.Domain.Doctors.UseCases.RegisterDoctor;

namespace RegisterDoctor;

public class Mapper : Mapper<Request, Response, object>
{
    public RegisterDoctorInput ToInput(Request r) => new(r.Name, r.Crm, r.Document, r.Email);
}