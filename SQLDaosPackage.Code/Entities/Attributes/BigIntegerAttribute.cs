namespace SQLDaosPackage.Entities.Attributes;

/// <summary>
/// Declares the marked integer property as a 64-bit integer column.
/// </summary>
 /*!
    The MySQL implementation maps this to \c BIGINT; a future PostgreSQL
    implementation maps it to \c BIGINT.
  */
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class BigIntegerAttribute : Attribute
{
}
