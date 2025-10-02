using System.Text;
using System.Data;

using MySql.Data.MySqlClient;

namespace SQLDaosPackage.DAOS.MySQL;

/// <summary>
/// Refactor common behaviour of MySQL DAOs.
/// </summary>
 /*!
    This class implements the basic setup that any other MySQL DAO must have.
  */
public abstract class MySQLBaseDAO<T> : IDAO<T>
{
    //! MySQL connection to determine database host.
     /*!
        By default, this attribute must be initialized at inherited class' constructor.
     */
    public required MySqlConnection _connection;

    //! MySQL command to initialize any interaction with database.
     /*!
        To implement extra commands implementation, it is necessary to initialize a new
        \c MySqlCommand object, this atribute is private to not affect current commands
        execution flow.
     */
    private protected MySqlCommand? _dbCommand;

    //! MySQL reader to execute query commands using \c _connection attribute.
     /*!
        This attribute makes possible obtaining values from MySQL queries, when
        implementing methods to extract values obtained from a query, use 
        \c MySqlDataReader methods. 
     */
    protected internal MySqlDataReader? _mySqlReader;

    //! String to set the table name.
     /*!
        To set this attribute, use class' constructor.\n
        Attribute's value must be exactly the same as table's name in database.
     */
    protected internal string _tableName = string.Empty;

    //! StringBuilder to build all MySQL commands.
     /*!
        When implementing methods inherited from this class, use this attribute to build
        MySQL commands, attribute's initialization is necessary at the beginning of
        inherited functions.
     */ 
    protected internal StringBuilder? _sb;

    //! \c T type to initialize entity casting from queries to C# object.
     /*!
        \c _entity must be used to put all C# implemented-by-library objects 
        from reader to entity. 
     */
    protected internal T? _entity;    
    
    //! List of \c T to return multiple \c T entities.
     /*!
        When implementing methods which execute a query that return multiple rows,
        initialize this object.
     */
    protected internal List<T>? _entitiesList;

    //! \c T type function to implement entity casting.
     /*!
        \c _entity Attribute can be used inside this function to follow  dynamic programming
        practices, but a new \c T type object can be initialized also.
     */
    protected internal abstract T MapReaderToEntity();

    //! List of \c T types function to implement entity casting.
     /*!
        \c _entitiesList Attribute can be used inside this function to follow dynamic
        programming practices, but a new list of \c T type can be initialized also.\n
        By default, when implementing this function,\c ReadAll() method is ready to use.
     */
    protected internal abstract List<T> MapReaderToEntitiesList();

    //! String builder to set the command's text to create a new entity.
     /*!
        By default, when implementing this function, \c Create() method is ready
        to use.
     */
    protected internal abstract StringBuilder CreateCommandIntoStringBuilder(T entity);

    //! String builder to set the command's text to update an entity.
     /*!
        By default, when implementing this function, \c Update() method is ready
        to use.
     */
    protected internal abstract StringBuilder UpdateCommandIntoStringBuilder(T entity);

    // Implementation to Create() method from IDAO interface.
    public int Create(T entity)
    {
        _sb = CreateCommandIntoStringBuilder(entity);
        return GetCommandByText(_sb).ExecuteNonQuery();
    }

    // Implementation to ReadAll() method from IDAO interface.
    public List<T> ReadAll()
    {
        _dbCommand = new MySqlCommand();
        _dbCommand.Connection = _connection;
        _dbCommand.CommandText = _tableName;
        _dbCommand.CommandType = CommandType.TableDirect;
        _mySqlReader = _dbCommand.ExecuteReader();
        return MapReaderToEntitiesList();
    }

    // Implementation to Update() method from IDAO interface.
    public int Update(T entity)
    {
        _sb = UpdateCommandIntoStringBuilder(entity);
        _mySqlReader = GetCommandByText(_sb).ExecuteReader();
        int toReturn = _mySqlReader.RecordsAffected;
        _mySqlReader.Close();
        return toReturn;
    }

    //! \c MySqlCommand function to setup a DAO command.
     /*!
        /return A command prepared to be used in another functions.
     */
    private protected MySqlCommand GetCommandByText(StringBuilder sb)
    {
        _dbCommand = new MySqlCommand();
        _dbCommand.Connection = _connection;
        _dbCommand.CommandText = sb.ToString();
        return _dbCommand;
    }

    //! /c MySqlCommand function to execute MySQL procedures.
     /*!
        /return A command configured to execute stored procedures.
     */
    protected internal MySqlCommand GetCommandStoredProcedure(string procedureName)
    {
        _dbCommand = new MySqlCommand(procedureName,_connection);
        _dbCommand.CommandType = CommandType.StoredProcedure;
        return _dbCommand;
    }
}