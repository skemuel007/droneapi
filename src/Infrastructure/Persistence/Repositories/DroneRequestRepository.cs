using Application.Contracts.Persistence;
using Domain.Entities;

namespace Persistence.Repositories;

public class DroneRequestRepository : Repository<DroneRequest>, IDroneRequestRepository
{
    public DroneRequestRepository(DronesAppContext dbContext) : base(dbContext)
    {
        
    }
}