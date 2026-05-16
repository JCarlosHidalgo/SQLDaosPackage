namespace SQLDaosPackage.Daos;

/// <summary>
/// Defines behaviour that any other Dao type must have.
/// </summary>
 /*!
      This project structures Daos according to the way a table depends on others 
      through its foreign key relationships:\n
      Depending on the number of relationships, the Dao will inherit from a different 
      interface, but all Daos interfaces will inherit from \c IDao.\n
      Also, to manage convention to inherited interfaces, all MySQL tables that
      have a primary key used to identification purposes must have \c Id as the 
      identificator column's name.
      \param T Is the entity over this interface provides its methods.
  */
public interface IDao<T>
{
    //! Creates a new entity.
     /*!
        \param element Is the entity to be created.
        \return The number of rows affected by the query.
     */
    int Create(T element);

    //! Uses a List of \c T to obtain all entities from table.
    List<T> ReadAll();

    //! Updates an entity.
     /*!
        \param element Is the entity to be updated.
        \return The number of rows affected by the query.
     */
    int Update(T element);

    //! Asynchronous version of \c Create.
    Task<int> CreateAsync(T element);

    //! Asynchronous version of \c ReadAll.
    Task<List<T>> ReadAllAsync();

    //! Asynchronous version of \c Update.
    Task<int> UpdateAsync(T element);
}
