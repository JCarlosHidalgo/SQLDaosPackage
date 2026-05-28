namespace SQLDaosPackage.Entities.Attributes;

/// <summary>
/// Marks the third foreign Guid property of an entity backed by a ThreeForeign Dao.
/// </summary>
 /*!
    Apply on the property whose value matches the third foreign-key column of the
    table backed by \c MySQLThreeForeignDao<T>. Exactly one property per entity
    must carry this attribute.
  */
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class ThirdForeignIdAttribute : Attribute
{
}
