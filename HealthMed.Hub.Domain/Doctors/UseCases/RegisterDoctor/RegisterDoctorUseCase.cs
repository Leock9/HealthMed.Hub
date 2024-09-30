using FluentValidation;
using HealthMed.Hub.Domain.Base;
using HealthMed.Hub.Domain.Base.UseCases;
using HealthMed.Hub.Domain.Doctors.Gateways;
using Microsoft.Extensions.Logging;
namespace HealthMed.Hub.Domain.Doctors.UseCases.RegisterDoctor;

public interface IRegisterDoctorUseCase : IUseCase<RegisterDoctorInput, RegisterDoctorOutput>{}

public class RegisterDoctorUseCase : BaseUseCase<RegisterDoctorInput>, IRegisterDoctorUseCase
{
    private readonly ILogger<RegisterDoctorUseCase> _logger;
    public IDoctorGateway _doctorGateway;

    public RegisterDoctorUseCase
    (
        IValidator<RegisterDoctorInput> validator,
        ILogger<RegisterDoctorUseCase> logger,
        IDoctorGateway doctorGateway
    ) : base(validator, logger)
    {
        _logger = logger;
        _doctorGateway = doctorGateway;
    }

    public async Task<RegisterDoctorOutput> HandleAsync(RegisterDoctorInput input, CancellationToken cancellationToken)
    {
        try
        {
            Validate(input);
            var doctor = await _doctorGateway.GetByCrmAsync(input.Crm);

            if (doctor != null)
                ThrowError("Crm", "Crm already exists");

            doctor ??= new Doctor(input.Name, input.Crm, input.Document, input.Email);

            await _doctorGateway.CreateAsync(doctor);

            return new RegisterDoctorOutput(doctor.Id.Value);
        }
        catch (ValidationException vx)
        {
            _logger.LogError(vx, vx.Message);
            throw;
        }
        catch (DomainException dx) 
        {
            _logger.LogError(dx, dx.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
