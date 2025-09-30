using System.Data;
using MySql.Data.MySqlClient;

namespace Test.MySQL.Connector;

[TestFixture]
public class ConnectorTest
{
    [Test]
    public void Check_unavailability_to_connect_to_invalid_MySQL_host()
    {
        MySqlConnection _connection = new MySqlConnection("");
        MySqlErrorCode _errorCode = MySqlErrorCode.None;
        try
        {
            _connection.Open();
        }
        catch (MySqlException ex)
        {
            _errorCode = (MySqlErrorCode)ex.Number;
        }

    
        Assert.That(_errorCode, Is.EqualTo(MySqlErrorCode.UnableToConnectToHost));
    }
}
