using System;
using FluentValidation;

namespace HealthMed.Hub.Domain.Appointments.UseCases.GetAppointmentByPatient;

public class GetAppointmentsByPatientIdUseCaseValidator : AbstractValidator<GetAppointmentsByPatientIdInput>
{
    public GetAppointmentsByPatientIdUseCaseValidator()
    {
    }
}
