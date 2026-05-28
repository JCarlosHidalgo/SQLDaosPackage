namespace SQLDaosPackage.Entities.Attributes;

/// <summary>
/// Declares the marked DateTime property as a microsecond-precision timestamp column.
/// </summary>
 /*!
    The MySQL implementation maps this to \c DATETIME(6); a future PostgreSQL
    implementation maps it to \c TIMESTAMP(6).
  */
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class PreciseTimestampAttribute : Attribute
{
}
