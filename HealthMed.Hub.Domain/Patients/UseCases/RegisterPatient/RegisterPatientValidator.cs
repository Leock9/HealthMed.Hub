using FluentValidation;

namespace HealthMed.Hub.Domain.Patients.UseCases.RegisterPatient;

public class RegisterPatientValidator : AbstractValidator<RegisterPatientInput>
{
    public RegisterPatientValidator()
    {
        RuleFor(x => x.Name)
                                      .NotEmpty()
                                      .WithMessage("Name is required");

        RuleFor(x => x.Document)
                                      .NotEmpty()
                                      .WithMessage("Document is required");

        RuleFor(x => x.Email)
                                      .NotEmpty()
                                      .WithMessage("Email is required");
    }
}
