namespace SQLDaosPackage.DAOS;

/// <summary>
/// Defines behaviour that an entity with two dependencies must have.
/// </summary>
 /*!
    This DAO represents a MySQL table that has two foreign key relationships with
    another tables, it is important to mention that the table must not have own
    identifier (for that purpose \c ISingleDAO is used) and it is a many-to-many 
    relation table.
    \param T Is the entity over this interface provides its methods.
  */
public interface ITwoForeignDAO<T> : IDAO<T>
{
    //! Obtains an entity based on two Guids.
     /*!
        This method returns a possibly-null \c T type, null case occurs when there's
        no entity on table which Id's match with \c id parameters.
        \param id1 Is first identifier to apply matching process.
        \param id2 Is second identifier to apply matching process.
        \return Possibly-null entity of \c T type.
     */
    T? Read(Guid id1, Guid id2);

    //! Deletes an entity based on Guid identifiers.
     /*!
        \param id1 Is first identifier to apply matching process.
        \param id2 Is second identifier to apply matching process.
        \return Boolean indicating if operation has effect over table.
     */
    bool Delete(Guid id1, Guid id2);
}
