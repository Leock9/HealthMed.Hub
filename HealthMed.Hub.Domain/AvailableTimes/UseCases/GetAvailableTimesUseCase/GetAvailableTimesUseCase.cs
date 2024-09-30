using FluentValidation;
using HealthMed.Hub.Domain.AvailableTimes.Gateways;
using HealthMed.Hub.Domain.Base.UseCases;
using Microsoft.Extensions.Logging;

namespace HealthMed.Hub.Domain.AvailableTimes.UseCases.GetAvailableTimes;

public interface IGetAvailableTimesUseCase : IUseCase<GetAvailableTimesInput, GetAvailableTimesOutput> { };
public class GetAvailableTimesUseCase : BaseUseCase<GetAvailableTimesInput>, IGetAvailableTimesUseCase
{
    private readonly ILogger<GetAvailableTimesUseCase> _logger;
    private readonly IAvailableTimeGateway _availableTimeGateway;

    public GetAvailableTimesUseCase
    (
        IValidator<GetAvailableTimesInput> validator,
        ILogger<GetAvailableTimesUseCase> logger,
        IAvailableTimeGateway availableTimeGateway
    ) : base(validator, logger)
    {
        _logger = logger;
        _availableTimeGateway = availableTimeGateway;
    }

    public async Task<GetAvailableTimesOutput> HandleAsync(GetAvailableTimesInput input, CancellationToken cancellationToken)
    {
        try
        {
            Validate(input);

            var availableTimes = await _availableTimeGateway.GetByDoctorIdAsync(input.DoctorId);
            var availableTimeDtos = availableTimes.Select(at => new AvailableTimeDto
            (
                at.Id,
                at.StartTime,
                at.EndTime,
                at.DayOfWeek,
                at.IsOccupied
            ));

            return new GetAvailableTimesOutput(availableTimeDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
