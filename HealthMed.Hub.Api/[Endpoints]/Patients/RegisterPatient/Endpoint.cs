using FastEndpoints;
using HealthMed.Hub.Domain.Base;
using HealthMed.Hub.Domain.Patients.UseCases.RegisterPatient;
using System.Net;
namespace RegisterPatient;

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    public ILogger<Endpoint> Log { get; set; } = null!;
    public IRegisterPatientUseCase UseCase { get; set; } = null!;

    public override void Configure()
    {
        AllowAnonymous();
        Post("/patients/register");
    }

    public override async Task HandleAsync(Request r, CancellationToken c)
    {
        try
        {
            var output = await UseCase.HandleAsync(Map.ToInput(r), c);
            await SendAsync(new Response { PatientId = output.Id }, cancellation: c);
        }
        catch (DomainException dx)
        {
            ThrowError(dx.Message);
        }
        catch (Exception ex)
        {
            Log.LogError("Ocorreu um erro inesperado ao executar o endpoint:{typeof(Endpoint).Namespace}. {ex.Message}", typeof(Endpoint).Namespace, ex.Message);
            ThrowError("Unexpected Error", (int)HttpStatusCode.BadRequest);
        }
    }
}