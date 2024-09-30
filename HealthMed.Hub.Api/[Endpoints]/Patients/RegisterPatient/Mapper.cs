using FastEndpoints;
using HealthMed.Hub.Domain.Patients.UseCases.RegisterPatient;

namespace RegisterPatient;

public class Mapper : Mapper<Request, Response, object>
{
    public RegisterPatientInput ToInput(Request request) => new(request.Name, request.Document, request.Email);
}