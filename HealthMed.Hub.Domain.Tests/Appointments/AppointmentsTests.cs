using System;
using Xunit;
using FluentAssertions;
using HealthMed.Hub.Domain.Appointments;
using HealthMed.Hub.Domain.Base;

namespace HealthMed.Hub.Domain.Tests.Appointments;

public class AppointmentTests
{
    [Fact]
    public void Should_Create_Appointment_With_Valid_Data()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var availableTimeId = Guid.NewGuid();
        var date = DateTime.Now;

        // Act
        var appointment = new Appointment(doctorId, patientId, availableTimeId, date);

        // Assert
        appointment.Should().NotBeNull();
        appointment.DoctorId.Should().Be(doctorId);
        appointment.PatientId.Should().Be(patientId);
        appointment.AvailableTimeId.Should().Be(availableTimeId);
        appointment.Date.Should().Be(date);
        appointment.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Should_Throw_Exception_When_DoctorId_Is_Empty()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var availableTimeId = Guid.NewGuid();
        var date = DateTime.Now;

        // Act
        Action act = () => new Appointment(Guid.Empty, patientId, availableTimeId, date);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("DoctorId cannot be an empty GUID");
    }

    [Fact]
    public void Should_Throw_Exception_When_PatientId_Is_Empty()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var availableTimeId = Guid.NewGuid();
        var date = DateTime.Now;

        // Act
        Action act = () => new Appointment(doctorId, Guid.Empty, availableTimeId, date);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("PatientId cannot be an empty GUID");
    }

    [Fact]
    public void Should_Throw_Exception_When_AvailableTimeId_Is_Empty()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var date = DateTime.Now;

        // Act
        Action act = () => new Appointment(doctorId, patientId, Guid.Empty, date);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("AvailableTimeId cannot be an empty GUID");
    }

    [Fact]
    public void Should_Throw_Exception_When_Date_Is_Default()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var availableTimeId = Guid.NewGuid();

        // Act
        Action act = () => new Appointment(doctorId, patientId, availableTimeId, default);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Date cannot be the default value");
    }
}

