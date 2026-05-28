namespace SQLDaosPackage.Entities.Attributes;

/// <summary>
/// Declares the marked bool property as a boolean column.
/// </summary>
 /*!
    The MySQL implementation maps this to \c TINYINT(1) with values \c 0 / \c 1;
    a future PostgreSQL implementation maps it to \c BOOLEAN.
  */
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class FlagAttribute : Attribute
{
}
