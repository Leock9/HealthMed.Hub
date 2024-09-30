using HealthMed.Hub.Domain.Base;

namespace HealthMed.Hub.Domain.Patients.UseCases.RegisterPatient;

public record RegisterPatientInput(string Name, string Document, string Email);
