using FastEndpoints;
using HealthMed.Hub.Domain.Base;
using HealthMed.Hub.Domain.Doctors.UseCases.GetDoctors;
using System.Net;

namespace Doctors.List;

public class Endpoint : EndpointWithoutRequest<Doctors.List.Response>
{
    public ILogger<Endpoint> Log { get; set; } = null!;
    public IGetDoctorsUseCase UseCase { get; set; } = null!;

    public override void Configure()
    {
        AllowAnonymous();
        Get("/doctors/available");
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        try
        {
            var output = await UseCase.HandleAsync(new GetDoctorsInput(), c);
            await SendAsync(new Response { Message = output }, cancellation: c);
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