using FastEndpoints;
using HealthMed.Hub.Domain.AvailableTimes.UseCases.GetAvailableTimes;
using HealthMed.Hub.Domain.Base;
using System.Net;

namespace Available.List;

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    public ILogger<Endpoint> Log { get; set; } = null!;
    public IGetAvailableTimesUseCase UseCase { get; set; } = null!;

    public override void Configure()
    {
        AllowAnonymous();
        Get("/doctors/availabletimes");
    }

    public override async Task HandleAsync(Request r, CancellationToken c)
    {
        try
        {
            var output = await UseCase.HandleAsync(Map.ToInput(r), c);
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