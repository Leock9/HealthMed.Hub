using FastEndpoints;
using HealthMed.Hub.Domain.Appointments.UseCases.RegisterAppointment;

namespace Appointments.Register;

public class Mapper : Mapper<Request, Response, object>
{
    public RegisterAppointmentInput ToInput(Request r) => new(r.DoctorId, r.PatientId, r.AvailabeTimeId);
}