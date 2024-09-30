﻿using FastEndpoints;
using HealthMed.Hub.Domain.AvailableTimes.UseCases.RegisterAvailableTime;
using HealthMed.Hub.Domain.AvailableTimes.UseCases.UpdateAvailableTime;
using HealthMed.Hub.Domain.Base;
using System.Net;

namespace Available.Update;

public class Endpoint : Endpoint<Request, Response, Mapper>
{
    public ILogger<Endpoint> Log { get; set; } = null!;
    public IUpdateAvailableTimeUseCase UseCase { get; set; } = null!;

    public override void Configure()
    {
        AllowAnonymous();
        Put("/doctors/available");
    }

    public override async  Task HandleAsync(Request r, CancellationToken c)
    {
        try
        {
            var output = await UseCase.HandleAsync(Map.ToInput(r), c);
            await SendAsync(new Response(), cancellation: c);
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