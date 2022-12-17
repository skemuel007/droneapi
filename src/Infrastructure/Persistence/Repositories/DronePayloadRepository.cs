using System.Linq.Expressions;
using Application.Contracts.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class DronePayloadRepository : Repository<DronePayload>, IDronePayloadRepository
{
    public DronePayloadRepository(DronesAppContext dbContext) : base(dbContext)
    {
        
    }

    public async Task<IEnumerable<DronePayload>> GetDronePayLoad(Expression<Func<DronePayload, bool>> predicate,
        bool disableTracking = true)
    {
        var queryable = _dbContext.DronePayloads.Where(predicate);
        
        if (disableTracking)
            queryable = queryable.AsNoTracking();
            
        var dronePayloads =  await queryable.Include(m => m.Medication)
            .Include(d => d.DroneRequest)
            .ThenInclude(d => d.Drone)
            .ToListAsync();
        
        return dronePayloads;
    }
}