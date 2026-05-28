namespace SQLDaosPackage.Entities.Attributes;

/// <summary>
/// Marks the Guid property that uniquely identifies a Single entity.
/// </summary>
 /*!
    Apply on the property whose value matches the primary key column of the table
    backed by \c MySQLSingleDao<T>. Exactly one property per entity must be marked.
  */
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class IdentificatorAttribute : Attribute
{
}
