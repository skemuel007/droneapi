using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IMedicationRepository : IRepository<Medication>
{
    Task<IEnumerable<Medication>> GetLoadedItems(Guid droneId);
}