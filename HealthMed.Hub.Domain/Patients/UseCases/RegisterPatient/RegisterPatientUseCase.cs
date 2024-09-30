using FluentValidation;
using HealthMed.Hub.Domain.Base;
using HealthMed.Hub.Domain.Base.UseCases;
using HealthMed.Hub.Domain.Patients.Gateways;
using Microsoft.Extensions.Logging;
namespace HealthMed.Hub.Domain.Patients.UseCases.RegisterPatient;

public interface IRegisterPatientUseCase : IUseCase<RegisterPatientInput, RegisterPatientOutput>{}

public class RegisterPatientUseCase : BaseUseCase<RegisterPatientInput>, IRegisterPatientUseCase
{
    private readonly ILogger<RegisterPatientUseCase> _logger;
    public readonly IPatientGateway _patientGateway;

    public RegisterPatientUseCase
    (
        IValidator<RegisterPatientInput> validator,
        ILogger<RegisterPatientUseCase> logger,
        IPatientGateway patientGateway
    ) : base(validator, logger)
    {
        _logger = logger;
        _patientGateway = patientGateway;
    }

    public async Task<RegisterPatientOutput> HandleAsync(RegisterPatientInput input, CancellationToken cancellationToken)
    {
        try
        {
            Validate(input);
            var patient = await _patientGateway.GetByDocumentAsync(input.Document);

            if (patient != null)
                ThrowError("Document", "Document already exists");

            patient ??= new Patient(input.Name, input.Document, input.Email);

            await _patientGateway.CreateAsync(patient);

            return new RegisterPatientOutput(patient.Id.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
