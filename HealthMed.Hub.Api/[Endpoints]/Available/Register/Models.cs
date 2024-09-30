using FastEndpoints;
using FluentValidation;

namespace Available;

public class Request
{
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public Guid DoctorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("StartTime não pode estar vazio")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("StartTime deve estar no formato HH:mm")
            .Must(BeAValidTime).WithMessage("StartTime deve ser um horário válido no formato HH:mm");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("EndTime não pode estar vazio")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("EndTime deve estar no formato HH:mm")
            .Must(BeAValidTime).WithMessage("EndTime deve ser um horário válido no formato HH:mm");
            
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.DayOfWeek).NotEmpty();
    }

    private static bool BeAValidTime(string time) => TimeOnly.TryParseExact(time, "HH:mm", out _);
}

public class Response
{
    public Guid AvailableId { get; init; } = Guid.Empty;
}
