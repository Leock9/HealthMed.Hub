using FluentValidation;

namespace HealthMed.Hub.Domain.AvailableTimes.UseCases.GetAvailableTimes;

public class GetAvailableTimesInputValidator : AbstractValidator<GetAvailableTimesInput>
{
    public GetAvailableTimesInputValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("DoctorId is required.");
    }
}
