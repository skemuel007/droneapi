using Application.Contracts.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class MedicationRepository : Repository<Medication>, IMedicationRepository
{
    public MedicationRepository(DronesAppContext dbContext) : base(dbContext)
    {
        
    }

    public async Task<IEnumerable<Medication>> GetLoadedItems(Guid droneId)
    {
        IQueryable<DroneRequest> query = _dbContext.DroneRequests.AsQueryable();
        query = query.AsNoTracking();
        query = query.Where(d => d.DroneId == droneId);

        query = query.Include(d => d.DronePayloads).ThenInclude(dp => dp.Medication);

        var response = await query.Select(x => x.DronePayloads.Select(d => d.Medication)).ToListAsync();

        return response.Any() ? (IEnumerable<Medication>)response[0] : null;

    }
}