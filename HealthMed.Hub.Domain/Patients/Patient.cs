using HealthMed.Hub.Domain.Base;

namespace HealthMed.Hub.Domain.Patients;

public record Patient(string Name, string Document, string Email, Guid? Id = null)
{
    public Guid? Id { get; init; } = Id ?? Guid.NewGuid();

    public string Name { get; init; } = string.IsNullOrEmpty(Name) ?
                                    throw new DomainException("Name is required") : Name;

    public string Document { get; init; } = DocumentValidator.CpfValidation.Validate(Document) ?
                                            Document : throw new DomainException("Document is invalid");

    public string Email { get; init; } = string.IsNullOrEmpty(Email) ?
                                        throw new DomainException("Email is required") : Email;
}
