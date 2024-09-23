using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace HealthMed.Hub.Domain.Base.UseCases;

public abstract class BaseUseCase<TInput>
{
    protected readonly IValidator<TInput> _validator;
    private readonly IList<ValidationFailure> _validationFailures = new List<ValidationFailure>();

    protected BaseUseCase
    (
        IValidator<TInput> validator,
        ILogger logger
    )
    {
        _validator = validator;
        Log = logger;
    }

    public ILogger Log { get; }
    private bool IsValidated { get; set; }

    public IEnumerable<ValidationFailure> Validate(TInput input)
    {
        var failures = _validationFailures;
        if (!IsValidated) failures = _validator.Validate(input).Errors;

        if (failures.Any())
        {
            throw new ValidationException("Validation exception", failures);
        }

        return failures;
    }

    protected void ThrowError(string propertyName, string errorMessage)
    {
        AddError(propertyName, errorMessage);
        ThrowIfAnyErrors();
    }

    protected void ThrowError(string errorMessage)
    {
        ThrowError("generalErrors", errorMessage);
    }

    protected void AddError(string propertyName, string errorMessage)
    {
        _validationFailures.Add(new ValidationFailure(propertyName, errorMessage));
    }

    protected void AddError(string errorMessage)
    {
        AddError("generalErrors", errorMessage);
    }

    protected void ThrowIfAnyErrors()
    {
        if (_validationFailures.Any())
        {
            throw new ValidationException("Validation exception", _validationFailures);
        }
    }

    public void AddValidationFailures(List<ValidationFailure> validationFailures)
    {
        IsValidated = true;
        foreach (var validationFailure in _validationFailures)
        {
            AddError(validationFailure.PropertyName, validationFailure.ErrorMessage);
        }
    }
}