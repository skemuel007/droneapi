namespace Application.Contracts.Infrastructure;

public interface IDroneBatteryCheckerService
{
    Task CheckDroneBatteryLevels();
}