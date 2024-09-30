using FluentValidation;

namespace HealthMed.Hub.Domain.AvailableTimes.UseCases.RegisterAvailableTime;

public class RegisterAvailableTimeValidator : AbstractValidator<RegisterAvailableTimeInput>
{
    public RegisterAvailableTimeValidator()
    {
        RuleFor(x => x.DoctorId)
                                      .NotEmpty()
                                      .WithMessage("DoctorId is required");

        RuleFor(x => x.StartTime)
                                      .NotEmpty()
                                      .WithMessage("StartTime is required");

        RuleFor(x => x.EndTime)
                                      .NotEmpty()
                                      .WithMessage("EndTime is required");

        RuleFor(x => x.DayOfWeek)
                                        .NotEmpty()
                                        .WithMessage("DayOfWeek is required");
    }
}
