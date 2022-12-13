using Application.Contracts.Persistence;
using Domain.Entities;

namespace Persistence.Repositories;

public class MedicationRepository : Repository<Medication>, IMedicationRepository
{
    public MedicationRepository(DronesAppContext dbContext) : base(dbContext)
    {
        
    }
}