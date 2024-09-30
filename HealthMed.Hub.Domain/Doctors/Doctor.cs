using HealthMed.Hub.Domain.Base;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace HealthMed.Hub.Domain.Doctors;

public record Doctor(string Name, string Crm, string Document, string Email, Guid? Id = null)
{
    public Guid? Id { get; init; } = Id ?? Guid.NewGuid();

    public string Name { get; init; } = string.IsNullOrEmpty(Name) ?
                                          throw new DomainException("Name is required") : Name;

    public string Crm { get; init; } = string.IsNullOrEmpty(Crm) ?
                                         throw new DomainException("Crm is required") : Crm;

    public string Document { get; init; } = DocumentValidator.CpfValidation.Validate(Document) ?
                                             Document : throw new DomainException("Document is invalid");

    public string Email { get; init; } = string.IsNullOrEmpty(Email) ?
                                          throw new DomainException("Email is required") : Email;
}