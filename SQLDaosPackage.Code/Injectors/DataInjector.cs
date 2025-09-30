using MySql.Data;
using MySql.Data.MySqlClient;

namespace SQLDaosPackage.Injectors;

/// <summary>
/// Class that implements methods from \c IDataInjector interface.
/// </summary>
 /*!
    This class applies \c IDataInjector constraint to inject data from .csv files to any 
    MySQL database.\n
    Namespaces Dependencies:
   - From \c MySql.Data :
      - \c MySql.Data
      - \c MySql.Data.MySqlClient
  */
public class DataInjector : IDataInjector
{
    //! MySQL command to inject the data.
     /*!
        To assign attribute's value, use the constructor of inherited class.
     */
    protected internal string? _injectionCommand;

    public int InjectData(MySqlConnection connection)
    {
        int injectionResult = 0;

        try
        {
            MySqlCommand injectionCommand = new MySqlCommand(_injectionCommand, connection);
            injectionCommand.ExecuteNonQuery();
        }
        catch (MySql.Data.MySqlClient.MySqlException ex)
        {
            injectionResult = ex.Number;
        }
        return injectionResult;
    }
}
