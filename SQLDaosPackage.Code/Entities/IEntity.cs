namespace SQLDaosPackage.Entities;

/// <summary>
/// Marker contract for entities whose persistence is handled by a Single Dao.
/// </summary>
 /*!
    Implementing this interface signals that the entity owns a unique Guid identifier
    and is a valid type parameter for \c ISingleDao<T>. The identifier property must
    be marked with \c [Identificator] so it can be discovered by reflection.
  */
public interface IEntity
{
}
