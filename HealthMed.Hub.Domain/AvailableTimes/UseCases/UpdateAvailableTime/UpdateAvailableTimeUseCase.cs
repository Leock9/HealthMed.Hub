using FluentValidation;
using HealthMed.Hub.Domain.AvailableTimes.Gateways;
using HealthMed.Hub.Domain.Base.UseCases;
using Microsoft.Extensions.Logging;

namespace HealthMed.Hub.Domain.AvailableTimes.UseCases.UpdateAvailableTime;

public interface IUpdateAvailableTimeUseCase : IUseCase<UpdateAvailableTimeInput, UpdateAvailableTimeOutput> { }

public class UpdateAvailableTimeUseCase : BaseUseCase<UpdateAvailableTimeInput>, IUpdateAvailableTimeUseCase
{
    private readonly ILogger<UpdateAvailableTimeUseCase> _logger;
    private readonly IAvailableTimeGateway _availableTimeGateway;

    public UpdateAvailableTimeUseCase
    (
        IValidator<UpdateAvailableTimeInput> validator,
        ILogger<UpdateAvailableTimeUseCase> logger,
        IAvailableTimeGateway availableTimeGateway
    ) : base(validator, logger)
    {
        _logger = logger;
        _availableTimeGateway = availableTimeGateway;
    }

    public async Task<UpdateAvailableTimeOutput> HandleAsync(UpdateAvailableTimeInput input, CancellationToken cancellationToken)
    {
        try
        {
            Validate(input);

            var existingAvailableTime = await _availableTimeGateway.GetByIdAsync(input.Id) ?? 
                                                   throw new ValidationException("Available time not found");
            
            if (existingAvailableTime.IsOccupied)
                throw new ValidationException("Available time is occupied");

            var updatedAvailableTime = new AvaliableTime(input.StartTime, input.EndTime, input.DoctorId, input.DayOfWeek)
            {
                Id = input.Id
            };

            await _availableTimeGateway.UpdateAsync(updatedAvailableTime);

            return new UpdateAvailableTimeOutput(updatedAvailableTime.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
