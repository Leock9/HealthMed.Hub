using System;
using Xunit;
using FluentAssertions;
using HealthMed.Hub.Domain.Doctors;
using Bogus;
using Bogus.Extensions.Brazil;
using HealthMed.Hub.Domain.Base;

namespace HealthMed.Hub.Domain.Tests.Doctors;

public class DoctorTests
{
    private readonly Faker _faker;

    public DoctorTests()
    {
        _faker = new Faker("pt_BR");
    }

    [Fact]
    public void Should_Create_Doctor_With_Valid_Data()
    {
        // Arrange
        var name = _faker.Person.FullName;
        var crm = _faker.Random.String2(10);
        var document = _faker.Person.Cpf();
        var email = _faker.Person.Email;

        // Act
        var doctor = new Doctor(name, crm, document, email);

        // Assert
        doctor.Should().NotBeNull();
        doctor.Name.Should().Be(name);
        doctor.Crm.Should().Be(crm);
        doctor.Document.Should().Be(document);
        doctor.Email.Should().Be(email);
    }

    [Fact]
    public void Should_Throw_Exception_When_Name_Is_Empty()
    {
        // Arrange
        var crm = _faker.Random.String2(10);
        var document = _faker.Person.Cpf();
        var email = _faker.Person.Email;

        // Act
        Action act = () => new Doctor(string.Empty, crm, document, email);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Name is required");
    }

    [Fact]
    public void Should_Throw_Exception_When_Crm_Is_Empty()
    {
        // Arrange
        var name = _faker.Person.FullName;
        var document = _faker.Person.Cpf();
        var email = _faker.Person.Email;

        // Act
        Action act = () => new Doctor(name, string.Empty, document, email);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Crm is required");
    }

    [Fact]
    public void Should_Throw_Exception_When_Document_Is_Invalid()
    {
        // Arrange
        var name = _faker.Person.FullName;
        var crm = _faker.Random.String2(10);
        var invalidDocument = "12345678900"; // Invalid CPF
        var email = _faker.Person.Email;

        // Act
        Action act = () => new Doctor(name, crm, invalidDocument, email);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Document is invalid");
    }

    [Fact]
    public void Should_Throw_Exception_When_Email_Is_Empty()
    {
        // Arrange
        var name = _faker.Person.FullName;
        var crm = _faker.Random.String2(10);
        var document = _faker.Person.Cpf();

        // Act
        Action act = () => new Doctor(name, crm, document, string.Empty);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Email is required");
    }
}
