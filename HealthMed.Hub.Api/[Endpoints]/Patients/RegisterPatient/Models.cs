using FastEndpoints;
using FluentValidation;

namespace RegisterPatient;

public class Request
{
    public required string Name { get; init; }
    public required string Document { get; init; }
    public required string Email { get; init; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Document).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
    }
}

public class Response
{
    public Guid PatientId { get; init; } = Guid.Empty;
}
