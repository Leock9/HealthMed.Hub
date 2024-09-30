using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using HealthMed.Hub.Domain.AvailableTimes;
using HealthMed.Hub.Domain.AvailableTimes.Gateways;
using HealthMed.Hub.Domain.AvailableTimes.UseCases.GetAvailableTimes;
using Microsoft.Extensions.Logging;
using Xunit;

namespace HealthMed.Hub.Domain.Tests.AvailableTimes.UseCases;

public class GetAvailableTimesUseCaseTests
{
    private readonly IValidator<GetAvailableTimesInput> _validator;
    private readonly ILogger<GetAvailableTimesUseCase> _logger;
    private readonly IAvailableTimeGateway _availableTimeGateway;
    private readonly GetAvailableTimesUseCase _useCase;

    public GetAvailableTimesUseCaseTests()
    {
        _validator = A.Fake<IValidator<GetAvailableTimesInput>>();
        _logger = A.Fake<ILogger<GetAvailableTimesUseCase>>();
        _availableTimeGateway = A.Fake<IAvailableTimeGateway>();
        _useCase = new GetAvailableTimesUseCase(_validator, _logger, _availableTimeGateway);
    }

    [Fact]
    public async Task Should_Return_AvailableTimes_When_Input_Is_Valid()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var input = new GetAvailableTimesInput(doctorId);

        var availableTimes = new List<AvaliableTime>
        {
            new AvaliableTime(new TimeOnly(9, 0), new TimeOnly(17, 0), doctorId, DayOfWeek.Monday) { Id = Guid.NewGuid(), IsOccupied = false },
            new AvaliableTime(new TimeOnly(10, 0), new TimeOnly(18, 0), doctorId, DayOfWeek.Tuesday) { Id = Guid.NewGuid(), IsOccupied = true }
        };

        A.CallTo(() => _validator.Validate(A<GetAvailableTimesInput>.Ignored))
            .Returns(new ValidationResult());

        A.CallTo(() => _availableTimeGateway.GetByDoctorIdAsync(doctorId))
            .Returns(Task.FromResult((IEnumerable<AvaliableTime>)availableTimes));

        // Act
        var result = await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AvailableTimes.Should().HaveCount(2);
        result.AvailableTimes.First().StartTime.Should().Be(new TimeOnly(9, 0));
        result.AvailableTimes.Last().IsOccupied.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Throw_ValidationException_When_Input_Is_Invalid()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var input = new GetAvailableTimesInput(doctorId);

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("DoctorId", "DoctorId is required")
        };

        A.CallTo(() => _validator.Validate(A<GetAvailableTimesInput>.Ignored))
            .Returns(new ValidationResult(validationFailures));

        // Act
        Func<Task> act = async () => await _useCase.HandleAsync(input, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>().WithMessage("Validation exception");
    }
}
