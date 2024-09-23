using System;
using Xunit;
using FluentAssertions;
using HealthMed.Hub.Domain.AvailableTimes;
using HealthMed.Hub.Domain.Base;

namespace HealthMed.Hub.Domain.Tests.AvailableTimes;

public class AvaliableTimeTests
{
    [Fact]
    public void Should_Create_AvaliableTime_With_Valid_Data()
    {
        // Arrange
        var startTime = new TimeOnly(9, 0);
        var endTime = new TimeOnly(17, 0);
        var doctorId = Guid.NewGuid();
        var dayOfWeek = DayOfWeek.Monday;

        // Act
        var avaliableTime = new AvaliableTime(startTime, endTime, doctorId, dayOfWeek);

        // Assert
        avaliableTime.Should().NotBeNull();
        avaliableTime.StartTime.Should().Be(startTime);
        avaliableTime.EndTime.Should().Be(endTime);
        avaliableTime.DoctorId.Should().Be(doctorId);
        avaliableTime.DayOfWeek.Should().Be(dayOfWeek);
        avaliableTime.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Should_Throw_Exception_When_StartTime_Is_After_EndTime()
    {
        // Arrange
        var startTime = new TimeOnly(18, 0);
        var endTime = new TimeOnly(17, 0);
        var doctorId = Guid.NewGuid();
        var dayOfWeek = DayOfWeek.Monday;

        // Act
        Action act = () => new AvaliableTime(startTime, endTime, doctorId, dayOfWeek);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("StartTime must be earlier than EndTime");
    }

    [Fact]
    public void Should_Throw_Exception_When_DoctorId_Is_Empty()
    {
        // Arrange
        var startTime = new TimeOnly(9, 0);
        var endTime = new TimeOnly(17, 0);
        var doctorId = Guid.Empty;
        var dayOfWeek = DayOfWeek.Monday;

        // Act
        Action act = () => new AvaliableTime(startTime, endTime, doctorId, dayOfWeek);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("DoctorId cannot be an empty GUID");
    }
}
