using System.Linq.Expressions;

namespace Application.Abstraction;

public interface IRepository<TEntity> where TEntity : class, new()
{
    Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByPropertyAsync<TProperty>(
        Expression<Func<TEntity, TProperty>> propertySelector,
        TProperty value,
        CancellationToken cancellationToken = default
    );
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}
