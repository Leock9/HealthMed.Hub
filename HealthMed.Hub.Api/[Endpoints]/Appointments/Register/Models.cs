using FastEndpoints;
using FluentValidation;

namespace Appointments.Register;

public class Request
{
    public Guid DoctorId { get; init; }
    public Guid PatientId { get; init; }
    public Guid AvailabeTimeId { get; init; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.AvailabeTimeId).NotEmpty();
    }
}

public class Response
{
    public Guid AppointmentId { get; init; } = Guid.Empty;
}
