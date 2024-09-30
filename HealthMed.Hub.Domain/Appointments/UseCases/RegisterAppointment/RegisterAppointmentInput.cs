namespace HealthMed.Hub.Domain.Appointments.UseCases.RegisterAppointment;

public record RegisterAppointmentInput(Guid DoctorId, Guid PatientId, Guid AvailabeTimeId);