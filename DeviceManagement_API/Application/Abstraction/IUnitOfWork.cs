namespace Application.Abstraction;

public interface IUnitOfWork : IDisposable
{
    public Task BeginTransaction();
    public Task TransactionRollback();
    public Task SaveChangesAsync( CancellationToken cancellationToken = default);
    public IRepository<TEntity> Repository<TEntity>() where TEntity : class, new();
    public void Dispose();
}
