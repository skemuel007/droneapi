using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IDronePayloadRepository : IRepository<DronePayload>
{
    Task<IEnumerable<DronePayload>> GetDronePayLoad(Expression<Func<DronePayload, bool>> predicate,
        bool disableTracking = true);
}