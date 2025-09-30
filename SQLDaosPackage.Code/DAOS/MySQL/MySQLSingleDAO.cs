using System.Text;
using System.Data;

using MySql.Data.MySqlClient;

namespace SQLDaosPackage.DAOS.MySQL;

/// <summary>
/// This class implements concrete functionality from \c ISingleDAO interface.
/// </summary>
 /*!
    This DAO represents a MySQL table that does not have any foreign key relationships
    with other ones, it also works with tables that only have one foreign key
    relationship.
    \param T Is the entity over this interface provides its methods.
  */
public abstract class MySQLSingleDAO<T> : MySQLBaseDAO<T>, ISingleDAO<T>
{
    // Implementation to Read() method from ISingleDAO interface.
    public T? Read(Guid id)
    {
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

    // Implementation to Delete() method from ISingleDAO interface.
    public bool Delete(Guid id)
    {
        _sb = new StringBuilder();
        _sb.Append("DELETE FROM ").Append(_tableName).Append(" WHERE Id = '").Append(id.ToString()).Append("';");
        MySqlCommand com = GetCommandByText(_sb);
        _mySqlReader = com.ExecuteReader();
        int recordsAffected = _mySqlReader.RecordsAffected;
        _mySqlReader.Close();

        return recordsAffected > 0;
    }
}
