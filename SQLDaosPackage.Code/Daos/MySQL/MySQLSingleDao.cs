using System.Text;

using MySql.Data.MySqlClient;

using SQLDaosPackage.Entities;
using SQLDaosPackage.Entities.Attributes;

namespace SQLDaosPackage.Daos.MySQL;

/// <summary>
/// This class implements concrete functionality from \c ISingleDao interface.
/// </summary>
 /*!
    This Dao represents a MySQL table that does not have any foreign key relationships
    with other ones, it also works with tables that only have one foreign key
    relationship.
    \param T Is the entity over this interface provides its methods, constrained to
    \c IEntity to enforce the marker contract at compile time. The identifier
    property must be marked with \c [Identificator] and named \c Id (the hard-coded
    SQL still targets that column).
  */
public abstract class MySQLSingleDao<T> : MySQLBaseDao<T>, ISingleDao<T> where T : IEntity
{
    protected MySQLSingleDao()
    {
        string identificator = EntityKeyResolver.ResolvePropertyName<IdentificatorAttribute>(typeof(T));
        if (identificator != "Id")
        {
            throw new InvalidOperationException(
                $"Entity type '{typeof(T).FullName}' marks '{identificator}' with [Identificator], but MySQLSingleDao requires the identifier property to be named 'Id'.");
        }
    }

    // Implementation to Read() method from ISingleDao interface.
    public T? Read(Guid id)
    {
        MySQLRetryPolicy.EnsureOpen(_connection);
        T? entity = default(T);
        _sb = new StringBuilder();
        _sb.Append("SELECT * FROM ").Append(_tableName).Append(" WHERE Id = '").Append(id.ToString()).Append("';");
        MySqlCommand com = GetCommandByText(_sb);
        _mySqlReader = com.ExecuteReader();
        if (_mySqlReader.Read())
        {
            entity = MapReaderToEntity();
        }
        _mySqlReader.Close();
        return entity;
    }

    // Implementation to Delete() method from ISingleDao interface.
    public bool Delete(Guid id)
    {
        MySQLRetryPolicy.EnsureOpen(_connection);
        _sb = new StringBuilder();
        _sb.Append("DELETE FROM ").Append(_tableName).Append(" WHERE Id = '").Append(id.ToString()).Append("';");
        MySqlCommand com = GetCommandByText(_sb);
        _mySqlReader = com.ExecuteReader();
        int recordsAffected = _mySqlReader.RecordsAffected;
        _mySqlReader.Close();

        return recordsAffected > 0;
    }

    // Implementation to ReadAsync() method from ISingleDao interface.
    public async Task<T?> ReadAsync(Guid id)
    {
        _sb = new StringBuilder();
        _sb.Append("SELECT * FROM ").Append(_tableName).Append(" WHERE Id = '").Append(id.ToString()).Append("';");
        return await MySQLRetryPolicy.ExecuteAsync(_connection, async () =>
        {
            T? entity = default(T);
            MySqlCommand com = GetCommandByText(_sb);
            _mySqlReader = (MySqlDataReader)await com.ExecuteReaderAsync();
            if (await _mySqlReader.ReadAsync())
            {
                entity = MapReaderToEntity();
            }
            await _mySqlReader.CloseAsync();
            return entity;
        });
    }

    // Implementation to DeleteAsync() method from ISingleDao interface.
    public async Task<bool> DeleteAsync(Guid id)
    {
        _sb = new StringBuilder();
        _sb.Append("DELETE FROM ").Append(_tableName).Append(" WHERE Id = '").Append(id.ToString()).Append("';");
        return await MySQLRetryPolicy.ExecuteAsync(_connection, async () =>
        {
            MySqlCommand com = GetCommandByText(_sb);
            _mySqlReader = (MySqlDataReader)await com.ExecuteReaderAsync();
            int recordsAffected = _mySqlReader.RecordsAffected;
            await _mySqlReader.CloseAsync();

            return recordsAffected > 0;
        });
    }
}
