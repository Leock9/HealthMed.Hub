using FluentValidation;

namespace HealthMed.Hub.Domain.Doctors.UseCases.RegisterDoctor;

public class RegisterDoctorValidator : AbstractValidator<RegisterDoctorInput>
{
    public RegisterDoctorValidator()
    {
        RuleFor(x => x.Name)
                                      .NotEmpty()
                                      .WithMessage("Name is required");

        RuleFor(x => x.Crm)
                                      .NotEmpty()
                                      .WithMessage("Crm is required");

        RuleFor(x => x.Document)
                                      .NotEmpty()
                                      .WithMessage("Document is required");

        RuleFor(x => x.Email)
                                      .NotEmpty()
                                      .WithMessage("Email is required");
    }
}
