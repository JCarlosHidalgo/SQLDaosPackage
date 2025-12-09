using NUnit.Framework.Legacy;

using SQLDaosPackage.Injectors;

using System.Data;
using System.Text;

using MySql.Data.MySqlClient;

using Test.MySQL.Utils;

namespace Test.MySQL.Connector;

[TestFixture]
public class InjectorTest
{
    private MySqlConnection? _databaseConnection;

    [OneTimeSetUp]
    public void Init_connection()
    {
        MySQLConnectionUtils test = new MySQLConnectionUtils();
        _databaseConnection = test.GetConnection();
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        _databaseConnection?.Dispose();
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
        MySqlConnection? _connection = _databaseConnection;
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
