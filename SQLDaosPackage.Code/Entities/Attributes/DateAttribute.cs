namespace SQLDaosPackage.Entities.Attributes;

/// <summary>
/// Declares the marked DateOnly property as a date column.
/// </summary>
 /*!
    The MySQL implementation maps this to \c DATE; a future PostgreSQL
    implementation maps it to \c DATE.
  */
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class DateAttribute : Attribute
{
}
