using System.Data;
using System.Text;

using MySql.Data.MySqlClient;
using NUnit.Framework.Legacy;

using SQLDaosPackage.Injectors;

namespace Test.MySQL.Connector;

[TestFixture]
public class InjectorTest
{
    private MySqlConnection? _databaseConnection()
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

    class Injector : DataInjector
    {
        public Injector()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("LOAD DATA INFILE '/var/lib/mysql-files/InjectionTest.csv' ")
            .Append("INTO TABLE User ")
            .Append("FIELDS TERMINATED BY ',' ")
            .Append("IGNORE 1 LINES");
            _injectionCommand = sb.ToString();
        }
    }

    private void TruncateAllTables(MySqlConnection _conn)
    {
        MySqlCommand com = new MySqlCommand("TruncateAllTables", _conn);
        com.CommandType = CommandType.StoredProcedure;
        com.ExecuteNonQuery();
    }

    [Test]
    public void Check_succesfull_csv_file_injection()
    {
        MySqlConnection? _connection = _databaseConnection();
        ClassicAssert.NotNull(_connection);
        
        DataInjector injector = new Injector();
        int injectionResult = injector.InjectData(_connection);
        Assert.That(injectionResult, Is.EqualTo(12));
        TruncateAllTables(_connection);
    }

    [Test]
    public void Check_unsuccesfull_csv_file_injection()
    {
        MySqlConnection _connection = new MySqlConnection("");

        DataInjector injector = new Injector();
        int injectionResult = injector.InjectData(_connection);

        Assert.That(injectionResult, Is.EqualTo(-1));
    }
}
