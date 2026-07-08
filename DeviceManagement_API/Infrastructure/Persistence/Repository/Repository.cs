using Application.Abstraction;
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repository;

public class Repository<TEntity> : IRepository<TEntity> 
    where TEntity : class, new()
{
    private readonly SqlConnection _connection;
    private readonly SqlTransaction? _transaction;
    private readonly string _tableName;

    public Repository(SqlConnection connection, SqlTransaction? transaction)
    {
        _connection = connection;
        _transaction = transaction;
        _tableName = typeof(TEntity).Name + "s";
    }

    public async Task<IList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var cmd = new SqlCommand($"SELECT * FROM {_tableName}", _connection, _transaction);
        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        var list = new List<TEntity>();
        while (await reader.ReadAsync(cancellationToken))
        {
            list.Add(Map(reader));
        }

        return list;
    }

    public async Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        var key = typeof(TEntity).GetProperties().First(p => p.Name.EndsWith("Id"));
        using var cmd = new SqlCommand(
            $"SELECT * FROM {_tableName} WHERE {key.Name} = @id",
            _connection,
            _transaction
        );

        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        return reader.Read() ? Map(reader) : null;
    }

    public async Task<TEntity?> GetByPropertyAsync<TProperty>(
        Expression<Func<TEntity, TProperty>> selector,
        TProperty value,
        CancellationToken cancellationToken = default)
    {
        var member = (selector.Body as MemberExpression)?.Member.Name;

        using var cmd = new SqlCommand(
            $"SELECT * FROM {_tableName} WHERE {member} = @val",
            _connection,
            _transaction
        );

        cmd.Parameters.AddWithValue("@val", value!);

        using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

        return reader.Read() ? Map(reader) : null;
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var props = typeof(TEntity)
            .GetProperties()
            .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string));

        var columns = string.Join(",", props.Select(p => p.Name));
        var values = string.Join(",", props.Select(p => "@" + p.Name));

        using var cmd = new SqlCommand(
            $"INSERT INTO {_tableName} ({columns}) VALUES ({values})",
            _connection,
            _transaction
        );

        foreach (var prop in props)
        {
            cmd.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(entity) ?? DBNull.Value);
        }

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var props = typeof(TEntity)
            .GetProperties()
            .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string));

        var key = props.First(p => p.Name.EndsWith("Id"));

        var setClause = string.Join(",",
            props.Where(p => p != key)
                 .Select(p => $"{p.Name}=@{p.Name}")
        );

        using var cmd = new SqlCommand(
            $"UPDATE {_tableName} SET {setClause} WHERE {key.Name}=@id",
            _connection,
            _transaction
        );

        foreach (var prop in props)
        {
            var value = prop.GetValue(entity) ?? DBNull.Value;
            cmd.Parameters.AddWithValue("@" + prop.Name, value);
        }

        cmd.Parameters.AddWithValue("@id", key.GetValue(entity)!);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var key = typeof(TEntity).GetProperties().First(p => p.Name.Equals("Id"));
        using var cmd = new SqlCommand(
            $"DELETE FROM {_tableName} WHERE {key.Name}=@id",
            _connection,
            _transaction
        );

        cmd.Parameters.AddWithValue("@id", key.GetValue(entity)!);
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private TEntity Map(SqlDataReader reader)
    {
        var entity = new TEntity();
        var props = typeof(TEntity)
            .GetProperties()
            .Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string));

        foreach (var prop in props)
        {
            if (!reader.HasColumn(prop.Name) || reader[prop.Name] is DBNull)
            {
                continue;
            }

            prop.SetValue(entity, reader[prop.Name]);
        }

        return entity;
    }
}

public static class RepositoryExtension
{
    public static bool HasColumn(this SqlDataReader reader, string columnName)
    {
        for (int i=0; i<reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName))
            {
                return true;
            }
        }
        return false;
    }
}
