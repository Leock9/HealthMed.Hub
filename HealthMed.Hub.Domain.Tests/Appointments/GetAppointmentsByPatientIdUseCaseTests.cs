using Bogus;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using HealthMed.Hub.Domain.Appointments;
using HealthMed.Hub.Domain.Appointments.Gateways;
using HealthMed.Hub.Domain.Appointments.UseCases.GetAppointmentByPatient;
using HealthMed.Hub.Domain.Appointments.UseCases.GetAppointmentsByPatientId;
using Microsoft.Extensions.Logging;
using ValidationException = FluentValidation.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace HealthMed.Hub.Domain.Tests.Appointments.UseCases;

public class GetAppointmentsByPatientIdUseCaseTests
{
    private readonly IValidator<GetAppointmentsByPatientIdInput> _validator;
    private readonly ILogger<GetAppointmentsByPatientIdUseCase> _logger;
    private readonly IAppointmentsGateway _appointmentGateway;
    private readonly GetAppointmentsByPatientIdUseCase _useCase;
    private readonly Faker<Appointment> _appointmentFaker;

    public GetAppointmentsByPatientIdUseCaseTests()
    {
        _validator = A.Fake<IValidator<GetAppointmentsByPatientIdInput>>();
        _logger = A.Fake<ILogger<GetAppointmentsByPatientIdUseCase>>();
        _appointmentGateway = A.Fake<IAppointmentsGateway>();
        _useCase = new GetAppointmentsByPatientIdUseCase(_validator, _logger, _appointmentGateway);

        _appointmentFaker = new Faker<Appointment>("pt_BR")
            .CustomInstantiator(f => new Appointment(
                f.Random.Guid(),
                f.Random.Guid(),
                f.Random.Guid(),
                f.Date.Future()
            ));
    }

    [Fact]
    public async Task Should_Return_Appointments_When_Input_Is_Valid()
    {
        // Arrange
        var input = new GetAppointmentsByPatientIdInput(Guid.NewGuid());
        var appointments = _appointmentFaker.Generate(3);

        A.CallTo(() => _validator.Validate(A<GetAppointmentsByPatientIdInput>.Ignored))
            .Returns(new ValidationResult());

        A.CallTo(() => _appointmentGateway.GetByPatientAsync(input.PatientId))
            .Returns(Task.FromResult((IEnumerable<Appointment>)appointments));

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Appointments.Should().HaveCount(3);
        result.Appointments.Should().BeEquivalentTo(appointments);
    }
}
