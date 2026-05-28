using System.Text;

using MySql.Data.MySqlClient;

using SQLDaosPackage.Entities;
using SQLDaosPackage.Entities.Attributes;

namespace SQLDaosPackage.Daos.MySQL;

/// <summary>
/// This class implements concrete functionality from \c IThreeForeignDao interface.
/// </summary>
 /*!
    This Dao represents a MySQL table whose row identity is the triple of three
    foreign Guids. Column names for the three foreign keys are resolved from the
    entity properties marked with \c [FirstForeignId], \c [SecondForeignId] and
    \c [ThirdForeignId]; subclasses may still override the resolved values by
    reassigning the protected fields in their own constructor.
    \param T Is the entity over this interface provides its methods, constrained to
    \c IThreeForeignEntity to enforce the marker contract at compile time.
  */
public abstract class MySQLThreeForeignDao<T> : MySQLBaseDao<T>, IThreeForeignDao<T> where T : IThreeForeignEntity
{
    //! String to identify the first foreign-key column.
    /*!
       Defaults to the name of the property marked with \c [FirstForeignId] on \c T.
       Inherited classes can override the resolved value in their constructor.
    */
    protected internal string _firstForeignKey;

    //! String to identify the second foreign-key column.
    /*!
       Defaults to the name of the property marked with \c [SecondForeignId] on \c T.
       Inherited classes can override the resolved value in their constructor.
    */
    protected internal string _secondForeignKey;

    //! String to identify the third foreign-key column.
    /*!
       Defaults to the name of the property marked with \c [ThirdForeignId] on \c T.
       Inherited classes can override the resolved value in their constructor.
    */
    protected internal string _thirdForeignKey;

    protected MySQLThreeForeignDao()
    {
        _firstForeignKey = EntityKeyResolver.ResolvePropertyName<FirstForeignIdAttribute>(typeof(T));
        _secondForeignKey = EntityKeyResolver.ResolvePropertyName<SecondForeignIdAttribute>(typeof(T));
        _thirdForeignKey = EntityKeyResolver.ResolvePropertyName<ThirdForeignIdAttribute>(typeof(T));
    }

    // Implementation to Read() method from IThreeForeignDao interface.
    public virtual T? Read(Guid id1, Guid id2, Guid id3)
    {
        MySQLRetryPolicy.EnsureOpen(_connection);
        T? entity = default(T);
        _sb = new StringBuilder();
        _sb.Append("SELECT * FROM ").Append(_tableName)
            .Append(" WHERE ").Append(_firstForeignKey).Append(" = '").Append(id1.ToString()).Append("' ")
            .Append(" AND ").Append(_secondForeignKey).Append(" = '").Append(id2.ToString()).Append("' ")
            .Append(" AND ").Append(_thirdForeignKey).Append(" = '").Append(id3.ToString()).Append("';");
        _mySqlReader = GetCommandByText(_sb).ExecuteReader();
        if (_mySqlReader.Read())
        {
            entity = MapReaderToEntity();
        }
        _mySqlReader.Close();
        return entity;
    }

    // Implementation to Delete() method from IThreeForeignDao interface.
    public virtual bool Delete(Guid id1, Guid id2, Guid id3)
    {
        MySQLRetryPolicy.EnsureOpen(_connection);
        _sb = new StringBuilder();
        _sb.Append("DELETE FROM ").Append(_tableName)
            .Append(" WHERE ").Append(_firstForeignKey).Append(" = '").Append(id1.ToString()).Append("' ")
            .Append(" AND ").Append(_secondForeignKey).Append(" = '").Append(id2.ToString()).Append("' ")
            .Append(" AND ").Append(_thirdForeignKey).Append(" = '").Append(id3.ToString()).Append("';");
        _mySqlReader = GetCommandByText(_sb).ExecuteReader();
        int recordsAffected = _mySqlReader.RecordsAffected;
        _mySqlReader.Close();

        return recordsAffected > 0;
    }

    // Implementation to ReadAsync() method from IThreeForeignDao interface.
    public virtual async Task<T?> ReadAsync(Guid id1, Guid id2, Guid id3)
    {
        _sb = new StringBuilder();
        _sb.Append("SELECT * FROM ").Append(_tableName)
            .Append(" WHERE ").Append(_firstForeignKey).Append(" = '").Append(id1.ToString()).Append("' ")
            .Append(" AND ").Append(_secondForeignKey).Append(" = '").Append(id2.ToString()).Append("' ")
            .Append(" AND ").Append(_thirdForeignKey).Append(" = '").Append(id3.ToString()).Append("';");
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

    // Implementation to DeleteAsync() method from IThreeForeignDao interface.
    public virtual async Task<bool> DeleteAsync(Guid id1, Guid id2, Guid id3)
    {
        _sb = new StringBuilder();
        _sb.Append("DELETE FROM ").Append(_tableName)
            .Append(" WHERE ").Append(_firstForeignKey).Append(" = '").Append(id1.ToString()).Append("' ")
            .Append(" AND ").Append(_secondForeignKey).Append(" = '").Append(id2.ToString()).Append("' ")
            .Append(" AND ").Append(_thirdForeignKey).Append(" = '").Append(id3.ToString()).Append("';");
        return await MySQLRetryPolicy.ExecuteAsync(_connection, async () =>
        {
            _mySqlReader = (MySqlDataReader)await GetCommandByText(_sb).ExecuteReaderAsync();
            int recordsAffected = _mySqlReader.RecordsAffected;
            await _mySqlReader.CloseAsync();

            return recordsAffected > 0;
        });
    }
}
