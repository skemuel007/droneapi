using Application.Contracts.Persistence;
using Application.Responses;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class DroneRequestRepository : Repository<DroneRequest>, IDroneRequestRepository
{
    public DroneRequestRepository(DronesAppContext dbContext) : base(dbContext)
    {
        
    }
}