using System.Text.Json;
using Application.Contracts.Infrastructure;
using Application.Contracts.Persistence;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Nest;
using Persistence.Implementation.Audit;

namespace Persistence.Implementation.Drone;

public class DroneBatteryCheckerService : IDroneBatteryCheckerService
{
    private readonly IDronesRepository _dronesRepository;
    private readonly ICacheService _cacheService;
    private readonly IAuditTrailProvider<AuditTrailLog> _auditTrailProvider;
    private readonly ILogger<DroneBatteryCheckerService> _logger;

    public DroneBatteryCheckerService(IDronesRepository dronesRepository,
        IAuditTrailProvider<AuditTrailLog> auditTrailProvider,
        ICacheService cacheService,
        ILogger<DroneBatteryCheckerService> logger)
    {
        _dronesRepository = dronesRepository ?? throw new ArgumentNullException(nameof(dronesRepository));
        _auditTrailProvider = auditTrailProvider;
        _cacheService =
            cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task CheckDroneBatteryLevels()
    {
        IReadOnlyList<Domain.Entities.Drone> drones = default;

        var droneCachedData = await _cacheService.GetData<IReadOnlyList<Domain.Entities.Drone>>("drones");

        if (droneCachedData != null)
            drones = droneCachedData;
        else
        {
            drones = await _dronesRepository.GetAllAsync();
            await _cacheService.SetData<IReadOnlyList<Domain.Entities.Drone>>("drones", drones);
        }

        _logger.LogInformation($"Successfully logged audit {JsonSerializer.Serialize(drones)}");
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