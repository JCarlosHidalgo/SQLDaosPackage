namespace SQLDaosPackage.DAOS;

/// <summary>
/// Defines behaviour that any other DAO type must have.
/// </summary>
 /*!
      This project structures DAOs according to the way a table depends on others 
      through its foreign key relationships:\n
      Depending on the number of relationships, the DAO will inherit from a different 
      interface, but all DAOs interfaces will inherit from \c IDAO.\n
      Also, to manage convention to inherited interfaces, all MySQL tables that
      have a primary key used to identification purposes must have \c Id as the 
      identificator column's name.
      \param T Is the entity over this interface provides its methods.
  */
public interface IDAO<T>
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
}
