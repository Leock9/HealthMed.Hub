using System;
using System.Collections.Generic;

namespace HealthMed.Hub.Domain.Doctors.UseCases.GetDoctors;

public record DoctorDto(Guid Id, string Name, string Crm, string Document, string Email);

public record GetDoctorsOutput(IEnumerable<DoctorDto> Doctors);
