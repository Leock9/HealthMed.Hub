using FluentValidation;

namespace HealthMed.Hub.Domain.AvailableTimes.UseCases.UpdateAvailableTime;

public class UpdateAvailableTimeInputValidator : AbstractValidator<UpdateAvailableTimeInput>
{
    public UpdateAvailableTimeInputValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("StartTime is required");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("EndTime is required")
            .GreaterThan(x => x.StartTime).WithMessage("EndTime must be after StartTime");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("DoctorId is required");

        RuleFor(x => x.DayOfWeek)
            .IsInEnum().WithMessage("DayOfWeek is required and must be a valid day of the week");
    }
}
