using Bogus;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using HealthMed.Hub.Domain.AvailableTimes;
using HealthMed.Hub.Domain.AvailableTimes.Gateways;
using HealthMed.Hub.Domain.AvailableTimes.UseCases.RegisterAvailableTime;
using Microsoft.Extensions.Logging;
using ValidationException = FluentValidation.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace HealthMed.Hub.Domain.Tests.AvailableTimes.UseCases;

public class RegisterAvailableTimeUseCaseTests
{
    private readonly Faker _faker;

    public RegisterAvailableTimeUseCaseTests()
    {
        _faker = new Faker("pt_BR");
    }

    private RegisterAvailableTimeInput GetTestRequest()
    {
        var startTime = TimeOnly.FromDateTime(DateTime.Now.ToLocalTime());
        var endTime = TimeOnly.FromDateTime(DateTime.Now.ToLocalTime().AddHours(1));

        return new RegisterAvailableTimeInput(
            startTime,
            endTime,
            Guid.NewGuid(),
            _faker.PickRandom<DayOfWeek>()
        );
    }

    private RegisterAvailableTimeUseCase GetUseCase()
    {
        return new RegisterAvailableTimeUseCase(
            A.Fake<IValidator<RegisterAvailableTimeInput>>(),
            A.Fake<ILogger<RegisterAvailableTimeUseCase>>(),
            A.Fake<IAvailableTimeGateway>()
        );
    }

    [Fact]
    public async Task Should_Register_AvailableTime_When_Input_Is_Valid()
    {
        // Arrange
        var useCase = GetUseCase();
        var input = GetTestRequest();

        var validator = A.Fake<IValidator<RegisterAvailableTimeInput>>();
        var logger = A.Fake<ILogger<RegisterAvailableTimeUseCase>>();
        var availableTimeGateway = A.Fake<IAvailableTimeGateway>();

        A.CallTo(() => validator.Validate(A<IValidationContext>.Ignored))
            .Returns(new ValidationResult());

        A.CallTo(() => availableTimeGateway.GetByDoctorIdAsync(input.DoctorId))
            .Returns(Task.FromResult<IEnumerable<AvaliableTime>>(new List<AvaliableTime>()));

        A.CallTo(() => availableTimeGateway.CreateAsync(A<AvaliableTime>.Ignored))
            .Returns(Task.FromResult(new AvaliableTime(input.StartTime, input.EndTime, input.DoctorId, input.DayOfWeek)));

        var useCaseWithMocks = new RegisterAvailableTimeUseCase(validator, logger, availableTimeGateway);

        // Act
        var result = await useCaseWithMocks.HandleAsync(input, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();

        A.CallTo(() => availableTimeGateway.GetByDoctorIdAsync(input.DoctorId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => availableTimeGateway.CreateAsync(A<AvaliableTime>.Ignored)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Should_Throw_Exception_When_AvailableTime_Already_Exists()
    {
        // Arrange
        var input = GetTestRequest();
        var existingAvailableTime = new AvaliableTime(input.StartTime, input.EndTime, input.DoctorId, input.DayOfWeek);

        var validator = A.Fake<IValidator<RegisterAvailableTimeInput>>();
        var logger = A.Fake<ILogger<RegisterAvailableTimeUseCase>>();
        var availableTimeGateway = A.Fake<IAvailableTimeGateway>();

        A.CallTo(() => validator.Validate(A<IValidationContext>.Ignored))
            .Returns(new ValidationResult());

        A.CallTo(() => availableTimeGateway.GetByDoctorIdAsync(input.DoctorId))
            .Returns(Task.FromResult<IEnumerable<AvaliableTime>>(new List<AvaliableTime> { existingAvailableTime }));

        var useCase = new RegisterAvailableTimeUseCase(validator, logger, availableTimeGateway);

        // Act
        Func<Task> act = async () => await useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>().WithMessage("Doctor already has an available time in this period");
    }
}
