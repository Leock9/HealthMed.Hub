using FluentValidation;
using HealthMed.Hub.Domain.Base.UseCases;
using HealthMed.Hub.Domain.Doctors.Gateways;
using Microsoft.Extensions.Logging;

namespace HealthMed.Hub.Domain.Doctors.UseCases.GetDoctors;

public interface IGetDoctorsUseCase : IUseCase<GetDoctorsInput, GetDoctorsOutput> {}
public class GetDoctorsUseCase : BaseUseCase<GetDoctorsInput>, IGetDoctorsUseCase
{
    private readonly ILogger<GetDoctorsUseCase> _logger;
    private readonly IDoctorGateway _doctorGateway;

    public GetDoctorsUseCase
    (
        IValidator<GetDoctorsInput> validator,
        ILogger<GetDoctorsUseCase> logger,
        IDoctorGateway doctorGateway
    ) : base(validator, logger)
    {
        _logger = logger;
        _doctorGateway = doctorGateway;
    }

    public async Task<GetDoctorsOutput> HandleAsync(GetDoctorsInput input, CancellationToken cancellationToken)
    {
        try
        {
            Validate(input);

            var doctors = await _doctorGateway.GetAsync();

            var doctorDtos = doctors.Select(d => new DoctorDto(
                d.Id.Value,
                d.Name,
                d.Crm,
                d.Document,
                d.Email
            ));

            return new GetDoctorsOutput(doctorDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
