using FastEndpoints;
using HealthMed.Hub.Domain.Base;
using HealthMed.Hub.Domain.Doctors.UseCases.RegisterDoctor;
using System.Net;

namespace RegisterDoctor;

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    public ILogger<Endpoint> Log { get; set; } = null!;
    public IRegisterDoctorUseCase UseCase { get; set; } = null!;

    public override void Configure()
    {
        AllowAnonymous();
        Post("/doctors/register");
    }

    public override async Task HandleAsync(Request r, CancellationToken c)
    {
        try
        {
            var doctorId = await UseCase.HandleAsync(Map.ToInput(r), c);
            await SendAsync(new Response { DoctorId = doctorId.Id}, cancellation: c);
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