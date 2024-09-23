using System;
using Xunit;
using FluentAssertions;
using HealthMed.Hub.Domain.Patients;
using Bogus;
using Bogus.Extensions.Brazil;
using HealthMed.Hub.Domain.Base;

namespace HealthMed.Hub.Domain.Tests.Patients;

public class PatientTests
{
    private readonly Faker _faker;

    public PatientTests()
    {
        _faker = new Faker("pt_BR");
    }

    [Fact]
    public void Should_Create_Patient_With_Valid_Data()
    {
        // Arrange
        var name = _faker.Person.FullName;
        var document = _faker.Person.Cpf();
        var email = _faker.Person.Email;

        // Act
        var patient = new Patient(name, document, email);

        // Assert
        patient.Should().NotBeNull();
        patient.Name.Should().Be(name);
        patient.Document.Should().Be(document);
        patient.Email.Should().Be(email);
        patient.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Should_Throw_Exception_When_Name_Is_Empty()
    {
        // Arrange
        var document = _faker.Person.Cpf();
        var email = _faker.Person.Email;

        // Act
        Action act = () => new Patient(string.Empty, document, email);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Name is required");
    }

    [Fact]
    public void Should_Throw_Exception_When_Document_Is_Invalid()
    {
        // Arrange
        var name = _faker.Person.FullName;
        var invalidDocument = "12345678900"; // Invalid CPF
        var email = _faker.Person.Email;

        // Act
        Action act = () => new Patient(name, invalidDocument, email);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Document is invalid");
    }

    [Fact]
    public void Should_Throw_Exception_When_Email_Is_Empty()
    {
        // Arrange
        var name = _faker.Person.FullName;
        var document = _faker.Person.Cpf();

        // Act
        Action act = () => new Patient(name, document, string.Empty);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Email is required");
    }
}
