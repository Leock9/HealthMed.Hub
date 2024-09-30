using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using HealthMed.Hub.Domain.AvailableTimes;
using HealthMed.Hub.Domain.AvailableTimes.Gateways;
using HealthMed.Hub.Domain.AvailableTimes.UseCases.UpdateAvailableTime;
using Microsoft.Extensions.Logging;
using Xunit;

namespace HealthMed.Hub.Domain.Tests.AvailableTimes.UseCases
{
    public class UpdateAvailableTimeUseCaseTests
    {
        private readonly IValidator<UpdateAvailableTimeInput> _validator;
        private readonly ILogger<UpdateAvailableTimeUseCase> _logger;
        private readonly IAvailableTimeGateway _availableTimeGateway;
        private readonly UpdateAvailableTimeUseCase _useCase;

        public UpdateAvailableTimeUseCaseTests()
        {
            _validator = A.Fake<IValidator<UpdateAvailableTimeInput>>();
            _logger = A.Fake<ILogger<UpdateAvailableTimeUseCase>>();
            _availableTimeGateway = A.Fake<IAvailableTimeGateway>();
            _useCase = new UpdateAvailableTimeUseCase(_validator, _logger, _availableTimeGateway);
        }

        [Fact]
        public async Task Should_Update_AvailableTime_When_Input_Is_Valid()
        {
            // Arrange
            var input = new UpdateAvailableTimeInput(
                Guid.NewGuid(),
                new TimeOnly(9, 0),
                new TimeOnly(17, 0),
                Guid.NewGuid(),
                DayOfWeek.Monday
            );

            var existingAvailableTime = new AvaliableTime(input.StartTime, input.EndTime, input.DoctorId, input.DayOfWeek)
            {
                Id = input.Id,
                IsOccupied = false
            };

            A.CallTo(() => _validator.Validate(A<UpdateAvailableTimeInput>.Ignored))
                .Returns(new ValidationResult());

            A.CallTo(() => _availableTimeGateway.GetByIdAsync(input.Id))
                .Returns(Task.FromResult(existingAvailableTime));

            // Act
            var result = await _useCase.HandleAsync(input, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(input.Id);

            A.CallTo(() => _availableTimeGateway.UpdateAsync(A<AvaliableTime>.That.Matches(at =>
                at.Id == input.Id &&
                at.StartTime == input.StartTime &&
                at.EndTime == input.EndTime &&
                at.DoctorId == input.DoctorId &&
                at.DayOfWeek == input.DayOfWeek
            ))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Should_Throw_ValidationException_When_AvailableTime_Not_Found()
        {
            // Arrange
            var input = new UpdateAvailableTimeInput(
                Guid.NewGuid(),
                new TimeOnly(9, 0),
                new TimeOnly(17, 0),
                Guid.NewGuid(),
                DayOfWeek.Monday
            );

            A.CallTo(() => _validator.Validate(A<UpdateAvailableTimeInput>.Ignored))
                .Returns(new ValidationResult());

            A.CallTo(() => _availableTimeGateway.GetByIdAsync(input.Id))
                .Returns(Task.FromResult<AvaliableTime>(null));

            // Act
            Func<Task> act = async () => await _useCase.HandleAsync(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage("Available time not found");
        }

        [Fact]
        public async Task Should_Throw_ValidationException_When_AvailableTime_Is_Occupied()
        {
            // Arrange
            var input = new UpdateAvailableTimeInput(
                Guid.NewGuid(),
                new TimeOnly(9, 0),
                new TimeOnly(17, 0),
                Guid.NewGuid(),
                DayOfWeek.Monday
            );

            var existingAvailableTime = new AvaliableTime(input.StartTime, input.EndTime, input.DoctorId, input.DayOfWeek)
            {
                Id = input.Id,
                IsOccupied = true
            };

            A.CallTo(() => _validator.Validate(A<UpdateAvailableTimeInput>.Ignored))
                .Returns(new ValidationResult());

            A.CallTo(() => _availableTimeGateway.GetByIdAsync(input.Id))
                .Returns(Task.FromResult(existingAvailableTime));

            // Act
            Func<Task> act = async () => await _useCase.HandleAsync(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage("Available time is occupied");
        }

        [Fact]
        public async Task Should_Throw_ValidationException_When_Input_Is_Invalid()
        {
            // Arrange
            var input = new UpdateAvailableTimeInput(
                Guid.NewGuid(),
                new TimeOnly(9, 0),
                new TimeOnly(17, 0),
                Guid.NewGuid(),
                DayOfWeek.Monday
            );

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("StartTime", "StartTime is required"),
                new ValidationFailure("EndTime", "EndTime is required")
            };

            A.CallTo(() => _validator.Validate(A<UpdateAvailableTimeInput>.Ignored))
                .Returns(new ValidationResult(validationFailures));

            // Act
            Func<Task> act = async () => await _useCase.HandleAsync(input, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>().WithMessage("Validation exception");
        }
    }
}
