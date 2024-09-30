using FastEndpoints;
using FluentValidation;
using HealthMed.Hub.Domain.AvailableTimes.UseCases.GetAvailableTimes;

namespace Available.List;

public class Request
{
    public Guid DoctorId { get; set; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.DoctorId).NotEmpty();
    }
}

public class Response
{
    public GetAvailableTimesOutput Message { get; set; } = new GetAvailableTimesOutput(new List<AvailableTimeDto>());
}
