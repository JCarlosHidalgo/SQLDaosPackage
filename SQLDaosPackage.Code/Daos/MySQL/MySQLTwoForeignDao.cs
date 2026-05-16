using System.Text;
using System.Data;

using MySql.Data.MySqlClient;

namespace SQLDaosPackage.Daos.MySQL;

public abstract class MySQLTwoForeignDao<T> : MySQLBaseDao<T>, ITwoForeignDao<T>
{
    //! String to identify first table's indentifier.
     /*!
        Use inherited class' constructor to assign attribute's value.
     */
    protected internal string _firstForeignKey = string.Empty;

    //! String to identify second table's indentifier.
     /*!
        Use inherited class' constructor to assign attribute's value.
     */
    protected internal string _secondForeignKey = string.Empty;

    // Implementation to Read() method from ITwoForeignDao interface.
    public T? Read(Guid id1, Guid id2)
    {
        MySQLRetryPolicy.EnsureOpen(_connection);
        T? entity = default(T);
        _sb = new StringBuilder();
        _sb.Append("SELECT * FROM ").Append(_tableName)
            .Append(" WHERE ").Append(_firstForeignKey).Append(" = '").Append(id1.ToString()).Append("' ")
            .Append(" AND ").Append(_secondForeignKey).Append(" = '").Append(id2.ToString()).Append("';");
        _mySqlReader = GetCommandByText(_sb).ExecuteReader();
        if (_mySqlReader.Read())
        {
            entity = MapReaderToEntity();
        }
        _mySqlReader.Close();
        return entity;
    }

    // Implementation to Delete() method from ITwoForeignDao interface.
    public bool Delete(Guid id1, Guid id2)
    {
        MySQLRetryPolicy.EnsureOpen(_connection);
        _sb = new StringBuilder();
        _sb.Append("DELETE FROM ").Append(_tableName)
            .Append(" WHERE ").Append(_firstForeignKey).Append(" = '").Append(id1.ToString()).Append("' ")
            .Append(" AND ").Append(_secondForeignKey).Append(" = '").Append(id2.ToString()).Append("';");
        _mySqlReader = GetCommandByText(_sb).ExecuteReader();
        int recordsAffected = _mySqlReader.RecordsAffected;
        _mySqlReader.Close();

        return recordsAffected > 0;
    }

    // Implementation to ReadAsync() method from ITwoForeignDao interface.
    public async Task<T?> ReadAsync(Guid id1, Guid id2)
    {
        _sb = new StringBuilder();
        _sb.Append("SELECT * FROM ").Append(_tableName)
            .Append(" WHERE ").Append(_firstForeignKey).Append(" = '").Append(id1.ToString()).Append("' ")
            .Append(" AND ").Append(_secondForeignKey).Append(" = '").Append(id2.ToString()).Append("';");
        return await MySQLRetryPolicy.ExecuteAsync(_connection, async () =>
        {
            T? entity = default(T);
            _mySqlReader = (MySqlDataReader)await GetCommandByText(_sb).ExecuteReaderAsync();
            if (await _mySqlReader.ReadAsync())
            {
                entity = MapReaderToEntity();
            }
            await _mySqlReader.CloseAsync();
            return entity;
        });
    }

    // Implementation to DeleteAsync() method from ITwoForeignDao interface.
    public async Task<bool> DeleteAsync(Guid id1, Guid id2)
    {
        _sb = new StringBuilder();
        _sb.Append("DELETE FROM ").Append(_tableName)
            .Append(" WHERE ").Append(_firstForeignKey).Append(" = '").Append(id1.ToString()).Append("' ")
            .Append(" AND ").Append(_secondForeignKey).Append(" = '").Append(id2.ToString()).Append("';");
        return await MySQLRetryPolicy.ExecuteAsync(_connection, async () =>
        {
            _mySqlReader = (MySqlDataReader)await GetCommandByText(_sb).ExecuteReaderAsync();
            int recordsAffected = _mySqlReader.RecordsAffected;
            await _mySqlReader.CloseAsync();

            return recordsAffected > 0;
        });
    }
}
