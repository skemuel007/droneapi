using Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private bool _disposedValues = false;
    private IDbContextTransaction _transaction;
    protected readonly DronesAppContext _context;

    public UnitOfWork(DronesAppContext context)
    {
        _context = context;
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        await _transaction.RollbackAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValues)
        {
            if (disposing && _context != null)
            {
                _context.Dispose();
            }

            _disposedValues = true;
        }
    }

    ~UnitOfWork()
    {
        Dispose(disposing: false);
    }
    
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}