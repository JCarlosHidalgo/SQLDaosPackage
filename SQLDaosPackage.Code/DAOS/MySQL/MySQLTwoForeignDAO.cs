using System.Text;
using System.Data;

using MySql.Data.MySqlClient;

namespace SQLDaosPackage.DAOS.MySQL;

public abstract class MySQLTwoForeignDAO<T> : MySQLBaseDAO<T>, ITwoForeignDAO<T>
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

    // Implementation to Read() method from ITwoForeignDAO interface.
    public T? Read(Guid id1, Guid id2)
    {
        _sb = new StringBuilder();
        _sb.Append("SELECT * FROM ").Append(_tableName)
            .Append(" WHERE ").Append(_firstForeignKey).Append(" = '").Append(id1.ToString()).Append("' ")
            .Append(" AND ").Append(_secondForeignKey).Append(" = '").Append(id2.ToString()).Append("';");
        _mySqlReader = GetCommandByText(_sb).ExecuteReader();
        if (_mySqlReader.Read())
        {
            return MapReaderToEntity();
        }
       
        _mySqlReader.Close();
        return default(T);
    }

    // Implementation to Delete() method from ITwoForeignDAO interface.
    public bool Delete(Guid id1, Guid id2)
    {
        _sb = new StringBuilder();
        _sb.Append("DELETE FROM ").Append(_tableName)
            .Append(" WHERE ").Append(_firstForeignKey).Append(" = '").Append(id1.ToString()).Append("' ")
            .Append(" AND ").Append(_secondForeignKey).Append(" = '").Append(id2.ToString()).Append("';");
        _mySqlReader = GetCommandByText(_sb).ExecuteReader();
        int recordsAffected = _mySqlReader.RecordsAffected;
        _mySqlReader.Close();

        return recordsAffected > 0;
    }
}
