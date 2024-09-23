using HealthMed.Hub.Domain.Base;

namespace HealthMed.Hub.Domain.Doctors.UseCases.RegisterDoctor;

public record RegisterDoctorInput(string Name, string Crm, string Document, string Email);
