namespace SQLDaosPackage.Entities.Attributes;

/// <summary>
/// Declares the marked TimeOnly property as a time-of-day column.
/// </summary>
 /*!
    The MySQL implementation maps this to \c TIME; a future PostgreSQL
    implementation maps it to \c TIME.
  */
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class TimeAttribute : Attribute
{
}
