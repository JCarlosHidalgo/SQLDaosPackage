namespace SQLDaosPackage.Entities.Attributes;

/// <summary>
/// Declares the marked integer property as a 32-bit integer column.
/// </summary>
 /*!
    The MySQL implementation maps this to \c INT; a future PostgreSQL implementation
    maps it to \c INTEGER.
  */
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class IntegerAttribute : Attribute
{
}
