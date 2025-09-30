namespace SQLDaosPackage.DAOS;

/// <summary>
/// Defines behaviour that an independent entity must have.
/// </summary>
 /*!
    This DAO represents a MySQL table that does not have any foreign key relationships
    with other ones, it also works with tables that only have one foreign key
    relationship.
    \param T Is the entity over this interface provides its methods.
  */
public interface ISingleDAO<T> : IDAO<T>
{
    //! Obtains an entity based on a Guid.
     /*!
        This method returns a possibly-null \c T type, null case occurs when there is
        no entity on table which Id matches with \c id parameter.
        \param id Is the identifier used to apply the matching process.
        \return Possibly-null entity of \c T type.
     */
    T? Read(Guid id);

    //! Deletes an entity based on Guid identifier.
     /*!
        \param id Is the identifier used to apply the matching process.
        \return Boolean indicating if operation has effect over table.
     */
    bool Delete(Guid id);
}
