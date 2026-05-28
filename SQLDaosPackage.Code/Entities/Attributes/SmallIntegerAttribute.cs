namespace SQLDaosPackage.Entities.Attributes;

/// <summary>
/// Declares the marked integer property as a small-range integer column.
/// </summary>
 /*!
    The MySQL implementation maps this to \c TINYINT; a future PostgreSQL
    implementation maps it to \c SMALLINT.
  */
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SmallIntegerAttribute : Attribute
{
}
