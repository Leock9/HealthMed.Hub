using FluentValidation.Results;

namespace HealthMed.Hub.Domain.Base.UseCases;

public interface IUseCase<TInput, TOutput>
{
    void AddValidationFailures(List<ValidationFailure> validationFailures);
    Task<TOutput> HandleAsync(TInput input, CancellationToken cancellationToken);
}
