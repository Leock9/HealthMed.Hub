using Bogus;
using Bogus.Extensions.Brazil;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using HealthMed.Hub.Domain.Base;
using HealthMed.Hub.Domain.Patients;
using HealthMed.Hub.Domain.Patients.Gateways;
using HealthMed.Hub.Domain.Patients.UseCases.RegisterPatient;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using ValidationException = FluentValidation.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace HealthMed.Hub.Domain.Tests.Patients.UseCases;

public class RegisterPatientUseCaseTests
{
    private readonly Faker _faker;

    public RegisterPatientUseCaseTests()
    {
        _faker = new Faker("pt_BR");
    }

    private RegisterPatientInput GetTestRequest()
    {
        return new RegisterPatientInput(
            _faker.Person.FullName,
            _faker.Person.Cpf(),
            _faker.Person.Email
        );
    }

    private RegisterPatientUseCase GetUseCase()
    {
        return new RegisterPatientUseCase(
            A.Fake<IValidator<RegisterPatientInput>>(),
            A.Fake<ILogger<RegisterPatientUseCase>>(),
            A.Fake<IPatientGateway>()
        );
    }

    [Fact]
    public async Task Should_Register_Patient_When_Input_Is_Valid()
    {
        // Arrange
        var useCase = GetUseCase();
        var input = GetTestRequest();

        var validator = A.Fake<IValidator<RegisterPatientInput>>();
        var logger = A.Fake<ILogger<RegisterPatientUseCase>>();
        var patientGateway = A.Fake<IPatientGateway>();

        A.CallTo(() => validator.Validate(A<IValidationContext>.Ignored))
            .Returns(new ValidationResult());

        A.CallTo(() => patientGateway.GetByDocumentAsync(input.Document))
            .Returns(Task.FromResult<Patient>(null));

        A.CallTo(() => patientGateway.CreateAsync(A<Patient>.Ignored))
            .Returns(Task.FromResult(new Patient(input.Name, input.Document, input.Email)));

        var useCaseWithMocks = new RegisterPatientUseCase(validator, logger, patientGateway);

        // Act
        var result = await useCaseWithMocks.HandleAsync(input, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();

        A.CallTo(() => patientGateway.GetByDocumentAsync(input.Document)).MustHaveHappenedOnceExactly();
        A.CallTo(() => patientGateway.CreateAsync(A<Patient>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Document_Already_Exists()
    {
        // Arrange
        var input = GetTestRequest();
        var existingPatient = new Patient(input.Name, input.Document, input.Email);

        var validator = A.Fake<IValidator<RegisterPatientInput>>();
        var logger = A.Fake<ILogger<RegisterPatientUseCase>>();
        var patientGateway = A.Fake<IPatientGateway>();

        A.CallTo(() => validator.Validate(A<IValidationContext>.Ignored))
            .Returns(new ValidationResult());

        A.CallTo(() => patientGateway.GetByDocumentAsync(input.Document))
            .Returns(Task.FromResult(existingPatient));

        var useCase = new RegisterPatientUseCase(validator, logger, patientGateway);

        // Act
        Func<Task> act = async () => await useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
