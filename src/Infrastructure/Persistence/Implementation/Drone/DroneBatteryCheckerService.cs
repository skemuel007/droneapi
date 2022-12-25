using System.Text.Json;
using Application.Contracts.Infrastructure;
using Application.Contracts.Persistence;
using Persistence.Implementation.Audit;

namespace Persistence.Implementation.Drone;

public class DroneBatteryCheckerService : IDroneBatteryCheckerService
{
    private readonly IDronesRepository _dronesRepository;
    private readonly IAuditTrailProvider<AuditTrailLog> _auditTrailProvider;

    public DroneBatteryCheckerService(IDronesRepository dronesRepository,
        IAuditTrailProvider<AuditTrailLog> auditTrailProvider)
    {
        _dronesRepository = dronesRepository ?? throw new ArgumentNullException(nameof(dronesRepository));
        _auditTrailProvider = auditTrailProvider;
    }

    public async Task CheckDroneBatteryLevels()
    {
        var drones = await _dronesRepository.GetAllAsync();

        var auditTrailLog = new AuditTrailLog()
        {
            User = "Automated",
            Origin = nameof(CheckDroneBatteryLevels),
            Action = "Get Drone Battery Levels",
            Log = $"{JsonSerializer.Serialize(drones)}",
            Extra = "Nothing"
        };
        
        _auditTrailProvider.AddLog(auditTrailLog);
    }
}