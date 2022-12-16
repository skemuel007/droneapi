using Application.Contracts.Persistence;
using Domain.Entities;

namespace Persistence.Repositories;

public class DronePayloadRepository : Repository<DronePayload>, IDronePayloadRepository
{
    public DronePayloadRepository(DronesAppContext dbContext) : base(dbContext)
    {
        
    }
}