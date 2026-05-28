namespace SQLDaosPackage.Entities;

/// <summary>
/// Marker contract for entities whose persistence is handled by a ThreeForeign Dao.
/// </summary>
 /*!
    Implementing this interface signals that the entity is a join row whose identity
    is the triple of three foreign Guids, and is a valid type parameter for
    \c IThreeForeignDao<T>. The three foreign properties must be marked with
    \c [FirstForeignId], \c [SecondForeignId] and \c [ThirdForeignId] so they can
    be discovered by reflection.
  */
public interface IThreeForeignEntity
{
}
