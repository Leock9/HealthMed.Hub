using Bogus;
using Bogus.Extensions.Brazil;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using HealthMed.Hub.Domain.Doctors;
using HealthMed.Hub.Domain.Doctors.Gateways;
using HealthMed.Hub.Domain.Doctors.UseCases.GetDoctors;
using Microsoft.Extensions.Logging;

namespace HealthMed.Hub.Domain.Tests.Doctors.UseCases;

public class GetDoctorsUseCaseTests
{
    private readonly IValidator<GetDoctorsInput> _validator;
    private readonly ILogger<GetDoctorsUseCase> _logger;
    private readonly IDoctorGateway _doctorGateway;
    private readonly GetDoctorsUseCase _useCase;
    private readonly Faker<Doctor> _doctorFaker;

    public GetDoctorsUseCaseTests()
    {
        _validator = A.Fake<IValidator<GetDoctorsInput>>();
        _logger = A.Fake<ILogger<GetDoctorsUseCase>>();
        _doctorGateway = A.Fake<IDoctorGateway>();
        _useCase = new GetDoctorsUseCase(_validator, _logger, _doctorGateway);

        _doctorFaker = new Faker<Doctor>("pt_BR")
            .CustomInstantiator(f => new Doctor(
                f.Name.FullName(),
                f.Random.String2(6, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"),
                f.Person.Cpf(),
                f.Internet.Email()
            ));
    }

    [Fact]
    public async Task Should_Return_Doctors_When_Input_Is_Valid()
    {
        // Arrange
        var input = new GetDoctorsInput();

        var doctors = _doctorFaker.Generate(2);

        A.CallTo(() => _validator.Validate(A<GetDoctorsInput>.Ignored))
            .Returns(new FluentValidation.Results.ValidationResult());

        A.CallTo(() => _doctorGateway.GetAsync())
            .Returns(Task.FromResult((IEnumerable<Doctor>)doctors));

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Doctors.Should().HaveCount(2);
        result.Doctors.First().Name.Should().Be(doctors.First().Name);
        result.Doctors.Last().Crm.Should().Be(doctors.Last().Crm);
    }
}
