using Bogus;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using HealthMed.Hub.Domain.Appointments;
using HealthMed.Hub.Domain.Appointments.Gateways;
using HealthMed.Hub.Domain.Appointments.UseCases.RegisterAppointment;
using Microsoft.Extensions.Logging;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace HealthMed.Hub.Domain.Tests.Appointments.UseCases;

public class RegisterAppointmentUseCaseTests
{
    private readonly IValidator<RegisterAppointmentInput> _validator;
    private readonly ILogger<RegisterAppointmentUseCase> _logger;
    private readonly IAppointmentsGateway _appointmentGateway;
    private readonly RegisterAppointmentUseCase _useCase;
    private readonly Faker<Appointment> _appointmentFaker;

    public RegisterAppointmentUseCaseTests()
    {
        _validator = A.Fake<IValidator<RegisterAppointmentInput>>();
        _logger = A.Fake<ILogger<RegisterAppointmentUseCase>>();
        _appointmentGateway = A.Fake<IAppointmentsGateway>();
        _useCase = new RegisterAppointmentUseCase(_validator, _logger, _appointmentGateway);

        _appointmentFaker = new Faker<Appointment>("pt_BR")
            .CustomInstantiator(f => new Appointment(
                f.Random.Guid(),
                f.Random.Guid(),
                f.Random.Guid(),
                f.Date.Future()
            ));
    }

    [Fact]
    public async Task Should_Register_Appointment_When_Input_Is_Valid_And_No_Conflicts()
    {
        // Arrange
        var input = new RegisterAppointmentInput
            (
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            );
        var appointments = new List<Appointment>();

        A.CallTo(() => _validator.Validate(A<RegisterAppointmentInput>.Ignored))
            .Returns(new ValidationResult());

        A.CallTo(() => _appointmentGateway.GetByDoctorIdAsync(input.DoctorId))
            .Returns(Task.FromResult((IEnumerable<Appointment>)appointments));

        A.CallTo(() => _appointmentGateway.CreateAsync(A<Appointment>.Ignored))
                .Returns(Task.FromResult(new Appointment(input.DoctorId, input.PatientId, input.AvailabeTimeId, DateTime.Now)));

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        A.CallTo(() => _appointmentGateway.CreateAsync(A<Appointment>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Should_Throw_ValidationException_When_Doctor_Has_Conflicting_Appointment()
    {
        // Arrange
        var input = new RegisterAppointmentInput
                   (
                       Guid.NewGuid(),
                       Guid.NewGuid(),
                       Guid.NewGuid()
                   );

        var existingAppointment = _appointmentFaker.Generate();
        var appointments = new List<Appointment> { existingAppointment };

        A.CallTo(() => _validator.Validate(A<RegisterAppointmentInput>.Ignored))
            .Returns(new ValidationResult());

        A.CallTo(() => _appointmentGateway.GetByDoctorIdAsync(input.DoctorId))
            .Returns(Task.FromResult((IEnumerable<Appointment>)appointments));

        // Act
        Func<Task> act = async () => await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>()
            .WithMessage("Doctor already has an appointment in this period");
    }
}
