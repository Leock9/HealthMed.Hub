using FluentValidation;
using HealthMed.Hub.Domain.Appointments.Gateways;
using HealthMed.Hub.Domain.Base.UseCases;
using Microsoft.Extensions.Logging;

namespace HealthMed.Hub.Domain.Appointments.UseCases.RegisterAppointment;

public interface IRegisterAppointmentUseCase : IUseCase<RegisterAppointmentInput, RegisterAppointmentOutput> { }

public class RegisterAppointmentUseCase : BaseUseCase<RegisterAppointmentInput>, IRegisterAppointmentUseCase
{
    private readonly ILogger<RegisterAppointmentUseCase> _logger;
    private readonly IAppointmentsGateway _appointmentGateway;

    public RegisterAppointmentUseCase
    (
        IValidator<RegisterAppointmentInput> validator,
        ILogger<RegisterAppointmentUseCase> logger,
        IAppointmentsGateway appointmentGateway
    ) : base(validator, logger)
    {
        _logger = logger;
        _appointmentGateway = appointmentGateway;
    }

    public async Task<RegisterAppointmentOutput> HandleAsync(RegisterAppointmentInput input, CancellationToken cancellationToken)
    {
        try
        {
            Validate(input);
            var appointments = await _appointmentGateway.GetByDoctorIdAsync(input.DoctorId);
            
            //Tem que validar se existe um horário disponível, pelo caso de uso getbyid do availbletimes

            var newAppointment = new Appointment
                (
                    input.DoctorId,
                    input.PatientId,
                    input.AvailabeTimeId,
                    DateTime.Now
                );

            if (appointments.Any())
            {
                Parallel.ForEach(appointments, appointment =>
                {
                    if (!appointment.IsSameAppointment(newAppointment))
                        throw new ValidationException("Doctor already has an appointment in this period");
                });
            }

            await _appointmentGateway.CreateAsync(newAppointment);
            return new RegisterAppointmentOutput(newAppointment.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
