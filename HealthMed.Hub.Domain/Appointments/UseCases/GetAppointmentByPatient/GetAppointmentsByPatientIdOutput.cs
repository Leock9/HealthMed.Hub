namespace HealthMed.Hub.Domain.Appointments.UseCases.GetAppointmentByPatient;

public record GetAppointmentsByPatientIdOutput(IEnumerable<Appointment> Appointments);
