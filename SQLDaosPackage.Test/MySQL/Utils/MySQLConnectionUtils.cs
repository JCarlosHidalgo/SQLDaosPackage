using MySql.Data.MySqlClient;

namespace Test.MySQL.Utils;

public class MySQLConnectionUtils
{
    public MySqlConnection? GetConnection()
    {
        MySqlConnection _connection = new MySqlConnection("server=SQLDaosPackageMySQLHost;port=3306;uid=root;pwd=admin;database=SchemaTest;Allow User Variables=True");
        try
        {
            _connection.Open();
        }
        catch
        {
            _connection = null!;
        }

        return _connection;
    }
}
