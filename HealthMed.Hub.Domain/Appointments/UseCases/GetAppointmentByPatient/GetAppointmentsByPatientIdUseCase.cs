using FluentValidation;
using HealthMed.Hub.Domain.Appointments.Gateways;
using HealthMed.Hub.Domain.Appointments.UseCases.GetAppointmentByPatient;
using HealthMed.Hub.Domain.Base.UseCases;
using Microsoft.Extensions.Logging;

namespace HealthMed.Hub.Domain.Appointments.UseCases.GetAppointmentsByPatientId;

public interface IGetAppointmentsByPatientIdUseCase : IUseCase<GetAppointmentsByPatientIdInput, GetAppointmentsByPatientIdOutput> { }

public class GetAppointmentsByPatientIdUseCase : BaseUseCase<GetAppointmentsByPatientIdInput>, IGetAppointmentsByPatientIdUseCase
{
    private readonly ILogger<GetAppointmentsByPatientIdUseCase> _logger;
    private readonly IAppointmentsGateway _appointmentGateway;

    public GetAppointmentsByPatientIdUseCase
    (
        IValidator<GetAppointmentsByPatientIdInput> validator,
        ILogger<GetAppointmentsByPatientIdUseCase> logger,
        IAppointmentsGateway appointmentGateway
    ) : base(validator, logger)
    {
        _logger = logger;
        _appointmentGateway = appointmentGateway;
    }

    public async Task<GetAppointmentsByPatientIdOutput> HandleAsync(GetAppointmentsByPatientIdInput input, CancellationToken cancellationToken)
    {
        try
        {
            Validate(input);
            var appointments = await _appointmentGateway.GetByPatientAsync(input.PatientId);
            return new GetAppointmentsByPatientIdOutput(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
