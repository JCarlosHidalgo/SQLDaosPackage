namespace SQLDaosPackage.Entities.Attributes;

/// <summary>
/// Marks the second foreign Guid property of an entity backed by a TwoForeign or
/// ThreeForeign Dao.
/// </summary>
 /*!
    Apply on the property whose value matches the second foreign-key column of the
    table backed by \c MySQLTwoForeignDao<T> or \c MySQLThreeForeignDao<T>.
    Exactly one property per entity must carry this attribute.
  */
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SecondForeignIdAttribute : Attribute
{
}
