using FluentValidation;

namespace HealthMed.Hub.Domain.Appointments.UseCases.RegisterAppointment;

public class RegisterAppointmentValidator : AbstractValidator<RegisterAppointmentInput>
{
    public RegisterAppointmentValidator()
    {
        RuleFor(x => x.DoctorId)
                                      .NotEmpty()
                                      .WithMessage("DoctorId is required");

        RuleFor(x => x.PatientId)
                                      .NotEmpty()
                                      .WithMessage("PatientId is required");

        RuleFor(x => x.AvailabeTimeId)
                                      .NotEmpty()
                                      .WithMessage("AvailabeTimeId is required");
    }
}
