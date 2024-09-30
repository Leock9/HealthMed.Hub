using Bogus;
using Bogus.Extensions.Brazil;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using HealthMed.Hub.Domain.Doctors;
using HealthMed.Hub.Domain.Doctors.Gateways;
using HealthMed.Hub.Domain.Doctors.UseCases.RegisterDoctor;
using Microsoft.Extensions.Logging;
using ValidationException = FluentValidation.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace HealthMed.Hub.Domain.Tests.Doctors.UseCases;

public class RegisterDoctorUseCaseTests
{
    private readonly Faker _faker;

    public RegisterDoctorUseCaseTests()
    {
        _faker = new Faker("pt_BR");
    }

    private RegisterDoctorInput GetTestRequest()
    {
        return new RegisterDoctorInput(
            _faker.Person.FullName,
            _faker.Random.String2(10),
            _faker.Person.Cpf(),
            _faker.Person.Email
        );
    }

    private RegisterDoctorUseCase GetUseCase()
    {
        return new RegisterDoctorUseCase(
            A.Fake<IValidator<RegisterDoctorInput>>(),
            A.Fake<ILogger<RegisterDoctorUseCase>>(),
            A.Fake<IDoctorGateway>()
        );
    }

    [Fact]
    public async Task Should_Register_Doctor_When_Input_Is_Valid()
    {
        // Arrange
        var useCase = GetUseCase();
        var input = GetTestRequest();

        var validator = A.Fake<IValidator<RegisterDoctorInput>>();
        var logger = A.Fake<ILogger<RegisterDoctorUseCase>>();
        var doctorGateway = A.Fake<IDoctorGateway>();

        A.CallTo(() => validator.Validate(A<IValidationContext>.Ignored))
            .Returns(new ValidationResult());

        A.CallTo(() => doctorGateway.GetByCrmAsync(input.Crm))
            .Returns(Task.FromResult<Doctor>(null));

        A.CallTo(() => doctorGateway.CreateAsync(A<Doctor>.Ignored))
            .Returns(Task.FromResult(new Doctor(input.Name, input.Crm, input.Document, input.Email)));

        var useCaseWithMocks = new RegisterDoctorUseCase(validator, logger, doctorGateway);

        // Act
        var result = await useCaseWithMocks.HandleAsync(input, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();

        A.CallTo(() => doctorGateway.GetByCrmAsync(input.Crm)).MustHaveHappenedOnceExactly();
        A.CallTo(() => doctorGateway.CreateAsync(A<Doctor>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Crm_Already_Exists()
    {
        // Arrange
        var input = GetTestRequest();
        var existingDoctor = new Doctor(input.Name, input.Crm, input.Document, input.Email);

        var validator = A.Fake<IValidator<RegisterDoctorInput>>();
        var logger = A.Fake<ILogger<RegisterDoctorUseCase>>();
        var doctorGateway = A.Fake<IDoctorGateway>();

        A.CallTo(() => validator.Validate(A<IValidationContext>.Ignored))
            .Returns(new ValidationResult());

        A.CallTo(() => doctorGateway.GetByCrmAsync(input.Crm))
            .Returns(Task.FromResult(existingDoctor));

        var useCase = new RegisterDoctorUseCase(validator, logger, doctorGateway);

        // Act
        Func<Task> act = async () => await useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
