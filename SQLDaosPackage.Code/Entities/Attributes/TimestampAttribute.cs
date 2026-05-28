namespace SQLDaosPackage.Entities.Attributes;

/// <summary>
/// Declares the marked DateTime property as a second-precision timestamp column.
/// </summary>
 /*!
    The MySQL implementation maps this to \c DATETIME; a future PostgreSQL
    implementation maps it to \c TIMESTAMP. For microsecond precision use
    \c [PreciseTimestamp] instead.
  */
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class TimestampAttribute : Attribute
{
}
