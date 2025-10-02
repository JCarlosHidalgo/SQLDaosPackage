using MySql.Data;
using MySql.Data.MySqlClient;

namespace SQLDaosPackage.Injectors;

/// <summary>
/// Interface to inject data files over a MySQL database.
/// </summary>
 /*!
   This interface is used to detect any .csv files inside a MySQL server and inject them 
   into the database.
 */
public interface IDataInjector
{
    //! Method to inject the data.
     /*!
        \param connection Is the active MySQL connection which this method will use.
        \return The number of rows inserted on the database.
     */
    int InjectData(MySqlConnection connection);
}
