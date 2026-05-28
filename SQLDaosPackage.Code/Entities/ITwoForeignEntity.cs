namespace SQLDaosPackage.Entities;

/// <summary>
/// Marker contract for entities whose persistence is handled by a TwoForeign Dao.
/// </summary>
 /*!
    Implementing this interface signals that the entity is a many-to-many join row
    whose identity is the pair of two foreign Guids, and is a valid type parameter
    for \c ITwoForeignDao<T>. The two foreign properties must be marked with
    \c [FirstForeignId] and \c [SecondForeignId] so they can be discovered by
    reflection.
  */
public interface ITwoForeignEntity
{
}
