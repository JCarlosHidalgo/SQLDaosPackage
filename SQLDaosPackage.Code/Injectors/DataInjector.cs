using MySql.Data;
using MySql.Data.MySqlClient;

namespace SQLDaosPackage.Injectors;

/// <summary>
/// Class that implements methods from \c IDataInjector interface.
/// </summary>
 /*!
    This class applies \c IDataInjector constraint to inject data from .csv files to any 
    MySQL database.
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
            injectionResult = injectionCommand.ExecuteNonQuery();
        }
        catch (Exception ex)
        { 
            injectionResult = ex.HResult;
            return -1; 
        }
        return injectionResult;
    }
}
