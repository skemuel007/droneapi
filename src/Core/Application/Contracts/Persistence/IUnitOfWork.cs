namespace Application.Contracts.Persistence;

public interface IUnitOfWork : IDisposable
{
    Task<int> CompleteAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}