using Application.Contracts.Persistence;
using Domain.Entities;

namespace Persistence.Repositories;

public class DroneRepository : Repository<Drone>, IDronesRepository
{
    public DroneRepository(DronesAppContext dbContext) : base(dbContext)
    {
        
    }
}