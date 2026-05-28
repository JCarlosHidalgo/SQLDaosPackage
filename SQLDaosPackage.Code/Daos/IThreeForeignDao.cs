using SQLDaosPackage.Entities;

namespace SQLDaosPackage.Daos;

/// <summary>
/// Defines behaviour that an entity with three dependencies must have.
/// </summary>
 /*!
    This Dao represents a MySQL table that has three foreign key relationships with
    other tables, where the row identity is the triple of three foreign Guids.
    \param T Is the entity over this interface provides its methods, constrained to
    \c IThreeForeignEntity to enforce the marker contract at compile time.
  */
public interface IThreeForeignDao<T> : IDao<T> where T : IThreeForeignEntity
{
    //! Obtains an entity based on three Guids.
    /*!
       This method returns a possibly-null \c T type, null case occurs when there's
       no entity on table which Id's match with \c id parameters.
       \param id1 Is first identifier to apply matching process.
       \param id2 Is second identifier to apply matching process.
       \param id3 Is third identifier to apply matching process.
       \return Possibly-null entity of \c T type.
    */
    T? Read(Guid id1, Guid id2, Guid id3);

    //! Deletes an entity based on three Guid identifiers.
    /*!
       \param id1 Is first identifier to apply matching process.
       \param id2 Is second identifier to apply matching process.
       \param id3 Is third identifier to apply matching process.
       \return Boolean indicating if operation has effect over table.
    */
    bool Delete(Guid id1, Guid id2, Guid id3);

    //! Asynchronous version of \c Read.
    Task<T?> ReadAsync(Guid id1, Guid id2, Guid id3);

    //! Asynchronous version of \c Delete.
    Task<bool> DeleteAsync(Guid id1, Guid id2, Guid id3);
}
