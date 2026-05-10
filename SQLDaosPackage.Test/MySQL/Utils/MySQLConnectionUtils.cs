using MySql.Data.MySqlClient;

namespace Test.MySQL.Utils;

public class MySQLConnectionUtils
{
    private static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING")
        ?? throw new InvalidOperationException("MYSQL_CONNECTION_STRING environment variable is not set.");

    public MySqlConnection? GetConnection()
    {
        MySqlConnection _connection = new MySqlConnection(ConnectionString);
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
