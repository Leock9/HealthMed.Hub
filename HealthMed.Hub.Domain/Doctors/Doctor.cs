using HealthMed.Hub.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMed.Hub.Domain.Doctors;

public record Doctor(string Name, string Crm, string Document, string Email)
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; init; } = string.IsNullOrEmpty(Name) ?
                                    throw new DomainException("Name is required") : Name;

    public string Crm { get; init; } = string.IsNullOrEmpty(Crm) ?
                                    throw new DomainException("Crm is required") : Crm;

    public string Document { get; init; } = DocumentValidator.CpfValidation.Validate(Document) ?
                                            Document : throw new DomainException("Document is invalid");

    public string Email { get; init; } = string.IsNullOrEmpty(Email) ?
                                        throw new DomainException("Email is required") : Email; 
}
