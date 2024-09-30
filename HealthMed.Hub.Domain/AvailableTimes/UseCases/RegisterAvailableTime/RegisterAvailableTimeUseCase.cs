using FluentValidation;
using HealthMed.Hub.Domain.AvailableTimes.Gateways;
using HealthMed.Hub.Domain.Base;
using HealthMed.Hub.Domain.Base.UseCases;
using Microsoft.Extensions.Logging;

namespace HealthMed.Hub.Domain.AvailableTimes.UseCases.RegisterAvailableTime;
public interface IRegisterAvailableTimeUseCase : IUseCase<RegisterAvailableTimeInput, RegisterAvailableTimeOutput> {}

public class RegisterAvailableTimeUseCase : BaseUseCase<RegisterAvailableTimeInput>, IRegisterAvailableTimeUseCase
{
    private readonly ILogger<RegisterAvailableTimeUseCase> _logger;
    public IAvailableTimeGateway _availableTimeGateway;

    public RegisterAvailableTimeUseCase
    (
        IValidator<RegisterAvailableTimeInput> validator,
        ILogger<RegisterAvailableTimeUseCase> logger, 
        IAvailableTimeGateway availableTimeGateway
    ) : base(validator, logger)
    {
        _logger = logger;
        _availableTimeGateway = availableTimeGateway;
    }

    public async Task<RegisterAvailableTimeOutput> HandleAsync(RegisterAvailableTimeInput input, CancellationToken cancellationToken)
    {
        try
        {
            Validate(input);
            var avaliableTimes = await _availableTimeGateway.GetByDoctorIdAsync(input.DoctorId);
            var newAvailableTime = new AvaliableTime(input.StartTime, input.EndTime, input.DoctorId, input.DayOfWeek);

            Parallel.ForEach(avaliableTimes, avaliableTime =>
            {
                if(!avaliableTime.IsSameAvaliableTime(newAvailableTime));
                    throw new ValidationException("Doctor already has an available time in this period");
            });

            await _availableTimeGateway.CreateAsync(newAvailableTime);

            return new RegisterAvailableTimeOutput(newAvailableTime.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}