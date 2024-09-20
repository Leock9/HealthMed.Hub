using HealthMed.Hub.Domain.Base;

namespace HealthMed.Hub.Domain.Appointments;

public record Appointment(Guid DoctorId, Guid PatientId, Guid AvailableTimeId, DateTime Date)
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid DoctorId { get; init; } = ValidateDoctorId(DoctorId);
    public Guid PatientId { get; init; } = ValidatePatientId(PatientId);
    public Guid AvailableTimeId { get; init; } = ValidateAvailableTimeId(AvailableTimeId);
    public DateTime Date { get; init; } = ValidateDate(Date);

    private static Guid ValidateDoctorId(Guid doctorId)
    {
        if (doctorId == Guid.Empty)
        {
            throw new DomainException("DoctorId cannot be an empty GUID");
        }
        return doctorId;
    }

    private static Guid ValidatePatientId(Guid patientId)
    {
        if (patientId == Guid.Empty)
        {
            throw new DomainException("PatientId cannot be an empty GUID");
        }
        return patientId;
    }

    private static Guid ValidateAvailableTimeId(Guid availableTimeId)
    {
        if (availableTimeId == Guid.Empty)
        {
            throw new DomainException("AvailableTimeId cannot be an empty GUID");
        }
        return availableTimeId;
    }

    private static DateTime ValidateDate(DateTime date)
    {
        if (date == default)
        {
            throw new DomainException("Date cannot be the default value");
        }
        return date;
    }
}
