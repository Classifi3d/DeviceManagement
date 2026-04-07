using Application.Abstraction;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Persistence.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly SqlConnection _connection;
    private SqlTransaction? _transaction;

    public UnitOfWork(SqlConnection connection)
    {
        _connection = connection;
    }

    public async Task BeginTransaction()
    {
        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync();
        }

        _transaction = _connection.BeginTransaction();
    }
    public async Task TransactionRollback()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();
        _transaction = null;
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _transaction?.Commit();
        }
        catch
        {
            _transaction?.Rollback();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
        await Task.CompletedTask;
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : class, new()
    {
        return new Repository<TEntity>(_connection, _transaction);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _transaction = null;
        _connection?.Dispose();
    }
}